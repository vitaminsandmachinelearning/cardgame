using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour
{
    #region VARIABLES
    [Header("General")]
    public GameObject deckObject;
    public GameObject handObject;
    public GameObject discardObject;
    public Deck deck;
    public Hand hand;
    public Discard discard;

    [Header("Player")]
    public PlayerController player;
    public GameObject playerObject;
    public List<Card> cardCatalog;

    [Header("Enemies")]
    List<GameObject> enemies = new List<GameObject>();
    public GameObject defaultEnemy;
    public List<Enemy> enemyCatalog;
    public bool enemyTurn = false;

    [Header("BattleControl")]
    public bool targetting = false;
    bool waitingCardEffects = false;
    GameObject target = null;
    List<Action> effects = new List<Action>();
    public GameObject hoverCardObject;
    GameObject currentHoveringCard;

    #endregion
    void Awake()
    {
        deck = deckObject.GetComponent<Deck>();
        hand = handObject.GetComponent<Hand>();
        discard = discardObject.GetComponent<Discard>();
        player = playerObject.GetComponent<PlayerController>();
        LoadDeck();
    }
    void Start()
    {
        BattleStart();
    }
    void BattleStart()
    {
        StartPlayerTurn();
        for(int i = 0; i < 1; i++)
            SpawnEnemy();
    }
    #region CARDS
    public void UseCard(GameObject cardObject)
    {
        if (!enemyTurn && !targetting && !waitingCardEffects)
        {
            Card card = cardObject.GetComponent<CardController>().card;
            if (PlayerData.energy >= card.manaCost)
            {
                player.ChangeEnergy(-card.manaCost);
                hoverCardObject.GetComponent<CardController>().card = card;
                currentHoveringCard = Instantiate(hoverCardObject);
                currentHoveringCard.transform.SetParent(GameObject.Find("Canvas").transform, false);
                ParseCard(card);
                hand.Remove(cardObject);
            }
        }
    }
    void RunCardEffects()
    {
        foreach (Action effect in effects)
            effect.Invoke();
        waitingCardEffects = false;
        Destroy(currentHoveringCard);
    }
    public void SetTarget(GameObject t)
    {
        targetting = false;
        target = t;
    }
    public void ParseCard(Card card)
    {
        effects.Clear();
        StartCoroutine(ParseSkillEffects(card.effectScript));
    }
    IEnumerator ParseSkillEffects(string effectScript)
    {
        string[] commands = effectScript.Split(';');
        foreach (string command in commands)
        {
            string action = "";
            string value = "";
            string targets = "";
            if (command.Split(' ').Length == 2)
            {
                action = command.Split(' ')[0];
                value = command.Split(' ')[1];
            }
            else if (command.Split(' ').Length == 3)
            {
                action = command.Split(' ')[0];
                value = command.Split(' ')[1];
                targets = command.Split(' ')[2];
            }
            switch (action)
            {
                case "Draw":
                    effects.Add(() => ActionDraw(int.Parse(value)));
                    break;
                case "Damage":
                    if (targets.Equals("self"))
                        effects.Add(() => ActionDamage(int.Parse(value), playerObject));
                    else if (targets.Equals("all"))
                        foreach (GameObject enemy in enemies)
                            effects.Add(() => ActionDamage(int.Parse(value), enemy));
                    else
                    {
                        targetting = true;
                        yield return new WaitUntil(() => !targetting);
                        effects.Add(() => ActionDamage(int.Parse(value), target));
                    }
                    break;
                case "Defend":
                    effects.Add(() => ActionDefend(int.Parse(value)));
                    break;
            }
        }
        waitingCardEffects = true;
        RunCardEffects();
    }
    #region ACTIONS
    void ActionDraw(int amount)
    {
        deck.Draw(amount);
    }
    void ActionDamage(int amount, GameObject target)
    {
        //try to get once here to save getting multiple times per enemy
        var e = target.GetComponent<EnemyController>();
        if (target.GetComponent<PlayerController>() != null)
            player.TakeDamage(amount);
        else if (e != null)
            e.TakeDamage(amount);
    }
    void ActionDefend(int amount)
    {
        player.ChangeArmor(amount);
    }
    #endregion
    #endregion
    #region GAMECONTROL
    void SpawnEnemy()
    {
        enemies.Add(Instantiate(defaultEnemy));
        enemies[enemies.Count - 1].GetComponent<EnemyController>().enemy = enemyCatalog[0];
        enemies[enemies.Count - 1].transform.SetParent(GameObject.Find("Canvas").transform, false);
        switch (enemies.Count)
        {
            case 0:
                CheckEndBattle();
                break;
            case 1:
                enemies[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 100);
                break;
            case 2:
                enemies[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(-55, 100);
                enemies[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(55, 100);
                break;
            case 3:
                enemies[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(-110, 100);
                enemies[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 100);
                enemies[2].GetComponent<RectTransform>().anchoredPosition = new Vector2(110, 100);
                break;
        }
    }
    public void StartPlayerTurn()
    {
        enemyTurn = false;
        hand.DiscardAll();
        ActionDraw(PlayerData.turnDrawAmount);
        player.StartTurn();
    }
    public void EndPlayerTurn()
    {
        if(!targetting)
            StartEnemyTurn();
    }
    public void StartEnemyTurn()
    {
        enemyTurn = true;
        foreach (GameObject enemy in enemies)
        {
            var ec = enemy.GetComponent<EnemyController>();
            EnemyActionParser(ec, ec.action, int.Parse(ec.value));
            ec.GetNextAction();
        }
        enemyTurn = false;
        StartPlayerTurn();
    }
    void EnemyActionParser(EnemyController enemy, string action, int value)
    {
        switch (action)
        {
            case "Attack":
                player.TakeDamage(value);
                break;
            case "Defend":
                enemy.ChangeArmor(value);
                break;
        }
    }
    public void AlertPlayerDeath()
    {
        //TODO
        Debug.Log("Player dead");
    }
    public void AlertEnemyDeath(GameObject go)
    {
        enemies.Remove(go);
        Destroy(go);
        CheckEndBattle();
    }
    public void CheckEndBattle()
    {
        if (enemies.Count <= 0)
            Debug.Log("End");
    }
    #endregion
    #region UTILITY
    void LoadDeck()
    {
        StreamReader reader = new StreamReader("Assets/Decks/starter.txt");
        string deckString = reader.ReadToEnd();
        reader.Close();
        string[] deckSplit = deckString.Split(';');
        foreach (string entry in deckSplit)
        {
            string[] entrySplit = entry.Split(':');
            for (int i = 0; i < int.Parse(entrySplit[1]); i++)
                deck.Add(cardCatalog.First(x => x.name == entrySplit[0]));
        }
        deck.Shuffle();
    }
    #endregion
}
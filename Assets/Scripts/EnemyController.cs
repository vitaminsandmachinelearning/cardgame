using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    public Enemy enemy;
    public Text nameText;
    public Text healthText;
    public Text armorText;
    public Text nextAction;
    public Image artwork;
    GameController gc;

    public string action;
    public string value;

    void Awake()
    {
        gc = GameObject.Find("Game Controller").GetComponent<GameController>();
    }
    void Start()
    {
        nameText.text = enemy.name;
        enemy.health = enemy.maxHealth;
        enemy.armor = 0;
        healthText.text = "Health:\n" + enemy.health;
        artwork.sprite = enemy.artwork;
        GetNextAction();
    }

    void UpdateUI()
    {
        healthText.text = "Health:\n" + enemy.health;
        armorText.text = "Armor:\n" + enemy.armor;
    }

    public void GetNextAction()
    {
        string[] actionsSplit = enemy.actionScript.Split(';');
        string actionChoice = actionsSplit[Random.Range(0, actionsSplit.Length)];
        action = actionChoice.Split(' ')[0];
        value = actionChoice.Split(' ')[1];
        if (value.Contains("-"))
            value = value.Split('-')[Random.Range(0, 2)];
        nextAction.text = action + " " + value;
        UpdateUI();
    }

    public void TakeDamage(int amount)
    {
        if (amount > enemy.armor)
        {
            amount -= enemy.armor;
            ChangeArmor(-enemy.armor);
            ChangeHealth(-amount);
        }
        else
            ChangeArmor(-amount);
    }

    public void ChangeHealth(int amount)
    {
        enemy.health += amount;
        if (enemy.health > enemy.maxHealth)
            enemy.health = enemy.maxHealth;
        UpdateUI();
        CheckForDeath();
    }

    public void ChangeArmor(int amount)
    {
        enemy.armor += amount;
        if (enemy.armor < 0)
            enemy.armor = 0;
        UpdateUI();
    }

    void CheckForDeath()
    {
        if(enemy.health <= 0)
            gc.AlertEnemyDeath(gameObject);
    }

    public void OnHover()
    {
        if(gc.targetting)
            artwork.color = new Color(0.5f, 0, 0, 1);
    }

    public void OnStopHover()
    {
        artwork.color = new Color(1, 1, 1, 1);
    }

    public void OnClick()
    {
        if (gc.targetting)
            gc.SetTarget(gameObject);
    }
}

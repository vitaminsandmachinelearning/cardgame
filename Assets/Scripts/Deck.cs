using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Deck : MonoBehaviour
{
    public GameObject GameController;
    public GameObject defaultCard;
    public Text cardsInDeckText;

    GameController gc;
    Hand hand;
    Discard discard;

    public List<Card> cards = new List<Card>();
    void Awake()
    {
        gc = GameController.GetComponent<GameController>();
        hand = gc.hand;
        discard = gc.discard;
    }
    public void UpdateUI()
    {
        cardsInDeckText.text = "Cards in deck: " + cards.Count;
        if (cards.Count == 0)
            cardsInDeckText.text = "Cards in deck: 0";
    }
    public void Add(Card card)
    {
        cards.Add(card);
        UpdateUI();
    }
    public void Remove(int index)
    {
        Destroy(cards[index]);
        cards.RemoveAt(index);
        UpdateUI();
    }
    public void Shuffle()
    {
        for(int i = 0; i < cards.Count - 1; i++)
        {
            var j = Random.Range(i, cards.Count);
            var t = cards[i];
            cards[i] = cards[j];
            cards[j] = t;
        }
    }

    public void ReturnFromDiscard()
    {
        cards.AddRange(discard.GetCards());
        discard.Empty();
        Shuffle();
        UpdateUI();
    }

    public void Draw(int amount)
    {
        while (amount > 0)
        {
            if (cards.Count == 0)
            {
                if (discard.cards.Count > 0)
                    ReturnFromDiscard();
                else
                    break;
            }
            else
            {
                hand.Add(cards[cards.Count - 1]);
                cards.RemoveAt(cards.Count - 1);
                UpdateUI();
                amount--;
            }
        }
    }
}

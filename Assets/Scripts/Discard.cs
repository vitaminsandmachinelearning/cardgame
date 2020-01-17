using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Discard : MonoBehaviour
{
    public Text cardsInDiscardText;

    public List<Card> cards = new List<Card>();

    void Start()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        cardsInDiscardText.text = "Cards in discard: " + cards.Count;
        if (cards.Count == 0)
            cardsInDiscardText.text = "Cards in discard: 0";
    }
    public void Empty()
    {
        cards.Clear();
        UpdateUI();
    }

    public List<Card> GetCards()
    {
        return cards;
    }

    public void Add(Card card)
    {
        cards.Add(card);
        UpdateUI();
    }
}

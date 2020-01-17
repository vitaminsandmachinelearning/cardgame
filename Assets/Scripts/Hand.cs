using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hand : MonoBehaviour
{
    GameController gc;
    Discard discard;
    public static List<GameObject> cards = new List<GameObject>();
    public int maxCards = 15;
    public float maxPosition = 400;
    public float cardWidth = 179.2f;
    public float posY = -256;
    bool cardsAdjacent = true;
    public GameObject DefaultCard;
    public Text cardsInHandText;

    void Start()
    {
        gc = GameObject.Find("Game Controller").GetComponent<GameController>();
        discard = gc.discard;
        UpdateUI();
    }

    public void UpdateUI()
    {
        cardsInHandText.text = "Cards in hand: " + cards.Count;
        if (cards.Count == 0)
            cardsInHandText.text = "Cards in hand: 0";
    }

    public void Add(Card card)
    {
        cards.Add(Instantiate(DefaultCard));
        cards[cards.Count - 1].GetComponent<CardController>().card = card;
        cards[cards.Count-1].transform.SetParent(GameObject.Find("Canvas").transform, false);
        UpdateUI();
        PlaceCards();
    }
    public void DiscardAll()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            discard.Add(cards[i].GetComponent<CardController>().card);
            Destroy(cards[i]);
        }
        cards.Clear();
        UpdateUI();
        PlaceCards();
    }
    public void Remove(GameObject cardObject)
    {
        discard.Add(cardObject.GetComponent<CardController>().card);
        Destroy(cardObject);
        cards.Remove(cardObject);
        UpdateUI();
        PlaceCards();
    }
    void PlaceCards()
    {
        float range = 2 * maxPosition;
        float adjustmentValue;
        float adjustedCardWidth;
        //if cards can fit side by side then let them do that, else overlap
        if (cards.Count * cardWidth > range)
        {
            cardsAdjacent = false;
            adjustmentValue = (cards.Count * cardWidth - range) / cards.Count;
            adjustedCardWidth = cardWidth - adjustmentValue;
        }
        else
        {
            cardsAdjacent = true;
            adjustmentValue = cardWidth / 9f;
            adjustedCardWidth = cardWidth - adjustmentValue;
        }

        if (cardsAdjacent)
            for (int i = 0; i < cards.Count; i++)
                cards[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(-((cards.Count / 2f) * cardWidth) + i * cardWidth + cardWidth / 2, posY);
        else
            for (int i = 0; i < cards.Count; i++)
                cards[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(-((cards.Count / 2f) * adjustedCardWidth) + i * adjustedCardWidth + adjustedCardWidth / 2, posY);
    }
}

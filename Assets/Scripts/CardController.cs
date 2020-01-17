using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardController : MonoBehaviour
{
    public Card card;
    public Text nameText;
    public Text manaText;
    public Text descriptionText;
    public Image artwork;
    private float hoverScale = 1.2f;
    private Vector3 defaultPos;
    private float hoverPosY = 268.8f;
    GameController gc;

    void Awake()
    {
        gc = GameObject.Find("Game Controller").GetComponent<GameController>();
    }

    void Start()
    {
        nameText.text = card.name;
        manaText.text = card.manaCost.ToString();
        descriptionText.text = card.description;
        artwork.sprite = card.artwork;
    }

    public void OnHover(BaseEventData ed)
    {
        foreach (Transform child in transform)
        {
            child.localScale *= hoverScale;
            child.localPosition = new Vector2(child.localPosition.x, hoverPosY);
        }
    }

    public void OnStopHover(BaseEventData ed)
    {
        foreach (Transform child in transform)
        {
            child.localScale /= hoverScale;
            child.localPosition = new Vector2(0,0);
        }
    }

    public void OnClick(BaseEventData ed)
    {
        gc.UseCard(gameObject);
    }
}

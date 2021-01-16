using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour, IPointerClickHandler
{

    public CardData cardData;

    public Text nameText;
    public Text descriptionText;

    public Image artworkImage;

    public Text costText;
    public Text damageText;
    public Text blockText;

    [HideInInspector]
    public CombatController combatController;
    IEnumerator CardFollower;
    private DeckDisplayManager deckDisplayManager;


    public void Awake()
    {
        CardFollower = FollowMouseIsSelected();
        deckDisplayManager = WorldSystem.instance.deckDisplayManager;
    }

    public void UpdateDisplay()
    {
        nameText.text = cardData.name;

        artworkImage.sprite = cardData.artwork;

        costText.text = cardData.cost.ToString();

        descriptionText.text = "";

        for(int i = 0; i < cardData.Effects.Count; i++)
        {
            descriptionText.text += cardData.Effects[i].Type.ToString() + ":" + cardData.Effects[i].Value;
            if (cardData.Effects[i].Times != 1) descriptionText.text += " " + cardData.Effects[i].Times + " times.";
            if (i != cardData.Effects.Count - 1) descriptionText.text += "\n";
        }
    }

    public void OnMouseEnter()
    {
        transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);

        if(WorldSystem.instance.worldState == WorldState.Combat)
        {
            transform.SetAsLastSibling();
        }
    }

    public void OnMouseExit()
    {
        transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);

        if(WorldSystem.instance.worldState == WorldState.Combat)
        {
            combatController.ResetSiblingIndexes();
        }
    }

    public void ResetScale()
    {
        transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);
    }
    private IEnumerator FollowMouseIsSelected()
    {
        while (true)
        {
            Vector2 outCoordinates;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(combatController.GetComponent<RectTransform>(), Input.mousePosition, null, out outCoordinates);
            GetComponent<RectTransform>().localPosition = outCoordinates;
            yield return new WaitForSeconds(0);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            OnMouseClick();
        else if (eventData.button == PointerEventData.InputButton.Right)
            OnMouseRightClick();
    }

    public void OnMouseClick()
    {
        switch (WorldSystem.instance.worldState)
        {
            case WorldState.Combat:

                if (!combatController.CardisSelectable(this))
                    break;

                combatController.ActiveCard = this;
                StartCoroutine(CardFollower);
                break;

            case WorldState.Shop:

                WorldSystem.instance.shopManager.shop.PurchaseCard(this);
                break;

            case WorldState.Display:

                DisplayCard();
                break;

            default:
                break;
        }
    }

    IEnumerator LerpPosition(Vector3 endValue, float duration)
    {
    float time = 0;
    Vector3 startValue = transform.position;

    while (time < duration)
    {
        transform.position = Vector3.Lerp(startValue, endValue, time / duration);
        time += Time.deltaTime;
        yield return null;
    }
    transform.position = endValue;
    }

    void DisplayCard()
    {
        if(deckDisplayManager.selectedCard == null)
        {
            deckDisplayManager.previousPosition = transform.position;
            deckDisplayManager.selectedCard = this;
            StartCoroutine(LerpPosition(new Vector3 (Screen.width * 0.5f, Screen.height * 0.5f, 0), 0.1f));
            transform.localScale += new Vector3(0.5f, 0.5f, 0.5f);
        }
        else if(deckDisplayManager.selectedCard != this)
        {
            deckDisplayManager.selectedCard.transform.position = deckDisplayManager.previousPosition;
            deckDisplayManager.selectedCard.transform.localScale -= new Vector3(0.5f, 0.5f, 0.5f);
            deckDisplayManager.previousPosition = transform.position;

            deckDisplayManager.selectedCard = this;
            StartCoroutine(LerpPosition(new Vector3 (Screen.width * 0.5f, Screen.height * 0.5f, 0), 0.1f));
            transform.localScale += new Vector3(0.5f, 0.5f, 0.5f);
        }
        else
        {
            ResetCardPosition();
        }
    }

    public void ResetCardPosition()
    {
        transform.position = deckDisplayManager.previousPosition;
        transform.localScale -= new Vector3(0.5f, 0.5f, 0.5f);
        deckDisplayManager.selectedCard = null;
    }

    public void OnMouseRightClick()
    {
        if(WorldSystem.instance.worldState == WorldState.Combat)
        {
            //Debug.Log("Card Right-Clicked");
            combatController.CancelCardSelection(this.gameObject);
            StopCoroutine(CardFollower);
        }
    }
}

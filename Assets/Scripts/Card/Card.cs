using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class Card : MonoBehaviour, IPointerClickHandler
{

    public CardData cardData;
    public Text nameText;
    public Text descriptionText;
    public Image artworkImage;

    public Text costText;
    public Text damageText;
    public Text blockText;
    public WorldState previousState;

    public void BindCardData()

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

    public abstract void OnMouseEnter();

    public abstract void OnMouseExit();
    public abstract void ResetScale();
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            OnMouseClick();
        else if (eventData.button == PointerEventData.InputButton.Right)
            OnMouseRightClick();
    }
    
    public void DisplayCard()
    {
        if(WorldSystem.instance.deckDisplayManager.selectedCard == null)
        {
            WorldSystem.instance.tempWorldState = WorldSystem.instance.worldState;
            WorldSystem.instance.worldState = WorldState.Display;
            WorldSystem.instance.deckDisplayManager.previousPosition = transform.position;
            WorldSystem.instance.deckDisplayManager.selectedCard = this;
            WorldSystem.instance.deckDisplayManager.placeholderCard.GetComponent<Card>().cardData = WorldSystem.instance.deckDisplayManager.selectedCard.cardData;
            WorldSystem.instance.deckDisplayManager.placeholderCard.GetComponent<Card>().BindCardData();
            WorldSystem.instance.deckDisplayManager.backgroundPanel.SetActive(true);
            WorldSystem.instance.deckDisplayManager.clickableArea.SetActive(true);
            WorldSystem.instance.deckDisplayManager.scroller.GetComponent<ScrollRect>().enabled = false;
            transform.position = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0.1f);
        }
        else
        {
            ResetCardPosition();
        }
    }
    public void ResetCardPosition()
    {

        WorldSystem.instance.worldState = WorldSystem.instance.tempWorldState;
        WorldSystem.instance.deckDisplayManager.backgroundPanel.SetActive(false);
        WorldSystem.instance.deckDisplayManager.clickableArea.SetActive(false);
        WorldSystem.instance.deckDisplayManager.scroller.GetComponent<ScrollRect>().enabled = true;
        WorldSystem.instance.deckDisplayManager.selectedCard.transform.position = WorldSystem.instance.deckDisplayManager.previousPosition;
        WorldSystem.instance.deckDisplayManager.previousPosition = transform.position;
        WorldSystem.instance.deckDisplayManager.selectedCard = null;
    }
    public void ResetCardPositionNext()
    {
        WorldSystem.instance.deckDisplayManager.selectedCard.transform.position = WorldSystem.instance.deckDisplayManager.previousPosition;
        WorldSystem.instance.deckDisplayManager.previousPosition = Vector3.zero;
        WorldSystem.instance.deckDisplayManager.selectedCard = null;
    }

    public abstract void OnMouseClick();

    public abstract void OnMouseRightClick();


}

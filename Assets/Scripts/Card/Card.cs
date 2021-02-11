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
    public bool targetRequired;
    public void BindCardData()

    {
        nameText.text = cardData.name;

        targetRequired = cardData.targetRequired;

        artworkImage.sprite = cardData.artwork;

        costText.text = cardData.cost.ToString();

        descriptionText.text = "";

        List<CardEffect> allEffects = new List<CardEffect>();
        allEffects.Add(cardData.Damage);
        allEffects.Add(cardData.Block);
        allEffects.AddRange(cardData.SelfEffects);
        allEffects.AddRange(cardData.Effects);

        for (int i = 0; i < allEffects.Count; i++)
        {
            if (allEffects[i].Value == 0) continue;
            descriptionText.text += allEffects[i].Type.ToString() + ":" + allEffects[i].Value;
            if (allEffects[i].Times != 1) descriptionText.text += " " + allEffects[i].Times + " times.";
            if (i != allEffects.Count - 1) descriptionText.text += "\n";
        }
    }

    public virtual void OnMouseEnter()
    {
        return;
    }

    public virtual void OnMouseExit()
    {
        return;
    }
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
            WorldSystem.instance.worldStateManager.AddState(WorldState.Display, false);
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

        WorldSystem.instance.worldStateManager.RemoveState(false);
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

    public virtual void OnMouseClick()
    {
        Debug.Log("Clicky");
        return;
    }

    public virtual void OnMouseRightClick(bool allowDisplay = true)
    {
        return;
    }


}

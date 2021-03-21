using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public abstract class CardVisual : Card, IPointerClickHandler
{
    public Text nameText;
    public TMP_Text descriptionText;
    public Image artworkImage;

    public Text costText;
    public Text damageText;
    public Text blockText;
    public WorldState previousState;


    public void BindCardVisualData()
    {
        nameText.text = cardName;

        artworkImage.sprite = artwork;

        costText.text = cost.ToString();

        descriptionText.text = "";

        for (int i = 0; i < allEffects.Count; i++)
        {
            if (allEffects[i].Value == 0) continue;
            descriptionText.text += allEffects[i].Type.ToString() + EffectTypeToIconCode(allEffects[i].Type) + ":" + allEffects[i].Value;
            if (allEffects[i].Times != 1) descriptionText.text += " " + allEffects[i].Times + " times.";
            if (i != allEffects.Count - 1) descriptionText.text += "\n";
        }

        for(int i = 0; i < activities.Count; i++)
        {
            descriptionText.text += CardActivitySystem.instance.DescriptionByCardActivity(activities[i]);
        }
    }

    private string EffectTypeToIconCode(EffectType type)
    {
        if (type == EffectType.Damage)
            return " <sprite name=\"Attack\">";
        else if (type == EffectType.Block)
            return " <sprite name=\"Block\">";
        else
            return "";
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
    
    public virtual void OnPointerClick(PointerEventData eventData)
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
            WorldSystem.instance.deckDisplayManager.previousPosition = transform.position;
            WorldSystem.instance.deckDisplayManager.selectedCard = this;
            WorldSystem.instance.deckDisplayManager.placeholderCard.GetComponent<CardVisual>().cardData = WorldSystem.instance.deckDisplayManager.selectedCard.cardData;
            WorldSystem.instance.deckDisplayManager.placeholderCard.GetComponent<CardVisual>().BindCardData();
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

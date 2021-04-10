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

    public int calcDamage = -1;
    public int calcBlock = -1;

    readonly static string colorCodeGood = "#2e590c";
    readonly static string colorCodeBad = "#a16658";

    public void BindCardVisualData()
    {
        nameText.text = cardName;

        artworkImage.sprite = artwork;

        costText.text = cost.ToString();

        ResetDamageBlockCalc();
        RefreshDescriptionText();
    }

    public void ResetDamageBlockCalc()
    {

        calcDamage = Damage.Value;
        calcBlock = Block.Value;
    }

    public void RefreshDescriptionText()
    {
        string descText = "";
        
        //Special care for Damage on Block
        if(Damage.Value != 0)
        {
            descText += Damage.Type.ToString() + EffectTypeToIconCode(Damage.Type) + ":";
            if (calcDamage < Damage.Value)
                descText += "<color=" + colorCodeBad + ">" + calcDamage.ToString() + "</color>";
            else if (calcDamage > Damage.Value)
                descText += "<color=" + colorCodeGood + ">" + calcDamage.ToString() + "</color>";
            else
                descText += calcDamage.ToString();

            if (Damage.Times != 1) descText += " " + Damage.Times + " times.";
        }

        //Special care for Damage on Block
        if (Block.Value != 0)
        {
            descText += Block.Type.ToString() + EffectTypeToIconCode(Block.Type) + ":";
            if (calcBlock < Block.Value)
                descText += "<color=" + colorCodeBad + ">" + calcBlock.ToString() + "</color>";
            else if (calcBlock > Block.Value)
                descText += "<color=" + colorCodeGood + ">" + calcBlock.ToString() + "</color>";
            else
                descText += calcBlock.ToString();

            if (Block.Times != 1) descText += " " + Block.Times + " times.";
        }

        //Generall non-modifiable descs

        for (int i = 0; i < Effects.Count; i++)
        {
            if (Effects[i].Value == 0) continue;
            if (descText != "") descText += "\n";
            descText += Effects[i].Type.ToString() + EffectTypeToIconCode(Effects[i].Type) + ":" + Effects[i].Value;
            if (Effects[i].Times != 1) descText += " " + Effects[i].Times + " times.";
        }

        for (int i = 0; i < activities.Count; i++)
        {
            if (descText != "") descText += "\n";
            descText += CardActivitySystem.instance.DescriptionByCardActivity(activities[i]);
        }

        descriptionText.text = descText;
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
            WorldSystem.instance.deckDisplayManager.placeholderCard.GetComponent<CardVisual>().BindCardVisualData();
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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Text;

public abstract class CardVisual : Card, IPointerClickHandler, IToolTipable, IPointerEnterHandler, IPointerExitHandler
{
    public TMP_Text nameText;
    public TMP_Text descriptionText;
    public TMP_Text typeText;
    public Image artworkImage;
    public Image highlightSelected, highlightNormal, hightlightSpecial;
    public Image border;
    public Image rarityBorder;
    public Image energyColor;
    public RectTransform TooltipAnchor;
    public List<string> toolTipTextBits = new List<string>();

    public GameObject energyObjects;

    public TMP_Text costText;

    public List<ICardTextElement> cardTextElements = new List<ICardTextElement>();

    private string displayText = "";

    public bool isBroken = false;

    public void BindCardVisualData()
    {
        displayText = "";
        nameText.text = cardName;
        artworkImage.sprite = artwork;
        typeText.text = cardType.ToString();

        costText.text = cost.GetTextForCost();
        energyObjects.SetActive(visibleCost);

        ResetCardTextElementsList();

        for (int i = 0; i < singleFieldProperties.Count; i++)
        {
            if ((int)singleFieldProperties[i].type < 0)
                cardTextElements.Insert(0, singleFieldProperties[i]);
            else
                cardTextElements.Add(singleFieldProperties[i]);
        }

        SetBorderColor();
        RefreshDescriptionText();
        SetToolTips();
    }

    public void Mimic(CardVisual card)
    {
        base.Mimic(card);
        nameText.text = card.cardName;
        artworkImage.sprite = card.artwork;
        typeText.text = card.typeText.text;

        costText.text = card.costText.text;
        energyObjects.SetActive(card.visibleCost);

        ResetCardTextElementsList();

        for (int i = 0; i < singleFieldProperties.Count; i++)
        {
            if ((int)singleFieldProperties[i].type < 0)
                cardTextElements.Insert(0, singleFieldProperties[i]);
            else
                cardTextElements.Add(singleFieldProperties[i]);
        }

        SetBorderColor();
        RefreshDescriptionText(true);
        SetToolTips();
    }

    public void ResetCardTextElementsList()
    {
        cardTextElements.Clear();
        cardTextElements.AddRange(Attacks);
        cardTextElements.AddRange(Blocks);
        cardTextElements.AddRange(effectsOnDraw);
        cardTextElements.AddRange(activitiesOnDraw);
        cardTextElements.AddRange(effectsOnPlay);
        cardTextElements.AddRange(activitiesOnPlay);
    }

    public void RefreshDescriptionText(bool forceRebuild = false)
    {
        if (displayText.Equals("") || forceRebuild) DeriveDescriptionText();
        descriptionText.text = displayText;
    }

    public void DeriveDescriptionText()
    {
        StringBuilder textDeriver = new StringBuilder(300);

        for (int i = 0; i < cardTextElements.Count; i++)
        {
            string element = cardTextElements[i].GetElementText();
            if (element == null || element.Equals(string.Empty)) continue;
            if (textDeriver.Length != 0) textDeriver.AppendLine();
            if (effectsOnDraw.Contains(cardTextElements[i]) || activitiesOnDraw.Contains(cardTextElements[i])) textDeriver.Append("On Draw ");
            textDeriver.Append(cardTextElements[i].GetElementText());
        }

        displayText = textDeriver.ToString();
    }

    public static string EffectTypeToIconCode(EffectType type)
    {
        if (type == EffectType.Damage)
            return " <sprite name=\"Attack\">";
        else if (type == EffectType.Block)
            return " <sprite name=\"Block\">";
        else
            return "";
    }

    protected void SetToolTips()
    {
        toolTipTextBits.Clear();
        for (int i = 0; i < cardTextElements.Count; i++)
        {
            string element = cardTextElements[i].GetElementToolTip();
            if (element == null || element.Equals(string.Empty)) continue;
            toolTipTextBits.Add(element);
        }
    }

    void SetBorderColor()
    {
        border.color = Helpers.borderColors[classType];
        rarityBorder.color = Helpers.rarityBorderColors[rarity];
        energyColor.color = Helpers.borderColors[classType];
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

  

    public virtual void OnMouseClick()
    {
        WorldSystem.instance.toolTipManager.DisableTips();
        return;
    }

    public virtual void OnMouseRightClick(bool allowDisplay = true)
    {
        return;
    }

    public (List<string> tips, Vector3 worldPosition) GetTipInfo()
    {
        //activitiesOnPlay.ForEach(x => Debug.Log(x));
        return (toolTipTextBits, WorldSystem.instance.cameraManager.currentCamera.WorldToScreenPoint(TooltipAnchor.position));
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        return;
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        return;
    }
}

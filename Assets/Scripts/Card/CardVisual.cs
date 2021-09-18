using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Text;
using DG.Tweening;

public abstract class CardVisual : Card, IPointerClickHandler, IToolTipable, IPointerEnterHandler, IPointerExitHandler
{
    public TMP_Text nameText;
    public TMP_Text descriptionText;
    public TMP_Text typeText;
    public Image artworkImage;
    public Image highlightSelected, highlightNormal, hightlightSpecial;
    public Image border, cardBackground;
    public Image rarityBorder;
    public Image energyColor;
    public RectTransform TooltipAnchor;
    public List<string> toolTipTextBits = new List<string>();

    public GameObject energyObjects;

    public TMP_Text costText;

    public List<ICardTextElement> cardTextElements = new List<ICardTextElement>();

    private string displayText = "";
    public bool permanentUpgrade;

    public bool isBroken = false;
    Tween highlightTween;
    
    CardHighlightType _cardHighlightType;
    public CardHighlightType cardHighlightType
    {
        get => _cardHighlightType;
        set
        {
            if (value == _cardHighlightType) return;
            _cardHighlightType = value;
            highlightTween?.Kill();
            switch (_cardHighlightType)
            {
                case CardHighlightType.Selected:
                    StartHighlightAnimation(highlightSelected, Color.cyan, Color.blue, 0.2f);
                    break;
                case CardHighlightType.Playable:
                    StartHighlightAnimation(highlightNormal, Color.cyan, Color.blue, 0.5f);
                    break;
                case CardHighlightType.PlayableSpecial:
                    StartHighlightAnimation(highlightNormal, Color.red, Color.grey, 0.4f);
                    break;
                default:
                    break;
            }
        }
    }
    private void StartHighlightAnimation(Image highlight, Color color1, Color color2, float speed)
    {

        if (highlight == null) return;

        highlight.gameObject.SetActive(true);
        highlight.color = color1;
        highlightTween = highlight.DOColor(color2, speed).SetLoops(-1, LoopType.Yoyo).OnKill(() =>
        {
            highlight.gameObject.SetActive(false);
        });
    }

    public void UpdateCardVisual()
    {
        nameText.text += " +";
        cardName += " +";
        cardBackground.color = Helpers.upgradeCardColor; 
        ResetCardTextElementsList();
        RefreshDescriptionText(true);
        SetToolTips();
    }

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
        cardBackground.color = card.cardBackground.color;

        costText.text = card.costText.text;
        energyObjects.SetActive(card.visibleCost);

        ResetCardTextElementsList();
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

        for (int i = 0; i < singleFieldProperties.Count; i++)
        {
            if ((int)singleFieldProperties[i].type < 0)
                cardTextElements.Insert(0, singleFieldProperties[i]);
            else
                cardTextElements.Add(singleFieldProperties[i]);
        }

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
        //cardBackground.color = Color.white;
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

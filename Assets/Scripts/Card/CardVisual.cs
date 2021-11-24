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
    public RectTransform TooltipAnchor;
    public List<string> toolTipTextBits = new List<string>();

    public GameObject cardPrefab;
    public List<ICardTextElement> cardTextElements = new List<ICardTextElement>();

    private string displayText = "";
    public bool permanentUpgrade;

    public bool isBroken = false;
    Tween highlightTween;

    public Transform costParent;
    public GameObject templateCostUI;
    public Dictionary<EnergyType, CardCostDisplay> energyToCostUI = new Dictionary<EnergyType, CardCostDisplay>();
    public Dictionary<EnergyType, CardCostDisplay> energyToCostOptionalUI = new Dictionary<EnergyType, CardCostDisplay>();
    
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
        string newCardName = timesUpgraded > 0 ? string.Format("{0} +{1}", cardData.cardName, timesUpgraded)  : cardData.cardName;
        nameText.text = newCardName;
        cardName = newCardName;
        cardBackground.color = timesUpgraded > 0 ? Helpers.upgradeCardColor : Helpers.normalCardColor;

        CheckEnergyIcons();
        cost.UpdateTextsForCosts();

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

        CheckEnergyIcons();
        cost.UpdateTextsForCosts();

        ResetCardTextElementsList();
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
        
        foreach (EnergyType eType in cost.energyCosts.Keys)
        {
            RegisterCostUI(eType, true);
            energyToCostUI[eType].lblEnergy.text = card.energyToCostUI[eType].lblEnergy.text;
        }

        foreach (EnergyType eType in cost.energyCostsOptional.Keys)
        {
            RegisterCostUI(eType, false);
            energyToCostOptionalUI[eType].lblEnergy.text = card.energyToCostOptionalUI[eType].lblEnergy.text;
        }

        ResetCardTextElementsList();
        SetBorderColor();
        RefreshDescriptionText(true);
        SetToolTips();
    }

    private void RegisterCostUI(EnergyType eType, bool mandatory)
    {
        Dictionary<EnergyType, CardCostDisplay> dickie = mandatory ? energyToCostUI : energyToCostOptionalUI;
        dickie[eType] = Instantiate(templateCostUI, costParent).GetComponent<CardCostDisplay>();
        dickie[eType].SetType(eType);
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

    public void CheckEnergyIcons()
    {
        foreach (EnergyType eType in cost.energyCosts.Keys)
            if(!energyToCostUI.ContainsKey(eType))
                RegisterCostUI(eType, true);

        foreach (EnergyType eType in cost.energyCostsOptional.Keys)
            if (!energyToCostOptionalUI.ContainsKey(eType))
                RegisterCostUI(eType, false);
    }

    public static CardVisual Factory(CardVisual cardVisual, Transform parent) => Factory(cardVisual.cardData, parent, cardVisual.cardModifiers);

    public static CardVisual Factory(CardData data, Transform parent, List<CardFunctionalityData> appliedUpgrades = null)
    {
        CardVisual card = Instantiate(WorldSystem.instance.characterManager.cardPrefab, parent).GetComponent<CardVisual>();
        card.cardData = data;
        card.BindCardData();
        card.BindCardVisualData();

        if(appliedUpgrades != null)
            foreach (CardFunctionalityData mod in appliedUpgrades)
                card.AddModifierToCard(mod);

        return card;
    }

    public void Clone(CardVisual card, List<CardFunctionalityData> explicitMods = null)
    {
        Reset();
        cardData = card.cardData;
        BindCardData();
        BindCardVisualData();

        List<CardFunctionalityData> mods = new List<CardFunctionalityData>(card.cardModifiers);
        if (explicitMods != null) mods.AddRange(explicitMods);

        foreach (CardFunctionalityData mod in mods)
            AddModifierToCard(mod);
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
            if (effectsOnDraw.Contains(cardTextElements[i]) || activitiesOnDraw.Contains(cardTextElements[i])) textDeriver.Append("On Draw: ");
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
        if (timesUpgraded < 1) cardBackground.color = new Color(0.7f, 0.7f, 0.7f);
        border.color = Helpers.borderColors[classType];
        rarityBorder.color = Helpers.rarityBorderColors[rarity];
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

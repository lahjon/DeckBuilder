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
    internal List<string> manualToolTips = new List<string>();
    static float width = 153;

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

    public void UpdateAfterModifier()
    {
        string newCardName = timesUpgraded > 0 ? string.Format("{0} +{1}", cardData.cardName, timesUpgraded)  : cardData.cardName;
        nameText.text = newCardName;
        cardName = newCardName;

        CheckEnergyIcons();
        cost.UpdateTextsForCosts();

        RunVisualSetters();
    }

    private void SetInnerColor()
    {
        Color col;
        if (!modifiedTypes.Any())
            col = new Color(0.7f, 0.7f, 0.7f);
        else if (modifiedTypes.Contains(ModifierType.Cursed))
            col = Helpers.cursedCardColor;
        else
            col = Helpers.upgradeCardColor;

        cardBackground.color = col;
    }

    public void BindCardVisualData()
    {
        displayText = "";
        nameText.text = cardName;
        artworkImage.sprite = artwork;
        typeText.text = cardType.ToString();

        CheckEnergyIcons();
        cost.UpdateTextsForCosts();

        RunVisualSetters();
    }

    private void RunVisualSetters()
    {
        ResetCardTextElementsList();
        SetBorderColor();
        SetInnerColor();
        RefreshDescriptionText(true);
        SetToolTips();
    }


    public void Mimic(CardVisual card)
    {
        base.Mimic(card);
        nameText.text = card.cardName;
        artworkImage.sprite = card.artwork;
        typeText.text = card.typeText.text;

        card.manualToolTips.ForEach(t => SetManualToolTip(t));

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

        RunVisualSetters();
    }

    public void SetManualToolTip(string tip)
    {
        if (manualToolTips.Contains(tip)) return;
        manualToolTips.Add(tip);
        toolTipTextBits.Insert(0, tip);
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
        card.manualToolTips.ForEach(t => SetManualToolTip(t));
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

    public static string EffectTypeToIconCode(StatusEffectType type)
    {
        if (type == StatusEffectType.Damage)
            return " <sprite name=\"Attack\">";
        else if (type == StatusEffectType.Block)
            return " <sprite name=\"Block\">";
        else
            return "";
    }

    protected void SetToolTips()
    {
        toolTipTextBits.Clear();
        toolTipTextBits.AddRange(manualToolTips);
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

    public virtual void OnMouseRightClick()
    {
        return;
    }

    public (List<string> tips, Vector3 worldPosition, float offset) GetTipInfo()
    {
        return (toolTipTextBits, WorldSystem.instance.cameraManager.currentCamera.WorldToScreenPoint(TooltipAnchor.position), width);
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

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

    readonly static string strBlockCode = "_BLOCKINFO_";
    readonly static string strDamageCode = "_DAMAGEINFO_";
    readonly static string colorCodeGood = "#2e590c";
    readonly static string colorCodeBad = "#a16658";


    private int _displayDamage = -1;
    private int _displayBlock = -1;
    private int _displayCost;

    public int displayDamage
    {
        get => _displayDamage;
        set
        {
            _displayDamage = value;
            RefreshBlockAndDamageParts();
        }
    }
    public int displayBlock
    {
        get => _displayBlock;
        set
        {
            _displayBlock = value;
            RefreshBlockAndDamageParts();
        }
    }
    public int displayCost
    {
        get { return _displayCost; }
        set
        {
            _displayCost = value;
            costText.text = ValueColorWrapper(cost, _displayCost, true);
        }
    }

    private string derivedText = "";
    private string displayText;

    public bool isBroken = false;

    public void BindCardVisualData()
    {
        derivedText = "";
        displayText = "";
        nameText.text = cardName;
        artworkImage.sprite = artwork;
        typeText.text = cardType.ToString();

        displayCost = cost;

        energyObjects.SetActive(visibleCost);

        SetBorderColor();
        ResetDamageBlockCalc();
        RefreshDescriptionText();
        SetToolTips();
    }

    public void Mimic(CardVisual card)
    {
        base.Mimic(card);
        nameText.text = card.cardName;
        artworkImage.sprite = card.artwork;
        typeText.text = cardType.ToString();

        displayCost = card.displayCost;
        energyObjects.SetActive(card.visibleCost);

        SetBorderColor();
        ResetDamageBlockCalc();
        RefreshDescriptionText(true);
        SetToolTips();
    }

    public void ResetDamageBlockCalc()
    {
        _displayDamage = Damage.Value;
        _displayBlock = Block.Value;
    }

    public void RefreshDescriptionText(bool forceRebuild = false)
    {
        if (derivedText.Equals("") || forceRebuild) DeriveDescriptionText();

        RefreshBlockAndDamageParts();
        descriptionText.text = displayText;
    }

    public void RefreshBlockAndDamageParts()
    {
        StringBuilder descText;
        displayText = derivedText;

        //Special care for Damage and Block
        if (Block.Value != 0)
        {
            descText = new StringBuilder(100);
            descText.Append(Block.Type.ToString() + EffectTypeToIconCode(Block.Type) + " ");
            descText.Append(ValueColorWrapper(Block.Value, displayBlock));
            if (Block.Times != 1) descText.Append(" " + Block.Times + " times ");
            if (Block.Target != CardTargetType.Self) descText.Append(" " + Block.Target.ToString());
            displayText = displayText.Replace(strBlockCode, descText.ToString());
        }
        //Special care for Damage and Block
        if (Damage.Value != 0)
        {
            descText = new StringBuilder(100);
            if (Damage.Value != 0) descText.AppendLine();
            descText.Append(Damage.Type.ToString() + EffectTypeToIconCode(Damage.Type) + " ");
            descText.Append(ValueColorWrapper(Damage.Value, displayDamage));
            if (Damage.Times != 1) descText.Append(" " + Damage.Times + " times ");
            if (Damage.Target != CardTargetType.EnemySingle) descText.Append(" " + Damage.Target.ToString());
            displayText = displayText.Replace(strDamageCode, descText.ToString());
        }
    }

    public void DeriveDescriptionText()
    {
        StringBuilder descText = new StringBuilder(300);

        if (immediate) descText.AppendLine("<b>Immediate</b>");
        if (unplayable) descText.AppendLine("<b>Unplayable</b>");
        if (unstable) descText.AppendLine("<b>Unstable</b>");


        //Special care for Damage and Block
        if (Block.Value != 0) descText.Append(strBlockCode);
        //Special care for Damage and Block
        if (Damage.Value != 0) descText.Append(strDamageCode);

        //On draw non-modifiable descs

        for (int i = 0; i < effectsOnDraw.Count; i++)
        {
            if (effectsOnDraw[i].Value == 0) continue;
            if (descText.Length != 0) descText.AppendLine();
            descText.Append("On Draw: " + EffectInfoToString(effectsOnDraw[i]));
        }

        for (int i = 0; i < activitiesOnDraw.Count; i++)
        {
            if (descText.Length != 0) descText.AppendLine();
            descText.Append("On Draw: " + CardActivitySystem.instance.DescriptionByCardActivity(activitiesOnDraw[i]));
        }

        //Generall non-modifiable descs
        for (int i = 0; i < effectsOnPlay.Count; i++)
        {
            if (effectsOnPlay[i].Value == 0) continue;
            if (descText.Length != 0) descText.AppendLine();
            descText.Append(EffectInfoToString(effectsOnPlay[i]));
        }

        for (int i = 0; i < activitiesOnPlay.Count; i++)
        {
            if (descText.Length != 0) descText.AppendLine();
            descText.Append(CardActivitySystem.instance.DescriptionByCardActivity(activitiesOnPlay[i]));
        }
        if (exhaust)
        {
            if (descText.Length != 0) descText.AppendLine();
            descText.Append("<b>Exhaust</b>");
        }

        derivedText = descText.ToString();
    }

    public string ValueColorWrapper(int originalVal, int currentVal, bool inverse = false)
    {
        if ((!inverse && currentVal < originalVal) || (inverse && currentVal > originalVal))
            return "<color=" + colorCodeBad + ">" + currentVal.ToString() + "</color>";
        else if(originalVal == currentVal)
            return currentVal.ToString();
        else
            return "<color=" + colorCodeGood + ">" + currentVal.ToString() + "</color>";
    }

    private string EffectInfoToString(CardEffectInfo effectInfo)
    {
        string retString = "";
        if (effectInfo.ConditionStruct.type != ConditionType.None) retString += "If " + effectInfo.ConditionStruct.type.ToString() + " " + effectInfo.ConditionStruct.strParameter
                + (effectInfo.ConditionStruct.numValue == 0 ? "" : effectInfo.ConditionStruct.numValue.ToString()) 
                 + " then: ";
        retString += effectInfo.Type.ToString() + EffectTypeToIconCode(effectInfo.Type) + " " + effectInfo.Value;
        if (effectInfo.Times != 1) retString += " " + effectInfo.Times + " times";
        if (effectInfo.Target != CardTargetType.EnemySingle) retString += " " + effectInfo.Target.ToString();

        return retString;
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

    protected void SetToolTips()
    {
        toolTipTextBits.Clear();
        if (immediate) toolTipTextBits.Add("<b>Immediate</b>\nThis card will play itself when you draw it.");
        if (unplayable) toolTipTextBits.Add("<b>Unplayable</b>\nThis card can not be played.");
        if (unstable) toolTipTextBits.Add("<b>Unstable</b>\nThis card will exhaust if it is still in hand at end of turn.");
        allEffects.ForEach(x => { if (x.Type != EffectType.Damage && !(x.Type == EffectType.Block && x.Value == 0)) toolTipTextBits.Add(x.Type.GetDescription()); });
        activitiesOnPlay.ForEach(x => toolTipTextBits.Add(CardActivitySystem.instance.ToolTipByCardActivity(x)));
        if (exhaust) toolTipTextBits.Add("<b>Exhaust</b>\nThis card disappears when used.");
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

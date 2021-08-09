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
    public Image artworkImage;
    public GameObject highlight;
    public Image border;
    public Image rarityBorder;
    public RectTransform TooltipAnchor; 
    public List<string> toolTipTextBits = new List<string>();

    public TMP_Text costText;
    public WorldState previousState;

    public int displayDamage = -1;
    public int displayBlock = -1;

    private int _displayCost;

    const string strBlockCode = "_BLOCKINFO_";
    const string strDamageCode = "_DAMAGEINFO_";

    public int displayCost
    {
        get { return _displayCost; }
        set
        {
            _displayCost = value;
            if (visibleCost)
                costText.text = ValueColorWrapper(cost, _displayCost, true);
            else
                costText.gameObject.SetActive(false);
        }
    }

    readonly static string colorCodeGood = "#2e590c";
    readonly static string colorCodeBad = "#a16658";

    private string derivedText = "";
    private string displayText;

    public bool isBroken = false;

    public void BindCardVisualData()
    {
        nameText.text = cardName;
        artworkImage.sprite = artwork;

        displayCost = cost;

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

        displayCost = card.displayCost;

        SetBorderColor();
        ResetDamageBlockCalc();
        RefreshDescriptionText();
    }

    public void ResetDamageBlockCalc()
    {

        displayDamage = Damage.Value;
        displayBlock = Block.Value;
    }

    public void RefreshDescriptionText()
    {
        if (derivedText.Equals("")) DeriveDescriptionText();
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
            if (Block.Value != 0) descText.AppendLine();
            descText.Append(Damage.Type.ToString() + EffectTypeToIconCode(Damage.Type) + " ");
            descText.Append(ValueColorWrapper(Damage.Value, displayDamage));
            if (Damage.Times != 1) descText.Append(" " + Damage.Times + " times ");
            if (Damage.Target != CardTargetType.EnemySingle) descText.Append(" " + Damage.Target.ToString());
            displayText = displayText.Replace(strDamageCode, descText.ToString());
        }

        descriptionText.text = displayText;
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
            descText.Append("On Draw: " + EffectInfoToString(effectsOnPlay[i]));
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
        if (effectInfo.ConditionStruct.type != ConditionType.None) retString += "If " + effectInfo.ConditionStruct.type.ToString() + " " + effectInfo.ConditionStruct.value
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
        if (unstable) toolTipTextBits.Add("<b>Unstable</b>\nThis card will exhaust if it is still in hand at end of turn.");
        if (unplayable) toolTipTextBits.Add("<b>Unplayable</b>\nThis card can not be played.");
        allEffects.ForEach(x => { if (x.Type != EffectType.Damage && !(x.Type == EffectType.Block && x.Value == 0)) toolTipTextBits.Add(x.Type.GetDescription()); });
        activitiesOnPlay.ForEach(x => toolTipTextBits.Add(CardActivitySystem.instance.ToolTipByCardActivity(x)));
        if (exhaust) toolTipTextBits.Add("<b>Exhaust</b>\nThis disappears when used.");
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
        //Debug.Log("Click");
        if (eventData.button == PointerEventData.InputButton.Left)
            OnMouseClick();
        else if (eventData.button == PointerEventData.InputButton.Right)
            OnMouseRightClick();
    }

    
    // public void DisplayCard()
    // {
    // if(WorldSystem.instance.deckDisplayManager.selectedCard == null)
    // {
    //     WorldSystem.instance.deckDisplayManager.previousPosition = transform.position;
    //     WorldSystem.instance.deckDisplayManager.selectedCard = this;
    //     WorldSystem.instance.deckDisplayManager.placeholderCard.GetComponent<CardVisual>().cardData = WorldSystem.instance.deckDisplayManager.selectedCard.cardData;
    //     WorldSystem.instance.deckDisplayManager.placeholderCard.GetComponent<CardVisual>().BindCardData();
    //     WorldSystem.instance.deckDisplayManager.placeholderCard.GetComponent<CardVisual>().BindCardVisualData();
    //     WorldSystem.instance.deckDisplayManager.backgroundPanel.SetActive(true);
    //     WorldSystem.instance.deckDisplayManager.clickableArea.SetActive(true);
    //     WorldSystem.instance.deckDisplayManager.scroller.GetComponent<ScrollRect>().enabled = false;
    //     transform.position = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0.1f);
    // }
    // else
    // {
    //     ResetCardPosition();
    // }
    // }
    // public void ResetCardPosition()
    // {
    //     WorldSystem.instance.deckDisplayManager.backgroundPanel.SetActive(false);
    //     WorldSystem.instance.deckDisplayManager.clickableArea.SetActive(false);
    //     WorldSystem.instance.deckDisplayManager.scroller.GetComponent<ScrollRect>().enabled = true;
    //     WorldSystem.instance.deckDisplayManager.selectedCard.transform.position = WorldSystem.instance.deckDisplayManager.previousPosition;
    //     WorldSystem.instance.deckDisplayManager.previousPosition = transform.position;
    //     WorldSystem.instance.deckDisplayManager.selectedCard = null;
    // }
    // public void ResetCardPositionNext()
    // {
    //     WorldSystem.instance.deckDisplayManager.selectedCard.transform.position = WorldSystem.instance.deckDisplayManager.previousPosition;
    //     WorldSystem.instance.deckDisplayManager.previousPosition = Vector3.zero;
    //     WorldSystem.instance.deckDisplayManager.selectedCard = null;
    // }

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
        activitiesOnPlay.ForEach(x => Debug.Log(x));
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

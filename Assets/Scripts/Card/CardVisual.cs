using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Linq;

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

    public bool isBroken = false;

    public void BindCardVisualData()
    {
        nameText.text = cardName;
        artworkImage.sprite = artwork;

        displayCost = cost;

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
        string descText = "";

        if (unplayable) descText += "<b>Unplayable</b>\n";
        if(unstable)    descText += "<b>Unstable</b>\n";
        
        //Special care for Damage on Block
        if(Damage.Value != 0)
        {
            descText += Damage.Type.ToString() + EffectTypeToIconCode(Damage.Type) + ":";
            descText += ValueColorWrapper(Damage.Value, displayDamage);
            if (Damage.Times != 1) descText += " " + Damage.Times + " times";
            if (Damage.Target != CardTargetType.EnemySingle) descText += " " + Damage.Target.ToString();
        }

        //Special care for Damage on Block
        if (Block.Value != 0)
        {
            descText += Block.Type.ToString() + EffectTypeToIconCode(Block.Type) + ":";
            descText += ValueColorWrapper(Block.Value, displayBlock);
            if (Block.Times != 1) descText += " " + Block.Times + " times.";
            if (Block.Target != CardTargetType.Self) descText += " " + Block.Target.ToString();
        }

        //On draw non-modifiable descs

        for (int i = 0; i < effectsOnDraw.Count; i++)
        {
            if (effectsOnDraw[i].Value == 0) continue;
            if (descText != "") descText += "\n";
            descText += "On Draw: " + effectsOnDraw[i].Type.ToString() + EffectTypeToIconCode(effectsOnDraw[i].Type) + ":" + effectsOnDraw[i].Value;
            if (effectsOnDraw[i].Times != 1) descText += " " + effectsOnDraw[i].Times + " times.";
            if (effectsOnDraw[i].Target != CardTargetType.EnemySingle) descText += " " + effectsOnDraw[i].Target.ToString();
        }

        for (int i = 0; i < activitiesOnDraw.Count; i++)
        {
            if (descText != "") descText += "\n";
            descText += "On Draw: " + CardActivitySystem.instance.DescriptionByCardActivity(activitiesOnDraw[i]);
        }

        //Generall non-modifiable descs

        for (int i = 0; i < effectsOnPlay.Count; i++)
        {
            if (effectsOnPlay[i].Value == 0) continue;
            if (descText != "") descText += "\n";
            descText += effectsOnPlay[i].Type.ToString() + EffectTypeToIconCode(effectsOnPlay[i].Type) + ":" + effectsOnPlay[i].Value;
            if (effectsOnPlay[i].Times != 1) descText += " " + effectsOnPlay[i].Times + " times.";
            if (effectsOnPlay[i].Target != CardTargetType.EnemySingle) descText += " " + effectsOnPlay[i].Target.ToString();
        }

        for (int i = 0; i < activitiesOnPlay.Count; i++)
        {
            if (descText != "") descText += "\n";
            descText += CardActivitySystem.instance.DescriptionByCardActivity(activitiesOnPlay[i]);
        }

        if (exhaust)
        {
            if (descText != "") descText += "\n";
            descText += "<b>Exhaust</b>";
        }

        descriptionText.text = descText;
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

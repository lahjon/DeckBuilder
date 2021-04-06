using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class CardCombat : CardVisual
{
    [HideInInspector]
    public CombatController combatController;
    public RectTransform cardPanel;
    public AnimationCurve transitionCurveDraw;
    public AnimationCurve transitionCurveReturn;

    public Animator animator;

    public TooltipController tooltipController;

    private bool _selected = false;
    private bool _selectable = false;
    private bool _mouseReact = false;

    public float fanDegreeCurrent;
    public float fanDegreeTarget;

    public bool MouseReact
    {
        get
        {
            return _mouseReact;
        }
        set
        {
            _mouseReact = value;
            animator.SetBool("AllowMouseOver", value);
        }
    }

    public bool selected 
    {
        get
        {
            return _selected;
        }
        set
        {
            _selected = value;
            if(_selected == true)
            {
                transform.SetAsLastSibling();
            }
            animator.SetBool("Selected", value);          
        }
    }

    public bool selectable
    {
        get
        {
            return _selectable;
        }
        set
        {
            _selectable = value;
            animator.SetBool("Selectable", value);
        }
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("NeedTarget",targetRequired);
    }

    public static CardCombat CreateCardCombatFromData(CardData cardData)
    {
        CombatController combatController = WorldSystem.instance.combatManager.combatController;
        GameObject CardObject = Instantiate(combatController.TemplateCard, new Vector3(-10000, -10000, -10000), Quaternion.Euler(0, 0, 0)) as GameObject;
        CardObject.transform.SetParent(combatController.cardPanel, false);
        CardObject.transform.localScale = Vector3.one;
        CardCombat card = CardObject.GetComponent<CardCombat>();
        card.cardData = cardData;
        card.cardPanel = combatController.cardPanel.GetComponent<RectTransform>();
        card.BindCardData();
        card.BindCardVisualData();

        card.combatController = combatController;
        card.owner = combatController.Hero;
        card.GetComponent<BezierFollow>().route = combatController.bezierPath.transform;
        combatController.createdCards.Add(card);

        card.allEffects.ForEach(x => { if (x.Type != EffectType.Damage && !(x.Type == EffectType.Block && x.Value == 0)) card.tooltipController.AddTipText(x.Type.GetDescription()); });
        card.activities.ForEach( x => card.tooltipController.AddTipText(CardActivitySystem.instance.ToolTipByCardActivity(x)));

        return card;
    }

    public static CardCombat CreateCardCombined(CardCombat a, CardCombat b)
    {
        CombatController combatController = WorldSystem.instance.combatManager.combatController;
        GameObject CardObject = Instantiate(combatController.TemplateCard, new Vector3(-10000, -10000, -10000), Quaternion.Euler(0, 0, 0)) as GameObject;
        CardObject.transform.SetParent(combatController.cardPanel, false);
        CardObject.transform.localScale = Vector3.one;
        CardCombat card = CardObject.GetComponent<CardCombat>();
        card.combatController = combatController;
        card.GetComponent<BezierFollow>().route = combatController.bezierPath.transform;
        SpliceCards(card, a, b);
        card.BindCardVisualData();
        card.owner = combatController.ActiveActor;
        card.allEffects.ForEach(x => { if (x.Type != EffectType.Damage && !(x.Type == EffectType.Block && x.Value == 0)) card.tooltipController.AddTipText(x.Type.GetDescription()); });
        card.activities.ForEach(x => card.tooltipController.AddTipText(CardActivitySystem.instance.ToolTipByCardActivity(x)));

        return card;
    }


    public override void OnMouseEnter()
    {
        animator.SetBool("MouseIsOver", true);
    }

    public override void OnMouseExit()
    {
        animator.SetBool("MouseIsOver", false);
    }

    public void SelectCard()
    {
        if (combatController.ActiveCard != null) combatController.ActiveCard.DeselectCard();
        selected = true;
        combatController.ActiveCard = this;
        animator.SetBool("Selected", true);
    }
    public override void  OnMouseRightClick(bool allowDisplay = true)
    {
        Debug.Log("OnMouseRighclick called");
        if (combatController.ActiveCard == this)
        {
            DeselectCard();
            Debug.Log("Deselect");
        }
        else if(!selected && allowDisplay && combatController.ActiveCard == null)
        {
            DisplayCard();
            Debug.Log("Display");
        }
    }

    public override void OnMouseClick()
    {
        Debug.Log("Cardclicked: " + cardName);
        if(combatController.ActiveCard == this)
            combatController.SelectedCardTriggered();
        else if(combatController.CardisSelectable(this,false))
            SelectCard();  
    }

    public void DeselectCard()
    {
        selected = false;
        combatController.CancelCardSelection();
    }

    public override void ResetScale()
    {
        throw new System.NotImplementedException();
    }

    public Vector3 AngleLerp(Vector3 StartAngle, Vector3 FinishAngle, float t)
    {
        float xLerp = Mathf.LerpAngle(StartAngle.x, FinishAngle.x, t);
        float yLerp = Mathf.LerpAngle(StartAngle.y, FinishAngle.y, t);
        float zLerp = Mathf.LerpAngle(StartAngle.z, FinishAngle.z, t);
        return new Vector3(xLerp, yLerp, zLerp);
    }
}
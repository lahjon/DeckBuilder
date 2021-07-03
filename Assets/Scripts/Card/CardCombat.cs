using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class CardCombat : CardVisual
{
    public RectTransform cardPanel;
    public AnimationCurve transitionCurveDraw;
    public AnimationCurve transitionCurveReturn;

    public Animator animator;

    private bool _selected = false;
    private bool _selectable = false;
    private bool _mouseReact = false;

    public float fanDegreeCurrent;
    public float fanDegreeTarget;

    public BoxCollider2D boxCollider2D;
    public Image image;
    

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
        animator.SetBool("NeedTarget",targetRequired);
        boxCollider2D = GetComponent<BoxCollider2D>();
        image = GetComponent<Image>();
    }

    public static CardCombat CreateCardCombatFromData(CardData cardData)
    {
        GameObject CardObject = Instantiate(CombatSystem.instance.TemplateCard, new Vector3(-10000, -10000, -10000), Quaternion.Euler(0, 0, 0)) as GameObject;
        CardObject.transform.SetParent(CombatSystem.instance.cardPanel, false);
        CardObject.transform.localScale = Vector3.one;
        CardCombat card = CardObject.GetComponent<CardCombat>();
        card.cardData = cardData;
        card.cardPanel = CombatSystem.instance.cardPanel.GetComponent<RectTransform>();
        card.BindCardData();
        card.BindCardVisualData();

        card.owner = CombatSystem.instance.Hero;
        card.GetComponent<BezierFollow>().routeDiscard = CombatSystem.instance.pathDiscard.transform;
        card.GetComponent<BezierFollow>().routeDeck = CombatSystem.instance.pathDeck.transform;
        CombatSystem.instance.createdCards.Add(card);

        card.animator = card.GetComponent<Animator>();
        card.SetToolTips();

        return card;
    }

    public static CardCombat CreateCardCombined(CardCombat a, CardCombat b)
    {
        GameObject CardObject = Instantiate(CombatSystem.instance.TemplateCard, new Vector3(-10000, -10000, -10000), Quaternion.Euler(0, 0, 0)) as GameObject;
        CardObject.transform.SetParent(CombatSystem.instance.cardPanel, false);
        CardObject.transform.localScale = Vector3.one;
        CardCombat card = CardObject.GetComponent<CardCombat>();
        card.GetComponent<BezierFollow>().routeDiscard = CombatSystem.instance.pathDiscard.transform;
        card.GetComponent<BezierFollow>().routeDeck = CombatSystem.instance.pathDeck.transform;
        SpliceCards(card, a, b);
        card.classType = a.classType;
        card.BindCardVisualData();
        card.owner = CombatSystem.instance.ActiveActor;
        card.animator = card.GetComponent<Animator>();
        card.SetToolTips();
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
        if (CombatSystem.instance.ActiveCard != null) CombatSystem.instance.ActiveCard.DeselectCard();
        selected = true;
        CombatSystem.instance.ActiveCard = this;
        animator.SetBool("Selected", true);
    }
    public override void  OnMouseRightClick(bool allowDisplay = true)
    {
        Debug.Log("OnMouseRighclick called");
        if (CombatSystem.instance.ActiveCard == this)
        {
            DeselectCard();
            Debug.Log("Deselect");
        }
        else if(!selected && allowDisplay && CombatSystem.instance.ActiveCard == null)
        {
            WorldSystem.instance.deckDisplayManager.DisplayCard(this);
            Debug.Log("Display");
        }
    }

    public override void OnMouseClick()
    {
        base.OnMouseClick();
        if(CombatSystem.instance.ActiveCard == this)
            CombatSystem.instance.SelectedCardTriggered();
        else if(CombatSystem.instance.CardisSelectable(this,false))
            SelectCard();  
    }

    public void DeselectCard()
    {
        selected = false;
        CombatSystem.instance.CancelCardSelection();
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
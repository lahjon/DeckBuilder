using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;


public class CardCombat : CardVisual, IEventSubscriber
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

    public Condition playCondition = new Condition();

    public delegate void DamageRecalcEvent();
    public event DamageRecalcEvent OnDamageRecalcEvent;

    public void DamageNeedsRecalc()
    {
        OnDamageRecalcEvent?.Invoke();
    }

    [HideInInspector]
    public bool targetRequired
    {
        get => Attacks.Any(x => x.Target == CardTargetType.EnemySingle) || effectsOnPlay.Any(x => x.Target == CardTargetType.EnemySingle);
    }


    public bool MouseReact
    {
        get => _mouseReact;
        set
        {
            _mouseReact = value;
            animator.SetBool("AllowMouseOver", value);
        }
    }

    public bool selected 
    {
        get => _selected;
        set
        {
            _selected = value;
            if(_selected == true)
            {
                transform.SetAsLastSibling();
                cardHighlightType = CardHighlightType.Selected;
            }
            else
            {
                EvaluateHighlightNotSelected();
            }
            animator.SetBool("Selected", value);          
        }
    }

    public bool selectable
    {
        get => _selectable && cost.Payable && playCondition;
        set
        {
            _selectable = value;
            animator.SetBool("Selectable", value);
            EvaluateHighlightNotSelected();
        }
    }

    public bool isPlayable => playCondition.value && cost.Payable;

    public void EvaluateHighlightNotSelected()
    {
        if (selected) return;

        if (!selectable)
            cardHighlightType = CardHighlightType.None;
        else
        {
            if (isPlayable)
            {
                if(!registeredConditions.Any() || registeredConditions.Any(x => !x.value))
                    cardHighlightType = CardHighlightType.Playable;
                else
                    cardHighlightType = CardHighlightType.PlayableSpecial;
            }
            else
                cardHighlightType = CardHighlightType.None;
        }
    }

    void Start()
    {
        animator.SetBool("NeedTarget",targetRequired);
        boxCollider2D = GetComponent<BoxCollider2D>();
        image = GetComponent<Image>();
    }

    public static CardCombat Factory(CardVisual cardVisual)
    {
        CardCombat card = Factory(cardVisual.cardData);
        for (int i = 0; i < cardVisual.cardModifiers.Count; i++)
            card.AddModifierToCard(cardVisual.cardModifiers[i]);

        return card;
    }

    public static CardCombat Factory(CardData data)
    {
        GameObject CardObject = Instantiate(CombatSystem.instance.TemplateCard, new Vector3(-10000, -10000, -10000), Quaternion.Euler(0, 0, 0)) as GameObject;
        CardObject.transform.SetParent(CombatSystem.instance.cardPanel, false);
        CardObject.transform.localScale = Vector3.one;
        CardCombat card = CardObject.GetComponent<CardCombat>();
        card.cardPanel = CombatSystem.instance.cardPanel.GetComponent<RectTransform>();
        card.cardData = data;
        card.BindCardData();
        card.BindCardVisualData();
        card.owner = CombatSystem.instance.Hero;
        card.GetComponent<BezierFollow>().routeDiscard = CombatSystem.instance.pathDiscard.transform;
        card.GetComponent<BezierFollow>().routeDeck = CombatSystem.instance.pathDeck.transform;
        CombatSystem.instance.createdCards.Add(card);

        if (card.singleFieldProperties.Any(s => s == CardSingleFieldPropertyType.Unplayable)) card.playCondition = new Condition() { value = false };

        card.Subscribe();
        card.RefreshConditions();
        return card;
    }

    public static CardCombat Combine(CardCombat a, CardCombat b)
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

    public override void OnMouseRightClick(bool allowDisplay = true)
    {
        Debug.Log("OnMouseRighclick called");
        if (selected)
        {
            CombatSystem.instance.CancelCardSelection();
            Debug.Log("Deselect");
        }
        else if(allowDisplay && CombatSystem.instance.ActiveCard == null)
        {
            WorldSystem.instance.deckDisplayManager.DisplayCard(this);
            Debug.Log("Display");
        }
    }

    public override void OnMouseClick()
    {
        base.OnMouseClick();
        CombatSystem.instance.CardClicked(this);
    }

    public override void ResetScale()
    {
        throw new System.NotImplementedException();
    }

    public void RefreshConditions()
    {
        foreach (Condition c in registeredConditions)
            c.OnEventNotification();

        playCondition.OnEventNotification();
    }

    public void Unsubscribe()
    {
        foreach (IEventSubscriber e in registeredSubscribers)
            e.Unsubscribe();

        playCondition.Unsubscribe();
        EventManager.OnEnergyChangedEvent -= EvaluateHighlightNotSelected;
    }

    public void Subscribe()
    {
        foreach (IEventSubscriber e in registeredSubscribers)
            e.Subscribe();

        playCondition.Subscribe();
        EventManager.OnEnergyChangedEvent += EvaluateHighlightNotSelected;
    }
}
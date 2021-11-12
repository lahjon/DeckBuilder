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

    public Condition playCondition = new Condition();

    public delegate void DamageRecalcEvent();
    public event DamageRecalcEvent OnDamageRecalcEvent;

    public CardCollider cardCollider;

    public void DamageNeedsRecalc()
    {
        OnDamageRecalcEvent?.Invoke();
    }

    [HideInInspector]
    public bool targetRequired
    {
        get => true;
        //get => Attacks.Any(x => x.Target == CardTargetType.EnemySingle) || effectsOnPlay.Any(x => x.Target == CardTargetType.EnemySingle);
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
                cardCollider.transform.SetAsLastSibling();
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
        card.cardCollider = CardColliderManager.instance.GetCollider();
        card.cardCollider.SetOwner(card);

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
        SpliceCards(card, a, b);
        card.classType = a.classType;
        card.BindCardVisualData();
        card.owner = CombatSystem.instance.ActiveActor;
        card.cardCollider = CardColliderManager.instance.GetCollider();
        card.cardCollider.SetOwner(card);
        return card;
    }
    public void StartBezierAnimation(int path)
    {
        switch (path)
        {
            case 0: // deck
                StartCoroutine(GoByTheRoute(CombatSystem.instance.bezierController.routeDeck, path));
                break;
            case 1: // discard
                StartCoroutine(GoByTheRoute(CombatSystem.instance.bezierController.routeDiscard, path));
                break;
            case 2: // hero
                StartCoroutine(GoByTheRoute(CombatSystem.instance.bezierController.routeSelf, path));
                break;
            default:
                break;
        }
    }

    IEnumerator GoByTheRoute(Vector3[] p, int path)
    {
        Vector3 startingAngles = transform.localEulerAngles;
        Vector3 endAngle = Vector3.zero;
        Vector3 p1 = transform.position;
        Vector3 objectPosition;
        float endScale = 0.7f;
        float scale;
        float tParam = 0;
        float speedModifier = 1;

        while(tParam < 1)
        {
            tParam += Time.deltaTime * speedModifier;

            objectPosition =    1 * Mathf.Pow(1 - tParam, 3) * p1 + 
                                3 * Mathf.Pow(1 - tParam, 2) * tParam * p[1] + 
                                3 * (1 - tParam) * Mathf.Pow(tParam, 2) * p[2] + 
                                1 * Mathf.Pow(tParam, 3) * p[3];

            scale = 1 - endScale * tParam;
            transform.localScale = new Vector3(scale, scale, scale);
            transform.localEulerAngles = Helpers.AngleLerp(startingAngles, endAngle, tParam);

            transform.position = objectPosition;
            yield return new WaitForEndOfFrame();
        }
        CombatSystem.instance.UpdateDeckTexts();
        if (path == 2)
            Unsubscribe();
        else
            GetComponent<Image>().raycastTarget = true;
        
        animator.SetTrigger("DoneDiscarding");
    }

    public override void OnMouseEnter()
    {
        Debug.Log("Mouse is over card " + cardName);
        animator.SetBool("MouseIsOver", true);
    }

    public override void OnMouseExit()
    {
        Debug.Log("Mouse left card " + cardName);
        animator.SetBool("MouseIsOver", false);
        if(CombatSystem.instance.deSelectOnMouseLeave && selected) CombatSystem.instance.CancelCardSelection();
    }

    public override void OnMouseRightClick(bool allowDisplay = true)
    {
        if (selected)
            CombatSystem.instance.CancelCardSelection();
    }

    public override void OnMouseClick()
    {
        Debug.Log("Card clicked: " + cardName);
        base.OnMouseClick();
        CombatSystem.instance.CardClicked(this);

        if (CombatSystem.instance.quickPlayCards) Helpers.DelayForSeconds(.1f, () => CombatSystem.instance.CardClicked(this));
    }

    public void OnMouseOver()
    {
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
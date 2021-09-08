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
    Tween highlightTween;

    public int energySpent = 0;

    public Condition playCondition = new Condition();

    public delegate void DamageRecalcEvent();
    public event DamageRecalcEvent OnDamageRecalcEvent;

    public void DamageNeedsRecalc()
    {
        OnDamageRecalcEvent?.Invoke();
    }

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
        get => _selectable;
        set
        {
            _selectable = value;
            animator.SetBool("Selectable", value);
            EvaluateHighlightNotSelected();
        }
    }

    public bool isPlayable()
    {
        return playCondition.value && CombatSystem.instance.cEnergy >= cost;
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

    public void EvaluateHighlightNotSelected()
    {
        if (selected) return;

        if (!selectable)
            cardHighlightType = CardHighlightType.None;
        else
        {
            if (isPlayable())
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

    public static CardCombat Factory(CardData cardData)
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
        Debug.Log("MouseClickedCard. Firing OnMouseClick for card " + name);
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
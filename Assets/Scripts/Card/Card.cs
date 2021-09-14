using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public Rarity rarity;  
    public string cardName;
    public string cardId;
    public Sprite artwork;
    public List<CardModifierData> cardModifiers;

    public CardType cardType;

    public CardInt cost;
    public bool visibleCost = true;

    public CardData cardData;

    public List<CardSingleFieldProperty> singleFieldProperties = new List<CardSingleFieldProperty>();
    public HashSet<CardSingleFieldPropertyType> singleFieldTypes = new HashSet<CardSingleFieldPropertyType>();

    public List<CardEffectCarrier> Attacks = new List<CardEffectCarrier>();

    public List<CardEffectCarrier> Blocks = new List<CardEffectCarrier>();

    public List<CardEffectCarrier> effectsOnPlay = new List<CardEffectCarrier>();
    public List<CardEffectCarrier> effectsOnDraw = new List<CardEffectCarrier>();
    public List<CardActivitySetting> activitiesOnPlay = new List<CardActivitySetting>();
    public List<CardActivitySetting> activitiesOnDraw = new List<CardActivitySetting>();

    public GameObject animationPrefab;
    public CombatActor owner; 
    public Material material;

    public CardClassType classType = CardClassType.None;

    public List<Condition> registeredConditions = new List<Condition>();
    public List<IEventSubscriber> registeredSubscribers = new List<IEventSubscriber>();


    public void Reset()
    {
        Attacks.Clear();
        Blocks.Clear();
        effectsOnPlay.Clear();
        effectsOnDraw.Clear();
        activitiesOnDraw.Clear();
        activitiesOnPlay.Clear();
        singleFieldTypes.Clear();
        singleFieldProperties.Clear();
        cardModifiers.Clear();
    }
    public void BindCardData()
    {
        Reset();
        name            = cardData.cardName;
        cardId          = cardData.cardId;
        cardType        = cardData.cardType;
        rarity          = cardData.rarity;
        cardName        = cardData.cardName;
        artwork         = cardData.artwork;
        cost            = CardInt.Factory(cardData.cost,this);
        cardData.singleFieldProperties.OrderBy(s => (int)s).ToList().ForEach(s => RegisterSingleField(s));

        foreach(CardEffectCarrierData effect in cardData.effects) 
        {
            CardEffectCarrier carrier = SetupEffectcarrier(effect);
            if (effect.Type == EffectType.Damage)
                Attacks.Add(carrier);
            else if (effect.Type == EffectType.Block)
                Blocks.Add(carrier);
            else if(effect.execTime == CardComponentExecType.OnPlay)
                effectsOnPlay.Add(carrier);
            else if(effect.execTime == CardComponentExecType.OnDraw)
                effectsOnDraw.Add(carrier);
        }

        foreach (CardActivitySetting activity in cardData.activities)
        {
            if (activity.execTime == CardComponentExecType.OnPlay)
                activitiesOnPlay.Add(activity);
            else if (activity.execTime == CardComponentExecType.OnDraw)
                activitiesOnDraw.Add(activity);
        }

        animationPrefab = cardData.animationPrefab;
        classType       = cardData.cardClass;
        visibleCost     = cardData.visibleCost;
    }

    public void AddModifierToCard(CardModifierData cardModifierData)
    {
        Debug.Log("Add logic in here to add modifiers");
        CardVisual card;
        if (TryGetComponent<CardVisual>(out card))
        {
            card.UpdateCardVisual();
        }
    }

    public void Exhaust()
    {
        Image image = GetComponent<Image>();
        image.raycastTarget = false;

        // if (owner = CombatSystem.instance.Hero)
        // {
        //     CardCombat card = (CardCombat)this;
        //     card.DeselectCard();
        // }

        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(transform.DORotate(new Vector3(0,0,1080), 1, RotateMode.FastBeyond360));
        mySequence.Join(transform.DOScale(0, 1));
        //mySequence.Join(transform.DORotate(new Vector3(0,0,1080), 1));
        mySequence.OnComplete(() => {
            gameObject.SetActive(false);
            transform.localScale = Vector3.one;
            transform.localRotation = Quaternion.identity;
            image.raycastTarget = true;
        });
    }

    public void Mimic(Card card)
    {
        name = card.cardName;
        cardData = card.cardData;
        cardId = card.cardId;
        cardType = card.cardType;
        rarity = card.rarity;
        cardName = card.cardName;
        artwork = card.artwork;
        cost = card.cost;
        singleFieldProperties = card.singleFieldProperties;
        singleFieldTypes = card.singleFieldTypes;
        Attacks = card.Attacks;
        Blocks = card.Blocks;
        effectsOnPlay = card.effectsOnPlay;
        effectsOnDraw = card.effectsOnDraw;
        activitiesOnPlay = card.activitiesOnPlay;
        activitiesOnDraw = card.activitiesOnDraw;
        animationPrefab = card.animationPrefab;
        classType = card.classType;
        visibleCost = card.visibleCost;
    }

    public CardEffectCarrier SetupEffectcarrier(CardEffectCarrierData data)
    {
        if (this is CardCombat cardCombat)
            return new CardEffectCarrier(data, this, cardCombat.EvaluateHighlightNotSelected);
        else
            return new CardEffectCarrier(data, this);
    }

    public void RegisterSingleField(CardSingleFieldPropertyType s)
    {
        if (singleFieldTypes.Contains(s)) return;
        singleFieldProperties.Add(new CardSingleFieldProperty(s));
        singleFieldTypes.Add(s);
    }

    public List<CardEffectCarrier> GetEffectsByType(EffectType type)
    {
        return effectsOnPlay.Where(x => x.Type == type).ToList();
    }

    public CardActivitySetting GetactivityByType(CardActivityType type)
    {
        return activitiesOnPlay.Where(x => x.type == type).FirstOrDefault();
    }

    public static void SpliceCards(Card Target, Card a, Card b)
    {
        HashSet<EffectType> effectTypes = new HashSet<EffectType>();
        HashSet<CardActivityType> activityTypes = new HashSet<CardActivityType>();

        Target.classType = a.classType;
        Target.cardType = (CardType)Mathf.Min((int)a.cardType, (int)b.cardType);
        Target.rarity = a.rarity;

        Target.artwork = a.artwork;
        Target.animationPrefab = a.animationPrefab;
        Target.cost = CardInt.Factory(Mathf.Max(a.cost, b.cost).ToString(),Target);

        Target.cardName = a.cardName + (a.cardName.Contains("Mod+") ? "+" : " Mod+");

        //Target.Damage = a.Damage + b.Damage;
        //Target.Block = a.Block + b.Block;

        a.effectsOnPlay.ForEach(e => effectTypes.Add(e.Type));
        b.effectsOnPlay.ForEach(e => effectTypes.Add(e.Type));

        foreach(EffectType type in effectTypes)
        {
            List<CardEffectCarrier> aE = a.GetEffectsByType(type);
            List<CardEffectCarrier> bE = b.GetEffectsByType(type);

            if (aE.Count == 0)
                Target.effectsOnPlay.AddRange(bE);
            else if(bE.Count == 0)
                Target.effectsOnPlay.AddRange(aE);
            else
            {
                Target.effectsOnPlay.Add(aE[0] + bE[0]);
            }
        }

        a.activitiesOnPlay.ForEach(x => activityTypes.Add(x.type));
        b.activitiesOnPlay.ForEach(x => activityTypes.Add(x.type));
        List<CardActivityType> activityTypesList = activityTypes.OrderBy(x => x).ToList();

        foreach(CardActivityType type in activityTypesList)
        {
            if(type != CardActivityType.Splice)
            {
                Target.activitiesOnPlay.AddRange(a.activitiesOnPlay.Where(x => x.type == type));
                Target.activitiesOnPlay.AddRange(b.activitiesOnPlay.Where(x => x.type == type));
            }
            else
            {
                int aParam = int.Parse(a.GetactivityByType(CardActivityType.Splice).parameter);
                if(aParam > 1)
                    Target.activitiesOnPlay.Add(new CardActivitySetting() { type = CardActivityType.Splice, parameter = (aParam-1).ToString()});
            }
        }
    }




}


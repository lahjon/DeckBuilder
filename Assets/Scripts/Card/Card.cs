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
    public int timesUpgraded;
    public Sprite artwork;
    public List<CardFunctionalityData> cardModifiers;

    public CardType cardType;

    public CardCost cost;

    public CardData cardData;
    public int idx;

    public List<CardSingleFieldProperty> singleFieldProperties = new List<CardSingleFieldProperty>();

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
    public HashSet<ModifierType> modifiedTypes = new HashSet<ModifierType>();

    public bool upgradable { get => timesUpgraded < cardData.maxUpgrades; }

    public void Reset()
    {
        timesUpgraded = 0;
        Attacks.Clear();
        Blocks.Clear();
        effectsOnPlay.Clear();
        effectsOnDraw.Clear();
        activitiesOnDraw.Clear();
        activitiesOnPlay.Clear();
        singleFieldProperties.Clear();
        cardModifiers.Clear();

        if(this is CardVisual cv)
        {
            foreach (CardCostDisplay display in cv.energyToCostUI.Values)
                Destroy(display.gameObject);
            foreach (CardCostDisplay display in cv.energyToCostOptionalUI.Values)
                Destroy(display.gameObject);

            cv.energyToCostUI.Clear();
            cv.energyToCostOptionalUI.Clear();
        }
    }
    public void BindCardData()
    {
        Reset();
        name            = cardData.cardName;
        cardId          = cardData.id;
        cardType        = cardData.cardType;
        rarity          = cardData.rarity;
        cardName        = cardData.cardName;
        artwork         = cardData.artwork;
        cost            = new CardCost(this,cardData.costDatas, cardData.costOptionalDatas);
        cardData.singleFieldProperties.OrderBy(s => (int)s.prop).ToList().ForEach(s => RegisterSingleField(s));

        PostSingleFieldSetup();

        foreach (CardEffectCarrierData effect in cardData.effects) 
            SetupComponentFromData(effect);

        foreach (CardActivityData activity in cardData.activities)
            SetupComponentFromData(activity);

        animationPrefab = cardData.animationPrefab;
        classType       = cardData.cardClass;
    }

    public bool UpgradeCard()
    {
        if (!upgradable) return false;
        AddModifierToCard(cardData.upgrades[timesUpgraded]);

        return true;
    }
    public void AddModifierToCard(CardFunctionalityData data, ModifierType type = ModifierType.Upgrade, bool supressVisualUpdate = false)
    {
        if(type != ModifierType.Cursed) timesUpgraded++;
        cardModifiers.Add(data);
        modifiedTypes.Add(type);

        for (int i = 0; i < data.singleFieldProperties.Count; i++)
            RegisterSingleField(data.singleFieldProperties[i]);

        foreach(CardEffectCarrierData effect in data.effects)
        {
            List<CardEffectCarrier> targetList;
            if (effect.Type == EffectType.Damage)
                targetList = Attacks;
            else if (effect.Type == EffectType.Block)
                targetList = Blocks;
            else if (effect.execTime == CardComponentExecType.OnDraw)
                targetList = effectsOnDraw;
            else
                targetList = effectsOnPlay;

            if(!AddUpgradeComponent(targetList, effect))
                SetupComponentFromData(effect);
        }

        foreach (CardActivityData activity in data.activities)
            if (!AddUpgradeComponent(activity.execTime == CardComponentExecType.OnPlay ? activitiesOnPlay : activitiesOnDraw, activity))
                SetupComponentFromData(activity);

        cost.AbsorbModifier(data.costDatas, data.costOptionalDatas);

        if (this is CardVisual card && !supressVisualUpdate)
            card.UpdateAfterModifier();
    }

    public bool AddUpgradeComponent<T1,T2>(List<T1> targetList, T2 data) where T1: ICardUpgradableComponent where T2 : ICardUpgradingData
    {
        for (int i = 0; i < targetList.Count; i++)
        {
            if (targetList[i].CanAbsorb(data))
            {
                targetList[i].AbsorbModifier(data);
                return true;
            }
        }
        return false;
    }

    public bool HasProperty(CardSingleFieldPropertyType prop) => singleFieldProperties.Any(x => x.type == prop);
    public void Exhaust()
    {
        if (!(this is CardCombat)) return;

        Image image = GetComponent<Image>();
        image.raycastTarget = false;

        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(transform.DORotate(new Vector3(0,0,1080), 1, RotateMode.FastBeyond360));
        mySequence.Join(transform.DOScale(0, 1));
        mySequence.OnComplete(() => {
            gameObject.SetActive(false);
            transform.localScale = Vector3.one;
            transform.localRotation = Quaternion.identity;
            image.raycastTarget = true;
        });
    }

    public void Mimic(Card card)
    {
        Reset();
        name = card.cardName;
        cardData = card.cardData;
        cardId = card.cardId;
        cardType = card.cardType;
        rarity = card.rarity;
        cardName = card.cardName;
        artwork = card.artwork;
        cost = card.cost;
        idx = card.idx;
        timesUpgraded = card.timesUpgraded;
        cardModifiers               = new List<CardFunctionalityData>(card.cardModifiers);
        singleFieldProperties       = new List<CardSingleFieldProperty>(card.singleFieldProperties);
        Attacks                     = new List<CardEffectCarrier>(card.Attacks);
        Blocks                      = new List<CardEffectCarrier>(card.Blocks);
        effectsOnPlay               = new List<CardEffectCarrier>(card.effectsOnPlay);
        effectsOnDraw               = new List<CardEffectCarrier>(card.effectsOnDraw);
        activitiesOnPlay            = new List<CardActivitySetting>(card.activitiesOnPlay);
        activitiesOnDraw            = new List<CardActivitySetting>(card.activitiesOnDraw);
        modifiedTypes.UnionWith(card.modifiedTypes);
        animationPrefab             = card.animationPrefab;
        classType = card.classType;
    }

    public void SetupComponentFromData(CardEffectCarrierData data)
    {
        CardEffectCarrier carrier;
        if (this is CardCombat cardCombat)
            carrier =  new CardEffectCarrier(data, this, cardCombat.EvaluateHighlightNotSelected);
        else
            carrier = new CardEffectCarrier(data, this);

        if (data.Type == EffectType.Damage)
            Attacks.Add(carrier);
        else if (data.Type == EffectType.Block)
            Blocks.Add(carrier);
        else if (data.execTime == CardComponentExecType.OnPlay)
            effectsOnPlay.Add(carrier);
        else if (data.execTime == CardComponentExecType.OnDraw)
            effectsOnDraw.Add(carrier);
    }

    public void SetupComponentFromData(CardActivityData data)
    {
        CardActivitySetting setting;
        if (this is CardCombat cardCombat)
            setting =  new CardActivitySetting(data, this, cardCombat.EvaluateHighlightNotSelected);
        else
            setting =  new CardActivitySetting(data, this);

        if (data.execTime == CardComponentExecType.OnPlay)
            activitiesOnPlay.Add(setting);
        else if (data.execTime == CardComponentExecType.OnDraw)
            activitiesOnDraw.Add(setting);
    }

    public void RegisterSingleField(CardSingleFieldPropertyTypeWrapper typeWrapper)
    {
        if (typeWrapper.val && !HasProperty(typeWrapper.prop))
            singleFieldProperties.Add(new CardSingleFieldProperty(typeWrapper.prop));
        else if (!typeWrapper.val && HasProperty(typeWrapper.prop))
        {
            for (int i = 0; i < singleFieldProperties.Count; i++)
            {
                if (singleFieldProperties[i].type == typeWrapper.prop)
                {
                    singleFieldProperties.RemoveAt(i);
                    break;
                }
            }
        }
    }

    public void PostSingleFieldSetup()
    {
        if(singleFieldProperties.Any(x => x.type == CardSingleFieldPropertyType.Fortify))
        {
            for(int i = 0; i < cardData.effects.Count; i++)
            {
                cardData.effects[i].Value.linkedProp = CardLinkablePropertyType.CountPlayedCardsSameName;
            }
        }
    }

    public List<CardEffectCarrier> GetEffectsByType(EffectType type)
    {
        return effectsOnPlay.Where(x => x.Type.Equals(type)).ToList();
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
        //Target.cost = CardInt.Factory(Mathf.Max(a.cost, b.cost).ToString(),Target);

        Target.cardName = a.cardName + (a.cardName.Contains("Mod+") ? "+" : " Mod+");

        //Target.Damage = a.Damage + b.Damage;
        //Target.Block = a.Block + b.Block;

        a.effectsOnPlay.ForEach(e => effectTypes.Add(e.Type.effectType));
        b.effectsOnPlay.ForEach(e => effectTypes.Add(e.Type.effectType));

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
                int aParam = int.Parse(a.GetactivityByType(CardActivityType.Splice).strParameter) - 1;
                if (aParam > 1)
                    Target.activitiesOnPlay.Add(new CardActivitySetting(new CardActivityData() { type = CardActivityType.Splice, strParameter = aParam.ToString() },Target));
            }
        }
    }




}


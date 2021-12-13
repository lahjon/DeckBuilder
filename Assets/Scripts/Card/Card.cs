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

    public List<StatusEffectCarrier> Attacks = new List<StatusEffectCarrier>();
    public List<StatusEffectCarrier> Blocks = new List<StatusEffectCarrier>();

    public List<StatusEffectCarrier> effectsOnPlay = new List<StatusEffectCarrier>();
    public List<StatusEffectCarrier> effectsOnDraw = new List<StatusEffectCarrier>();
    public List<CombatActivitySetting> activitiesOnPlay = new List<CombatActivitySetting>();
    public List<CombatActivitySetting> activitiesOnDraw = new List<CombatActivitySetting>();

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
            cv.manualToolTips.Clear();
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
            List<StatusEffectCarrier> targetList;
            if (effect.Type == StatusEffectType.Damage)
                targetList = Attacks;
            else if (effect.Type == StatusEffectType.Block)
                targetList = Blocks;
            else if (effect.execTime == CardComponentExecType.OnDraw)
                targetList = effectsOnDraw;
            else
                targetList = effectsOnPlay;

            if(!AddUpgradeComponent(targetList, effect, type))
                SetupComponentFromData(effect);
        }

        foreach (CardActivityData activity in data.activities)
            if (!AddUpgradeComponent(activity.execTime == CardComponentExecType.OnPlay ? activitiesOnPlay : activitiesOnDraw, activity, type))
                SetupComponentFromData(activity);

        cost.AbsorbModifier(data.costDatas, data.costOptionalDatas);

        if (this is CardVisual card && !supressVisualUpdate)
            card.UpdateAfterModifier();
    }

    public bool AddUpgradeComponent<T1,T2>(List<T1> targetList, T2 data, ModifierType type) where T1: ICardUpgradableComponent where T2 : ICardUpgradingData
    {
        for (int i = 0; i < targetList.Count; i++)
        {
            if (targetList[i].CanAbsorb(data))
            {
                targetList[i].AbsorbModifier(data);
                targetList[i].RegisterModified(type);
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
        Attacks                     = new List<StatusEffectCarrier>(card.Attacks);
        Blocks                      = new List<StatusEffectCarrier>(card.Blocks);
        effectsOnPlay               = new List<StatusEffectCarrier>(card.effectsOnPlay);
        effectsOnDraw               = new List<StatusEffectCarrier>(card.effectsOnDraw);
        activitiesOnPlay            = new List<CombatActivitySetting>(card.activitiesOnPlay);
        activitiesOnDraw            = new List<CombatActivitySetting>(card.activitiesOnDraw);
        modifiedTypes.UnionWith(card.modifiedTypes);
        animationPrefab             = card.animationPrefab;
        classType = card.classType;
    }

    public void SetupComponentFromData(CardEffectCarrierData data, ModifierType type = ModifierType.None)
    {
        StatusEffectCarrier carrier;
        if (this is CardCombat cardCombat)
            carrier =  new StatusEffectCarrier(data, this, cardCombat.EvaluateHighlightNotSelected);
        else
            carrier = new StatusEffectCarrier(data, this);

        carrier.RegisterModified(type);

        if (data.Type == StatusEffectType.Damage)
            Attacks.Add(carrier);
        else if (data.Type == StatusEffectType.Block)
            Blocks.Add(carrier);
        else if (data.execTime == CardComponentExecType.OnPlay)
            effectsOnPlay.Add(carrier);
        else if (data.execTime == CardComponentExecType.OnDraw)
            effectsOnDraw.Add(carrier);
    }

    public void SetupComponentFromData(CardActivityData data, ModifierType type = ModifierType.None)
    {
        CombatActivitySetting setting;
        if (this is CardCombat cardCombat)
            setting =  new CombatActivitySetting(data, this, cardCombat.EvaluateHighlightNotSelected);
        else
            setting =  new CombatActivitySetting(data, this);

        setting.RegisterModified(type);

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

    public List<StatusEffectCarrier> GetEffectsByType(StatusEffectType type)
    {
        return effectsOnPlay.Where(x => x.info.Equals(type)).ToList();
    }

    public CombatActivitySetting GetactivityByType(CombatActivityType type)
    {
        return activitiesOnPlay.Where(x => x.type == type).FirstOrDefault();
    }

    public static void SpliceCards(Card Target, Card a, Card b)
    {
        HashSet<StatusEffectType> effectTypes = new HashSet<StatusEffectType>();
        HashSet<CombatActivityType> activityTypes = new HashSet<CombatActivityType>();

        Target.classType = a.classType;
        Target.cardType = (CardType)Mathf.Min((int)a.cardType, (int)b.cardType);
        Target.rarity = a.rarity;

        Target.artwork = a.artwork;
        Target.animationPrefab = a.animationPrefab;
        //Target.cost = CardInt.Factory(Mathf.Max(a.cost, b.cost).ToString(),Target);

        Target.cardName = a.cardName + (a.cardName.Contains("Mod+") ? "+" : " Mod+");

        //Target.Damage = a.Damage + b.Damage;
        //Target.Block = a.Block + b.Block;

        a.effectsOnPlay.ForEach(e => effectTypes.Add(e.info.type));
        b.effectsOnPlay.ForEach(e => effectTypes.Add(e.info.type));

        foreach(StatusEffectType type in effectTypes)
        {
            List<StatusEffectCarrier> aE = a.GetEffectsByType(type);
            List<StatusEffectCarrier> bE = b.GetEffectsByType(type);

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
        List<CombatActivityType> activityTypesList = activityTypes.OrderBy(x => x).ToList();

        foreach(CombatActivityType type in activityTypesList)
        {
            if(type != CombatActivityType.Splice)
            {
                Target.activitiesOnPlay.AddRange(a.activitiesOnPlay.Where(x => x.type == type));
                Target.activitiesOnPlay.AddRange(b.activitiesOnPlay.Where(x => x.type == type));
            }
            else
            {
                int aParam = int.Parse(a.GetactivityByType(CombatActivityType.Splice).strParameter) - 1;
                if (aParam > 1)
                    Target.activitiesOnPlay.Add(new CombatActivitySetting(new CardActivityData() { type = CombatActivityType.Splice, strParameter = aParam.ToString() },Target));
            }
        }
    }




}


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;

public abstract class CombatActor : MonoBehaviour, IToolTipable
{
    [HideInInspector] public int maxHitPoints;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public BoxCollider2D collision;
    int _hitPoints;

    public int hitPoints { get { return _hitPoints; } set
        {
            _hitPoints = value;
            healthEffectsUI.UpdateHealthBar(hitPoints, maxHitPoints);
        }
    }

    private int shield = 0;

    public Transform AnchorToolTip;

    public HealthEffectsUI healthEffectsUI;

    public Dictionary<EffectType, CardEffect> effectTypeToRule = new Dictionary<EffectType, CardEffect>();


    public List<Func<float>> dealAttackMult = new List<Func<float>>();
    public List<Func<float>> takeAttackMult = new List<Func<float>>();

    public List<Func<int>> dealAttackLinear = new List<Func<int>>();
    public List<Func<int,int>> looseLifeTransform = new List<Func<int,int>>();

    public Dictionary<CombatActor, List<Func<float>>> dealAttackActorMods = new Dictionary<CombatActor, List<Func<float>>>();

    public List<Func<IEnumerator>> actionsNewTurn = new List<Func<IEnumerator>>();
    public List<Func<IEnumerator>> actionsEndTurn = new List<Func<IEnumerator>>();
    public List<Func<IEnumerator>> actionsStartCombat = new List<Func<IEnumerator>>();

    public List<Func<CombatActor, IEnumerator>> onAttackRecieved = new List<Func<CombatActor,IEnumerator>>();
    public List<Func<CombatActor, IEnumerator>> onUnblockedDmgDealt = new List<Func<CombatActor, IEnumerator>>();

    public List<Func<float>> gainBlockMult = new List<Func<float>>();
    public List<Func<IEnumerator>> OnBlockGained = new List<Func<IEnumerator>>();
    
    public ListEventReporter<Card> deck;
    public ListEventReporter<Card> discard; 
    public ListEventReporter<Card> exhaust = new ListEventReporter<Card>(); 


    public List<CombatActor> allies;
    public List<CombatActor> enemies;

    public int strengthCombat = 0;

    public Dictionary<CardTargetType, CardTargetType> targetDistorter = new Dictionary<CardTargetType, CardTargetType>()
    {   {CardTargetType.All,            CardTargetType.All},
        {CardTargetType.AlliesExclSelf, CardTargetType.AlliesExclSelf},
        {CardTargetType.AlliesInclSelf, CardTargetType.AlliesInclSelf},
        {CardTargetType.EnemyAll,       CardTargetType.EnemyAll},
        {CardTargetType.EnemyRandom,    CardTargetType.EnemyRandom},
        {CardTargetType.EnemySingle,    CardTargetType.EnemySingle},
        {CardTargetType.Self,           CardTargetType.Self}
    };



    public void InitializeCombat()
    {
        AnchorToolTip.localPosition = new Vector3(collision.size.x / 2, collision.size.y * 0.9f);

        if (!typeof(CombatActorCompanion).IsAssignableFrom(this.GetType()))
        {
            healthEffectsUI.UpdateShield(shield);
            actionsNewTurn.Add(RemoveAllBlock);
        }
        actionsNewTurn.Add(EffectsOnNewTurnBehavior);

        dealAttackLinear.Add(ApplyCombatStrength);

        foreach (CombatActor actor in CombatSystem.instance.ActorsInScene)
            dealAttackActorMods[actor] = new List<Func<float>>(); //technically includes oneself but who cares?

        SetupAlliesEnemies();
    }

    public abstract void SetupAlliesEnemies();

    public void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            Card temp = deck[i];
            int index = UnityEngine.Random.Range(i, deck.Count);
            deck[i] = deck[index];
            deck[index] = temp;
        }
    }
    public abstract void RecalcDamage();

    public virtual void DiscardCard(Card card)
    {
        discard.Insert(0, card);
        Debug.Log("insert card in discard: " + card);
    }

    public virtual void ExhaustCard(Card card)
    {
        Debug.Log("insert card in exhaust");
        exhaust.Insert(0, card);
        card.Exhaust();
        if (card is CardCombat cc)
            cc.cardCollider.gameObject.SetActive(false);
    }

    public virtual void CardResolved(Card card)
    {
        if (card.HasProperty(CardSingleFieldPropertyType.Exhaust))
            ExhaustCard(card);
        else
            DiscardCard(card);
    }

    public virtual void AddToDeck(Card card)
    {
        deck.Insert(0, card);
    }

    public virtual void AddToDeckSemiRandom(Card card)
    {
        int index = UnityEngine.Random.Range(0, deck.Count + 1);
        deck.Insert(index, card);
    }


    public IEnumerator GetAttacked(int damage, CombatActor sourceActor)
    {
        int unblockedDamage = TakeDamage(damage);
        TakeDamageAnimation();

        for (int i = 0; i < onAttackRecieved.Count; i++)
            yield return onAttackRecieved[i].Invoke(sourceActor);


        if (unblockedDamage > 0)
            for (int i = 0; i < sourceActor.onUnblockedDmgDealt.Count; i++)
                yield return sourceActor.onUnblockedDmgDealt[i].Invoke(this);
    }

    public virtual void TakeDamageAnimation()
    {
        // Helpers.DelayForSeconds(.5f, () => {
        //     transform.DOMoveX(transform.position.x + -2f, .05f).SetLoops(2, LoopType.Yoyo);
        // });
    }
    public virtual void AttackAnimation()
    {
        
    }

    public int TakeDamage(int damage)
    {
        if (shield > 0)
        {
            int shieldDamage = Mathf.Min(shield, damage);
            StartCoroutine(LooseBlock(shieldDamage));
            damage -= shieldDamage;
        }

        int unblockedDamage = LooseLife(damage);
        return unblockedDamage;
    }

    public int LooseLife(int lifeToLose)
    {
        if (lifeToLose == 0) return 0;

        List<Func<int, int>> snapShot = new List<Func<int, int>>(looseLifeTransform); 
        foreach (Func<int, int> func in snapShot)
            if(func != null)
                lifeToLose = func(lifeToLose);

        if (lifeToLose == 0) return 0;

        lifeToLose = Mathf.Min(lifeToLose, hitPoints);

        hitPoints -= lifeToLose;

        if (this == CombatSystem.instance.Hero)
            WorldSystem.instance.characterManager.TakeDamage(lifeToLose);

        healthEffectsUI.StartLifeLossNotification(lifeToLose);

        if (hitPoints <= 0)
            CombatSystem.instance.ReportDeath(this);

        return lifeToLose;
    }

    public void HealLife(int x)
    {
        if (x == 0) return;
        x = Mathf.Min(x, maxHitPoints - hitPoints);
        hitPoints += x;
        if (this == CombatSystem.instance.Hero)
            WorldSystem.instance.characterManager.Heal(x);
    }


    public IEnumerator RecieveEffectNonDamageNonBlock(CardEffectCarrier effectInfo)
    {
        if (!effectTypeToRule.ContainsKey(effectInfo.Type))
        {
            effectTypeToRule[effectInfo.Type] = effectInfo.Type.constructor.Invoke();
            effectTypeToRule[effectInfo.Type].type = effectInfo.Type;
            effectTypeToRule[effectInfo.Type].actor = this;
        }

        yield return StartCoroutine(effectTypeToRule[effectInfo.Type].RecieveInput(effectInfo.Value));
    }

    public IEnumerator EffectsOnNewTurnBehavior()
    {
        List<EffectType> effects = new List<EffectType>(effectTypeToRule.Keys);
        foreach (EffectType effect in effects)
            if (effectTypeToRule[effect].OnNewTurn != null)
                yield return StartCoroutine(effectTypeToRule[effect].OnNewTurn());
    }

    public IEnumerator EffectsOnEndTurnBehavior()
    {
        List<EffectType> effects = new List<EffectType>(effectTypeToRule.Keys);
        foreach (EffectType effect in effects)
            if(effectTypeToRule.ContainsKey(effect) && effectTypeToRule[effect].OnEndTurn != null)
                yield return StartCoroutine(effectTypeToRule[effect].OnEndTurn());
    }

    public IEnumerator GainBlock(int change)
    {
        shield += change;
        yield return StartCoroutine(healthEffectsUI?.UpdateShield(shield));
        List<Func<IEnumerator>> funcs = new List<Func<IEnumerator>>(OnBlockGained);
        foreach (Func<IEnumerator> funkie in funcs)
            if(funkie != null)
                yield return StartCoroutine(funkie.Invoke());
    }

    public IEnumerator LooseBlock(int change)
    {
        //Debug.Log("Starting change of block with " + change);
        shield -= change;
        //Debug.Log("Shield now at  " + shield);
        yield return StartCoroutine(healthEffectsUI?.UpdateShield(shield));
    }

    public IEnumerator RemoveAllBlock()
    {
        yield return StartCoroutine(LooseBlock(shield));
    }

    public int ApplyCombatStrength()
    {
        return strengthCombat;
    }


    public (List<string> tips, Vector3 worldPosition) GetTipInfo()
    {
        List<string> toolTipTextBits = new List<string>();
        effectTypeToRule.Keys.ToList().ForEach(x => { toolTipTextBits.Add(x.GetEffectTypeStruct().toolTipCard); });
        return (toolTipTextBits, AnchorToolTip.position);
    }

    public void ModifyStrength(int x) => strengthCombat += x;

    public abstract int GetStat(StatType stat);

}

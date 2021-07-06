using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

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

    public Dictionary<CombatActor, List<Func<float>>> dealAttackActorMods = new Dictionary<CombatActor, List<Func<float>>>();

    public List<Func<IEnumerator>> actionsNewTurn = new List<Func<IEnumerator>>();
    public List<Func<IEnumerator>> actionsEndTurn = new List<Func<IEnumerator>>();
    public List<Func<IEnumerator>> actionsStartCombat = new List<Func<IEnumerator>>();

    public List<Func<CombatActor, IEnumerator>> onAttackRecieved = new List<Func<CombatActor,IEnumerator>>();

    
    public List<Card> deck = new List<Card>();
    public List<Card> discard = new List<Card>();


    public List<CombatActor> allies;
    public List<CombatActor> enemies;

    public int strengthCombat = 0;

    public Dictionary<CardTargetType, CardTargetType> targetDistorter = new Dictionary<CardTargetType, CardTargetType>()
    { {CardTargetType.All, CardTargetType.All},
        {CardTargetType.EnemyAll, CardTargetType.All},
        {CardTargetType.EnemyRandom, CardTargetType.EnemyRandom},
        {CardTargetType.EnemySingle, CardTargetType.EnemySingle},
        {CardTargetType.Self, CardTargetType.Self}
    };



    public void InitializeCombat()
    {
        AnchorToolTip.localPosition = new Vector3(collision.size.x / 2, collision.size.y * 0.9f);
        healthEffectsUI.UpdateShield(shield);

        actionsNewTurn.Add(RemoveAllBlock);
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

    public virtual void DiscardCard(Card card)
    {
        discard.Insert(0,card);
    }

    public virtual void CardResolved(Card card)
    {
        if (card.exhaust)
            Destroy(card.gameObject);
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
        TakeDamage(damage);

        for (int i = 0; i < onAttackRecieved.Count; i++)
            yield return onAttackRecieved[i].Invoke(sourceActor);
    }


    public void TakeDamage(int damage)
    {
        if (shield > 0)
        {
            int shieldDamage = Mathf.Min(shield, damage);
            StartCoroutine(ChangeBlock(-shieldDamage));
            damage -= shieldDamage;
        }

        LooseLife(Mathf.Min(damage));

    }

    public void LooseLife(int lifeToLose)
    {
        if (lifeToLose == 0) return;

        hitPoints -= Mathf.Min(lifeToLose, hitPoints);
        Debug.Log(hitPoints);
        if (this == CombatSystem.instance.Hero)
            WorldSystem.instance.characterManager.TakeDamage(lifeToLose);

        //Debug.Log("Starting LifeLoss");
        healthEffectsUI.StartLifeLossNotification(lifeToLose);

        if (hitPoints <= 0)
            CombatSystem.instance.ReportDeath(this);
    }


    public IEnumerator RecieveEffectNonDamageNonBlock(CardEffectInfo effectInfo)
    {
        if (!effectTypeToRule.ContainsKey(effectInfo.Type))
        {
            effectTypeToRule[effectInfo.Type] = effectInfo.Type.GetRuleEffect();
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

    public IEnumerator ChangeBlock(int change)
    {
        //Debug.Log("Starting change of block with " + change);
        shield += change;
        //Debug.Log("Shield now at  " + shield);
        yield return StartCoroutine(healthEffectsUI.UpdateShield(shield));
    }

    public IEnumerator RemoveAllBlock()
    {
        yield return StartCoroutine(ChangeBlock(-shield));
    }

    public int ApplyCombatStrength()
    {
        return strengthCombat;
    }


    public (List<string> tips, Vector3 worldPosition) GetTipInfo()
    {
        List<string> toolTipTextBits = new List<string>();
        effectTypeToRule.Keys.ToList().ForEach(x => { toolTipTextBits.Add(x.GetDescription()); });
        return (toolTipTextBits, AnchorToolTip.position);
    }

    public void ModifyStrength(int x) => strengthCombat += x;
    

}

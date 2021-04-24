using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CombatActor : MonoBehaviour
{
    [HideInInspector] public int maxHitPoints;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public BoxCollider2D collision;
    [HideInInspector] public CombatController combatController;
    int _hitPoints;

    public int hitPoints { get { return _hitPoints; } set
        {
            _hitPoints = value;
            healthEffectsUI.UpdateHealthBar(hitPoints, maxHitPoints);
        }
    }

    private int shield = 0;

    public HealthEffectsUI healthEffectsUI;

    public Dictionary<EffectType, RuleEffect> effectTypeToRule = new Dictionary<EffectType, RuleEffect>();


    public List<Func<float, float>> dealAttackMods = new List<Func<float, float>>();
    public List<Func<float, float>> takeAttackMods = new List<Func<float, float>>();

    public Dictionary<CombatActor, List<Func<float, float>>> dealAttackActorMods = new Dictionary<CombatActor, List<Func<float, float>>>();

    public List<Func<IEnumerator>> actionsNewTurn = new List<Func<IEnumerator>>();
    public List<Func<IEnumerator>> actionsEndTurn = new List<Func<IEnumerator>>();
    public List<Func<IEnumerator>> actionsStartCombat = new List<Func<IEnumerator>>();

    public List<Func<CombatActor, IEnumerator>> onAttackRecieved = new List<Func<CombatActor,IEnumerator>>();

    
    public List<Card> deck = new List<Card>();
    public List<Card> discard = new List<Card>();

    

    public void InitializeCombat()
    {
        actionsNewTurn.Add(RemoveAllBlock);
        healthEffectsUI.UpdateShield(shield);

        foreach (CombatActor actor in combatController.ActorsInScene)
            dealAttackActorMods[actor] = new List<Func<float, float>>();
    }

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
        discard.Insert(0, card);
    }

    public virtual void AddToDeck(Card card)
    {
        deck.Insert(0, card);
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

        if (hitPoints == 0)
            WorldSystem.instance.combatManager.combatController.ReportDeath(this);
    }

    public void LooseLife(int lifeToLose)
    {
        if (lifeToLose == 0) return;

        //k�r on life about to be lost
        hitPoints -= Mathf.Min(lifeToLose, hitPoints);
        if (this == combatController.Hero)
            WorldSystem.instance.characterManager.TakeDamage(lifeToLose);

        Debug.Log("Starting LifeLoss");
        healthEffectsUI.StartLifeLossNotification(lifeToLose);
        // k�r onLifeLost
    }


    public void RecieveEffectNonDamageNonBlock(CardEffect effect)
    {
        if (effectTypeToRule.ContainsKey(effect.Type))
            effectTypeToRule[effect.Type].RecieveInput(effect);
        else
        {
            effectTypeToRule[effect.Type] = effect.Type.GetRuleEffect();
            effectTypeToRule[effect.Type].actor = this;
            effectTypeToRule[effect.Type].RecieveInput(effect);
        }
    }

    public void EffectsOnNewTurnBehavior()
    {
        List<EffectType> effects = new List<EffectType>(effectTypeToRule.Keys);
        foreach (EffectType effect in effects)
            effectTypeToRule[effect].OnNewTurn();
    }

    public void EffectsOnEndTurnBehavior()
    {
        List<EffectType> effects = new List<EffectType>(effectTypeToRule.Keys);
        foreach (EffectType effect in effects)
        {
            effectTypeToRule[effect].OnEndTurn();
            healthEffectsUI.ModifyEffectUI(effectTypeToRule[effect]);
            if (effectTypeToRule[effect].nrStacked <= 0)
            {
                effectTypeToRule[effect].RemoveFunctionFromRules();
                effectTypeToRule.Remove(effect);
            }
        }
    }

    public IEnumerator ChangeBlock(int change)
    {
        Debug.Log("Starting change of block with " + change);
        shield += change;
        Debug.Log("Shield now at  " + shield);
        yield return StartCoroutine(healthEffectsUI.UpdateShield(shield));
    }

    public IEnumerator RemoveAllBlock()
    {
        yield return StartCoroutine(ChangeBlock(-shield));
    }

    

}

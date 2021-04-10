using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CombatActor : MonoBehaviour
{
    public int maxHitPoints;
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

    public CombatController combatController;

    public List<Func<float, float>> dealAttackMods = new List<Func<float, float>>();
    public List<Func<float, float>> takeAttackMods = new List<Func<float, float>>();

    public List<Func<IEnumerator>> actionsNewTurn = new List<Func<IEnumerator>>();
    public List<Func<IEnumerator>> actionsEndTurn = new List<Func<IEnumerator>>();

    public List<Func<CombatActor, IEnumerator>> onAttackRecieved = new List<Func<CombatActor,IEnumerator>>();

    public List<Card> deck = new List<Card>();
    public List<Card> discard = new List<Card>();

    public void Awake()
    {
        actionsNewTurn.Add(RemoveAllBlock);
    }

    public void Start()
    {
        healthEffectsUI.UpdateShield(shield);
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
            ChangeBlock(-shieldDamage);
            damage -= shieldDamage;
        }

        LooseLife(Mathf.Min(damage));

        if (hitPoints == 0)
            WorldSystem.instance.combatManager.combatController.ReportDeath(this);
    }

    public void LooseLife(int lifeToLose)
    {
        if (lifeToLose == 0) return;

        //kör on life about to be lost
        hitPoints -= Mathf.Min(lifeToLose, hitPoints);
        if (this == combatController.Hero)
            WorldSystem.instance.characterManager.TakeDamage(lifeToLose);
        // kör onLifeLost
    }

    public void RecieveEffectNonDamageNonBlock(CardEffect effect)
    {
        if (effectTypeToRule.ContainsKey(effect.Type))
        {
            effectTypeToRule[effect.Type].nrStacked += effect.Value * effect.Times;
            if(effectTypeToRule[effect.Type].nrStacked == 0 && effectTypeToRule[effect.Type].triggerRecalcDamage) combatController.RecalcAllCardsDamage();
        }
        else
        {
            effectTypeToRule[effect.Type] = effect.Type.GetRuleEffect();
            effectTypeToRule[effect.Type].actor = this;
            effectTypeToRule[effect.Type].AddFunctionToRules();
            effectTypeToRule[effect.Type].nrStacked = effect.Value * effect.Times;
            if (effectTypeToRule[effect.Type].triggerRecalcDamage) combatController.RecalcAllCardsDamage();
        }

        //Update UI
        healthEffectsUI.ModifyEffectUI(effectTypeToRule[effect.Type]);
    }

    public void EffectsOnNewTurnBehavior()
    {
        List<EffectType> effects = new List<EffectType>(effectTypeToRule.Keys);
        foreach (EffectType effect in effects)
        {
            effectTypeToRule[effect].OnNewTurnBehaviour();
            healthEffectsUI.ModifyEffectUI(effectTypeToRule[effect]);
            if (effectTypeToRule[effect].nrStacked <= 0)
            {
                effectTypeToRule[effect].RemoveFunctionFromRules();
                effectTypeToRule.Remove(effect);
            }
        }
    }

    public void EffectsOnEndTurnBehavior()
    {
        List<EffectType> effects = new List<EffectType>(effectTypeToRule.Keys);
        foreach (EffectType effect in effects)
        {
            effectTypeToRule[effect].OnEndTurnBehaviour();
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

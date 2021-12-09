using System;
using System.Collections;
using UnityEngine;

public abstract class CardEffect
{
    public CombatActor actor;
    public string effectName { get { return GetType().ToString().Substring(10); } }
    public virtual bool triggerRecalcDamageSelf { get { return false; } }
    public virtual bool triggerRecalcDamageEnemy { get { return false; } }
    public virtual bool stackable { get { return true; } }

    public float applyEffectWait = 0.5f;

    public Func<IEnumerator> OnNewTurn;
    public Func<IEnumerator> OnEndTurn;

    public EffectDisplay UI;
    public StatusEffectTypeInfo info;

    private int _nrStacked;
    public int nrStacked { get => _nrStacked; set
        {
            int oldVal = _nrStacked;
            _nrStacked = value;
            if (oldVal != value)
                UpdateEffectUI();
        } }

    public CardEffect()
    {
        OnNewTurn = null;
        OnEndTurn = _OnEndTurn;
    }

    public virtual IEnumerator RecieveInput(int stackUpdate)
    {
        if (stackUpdate != 0)
        {
            int nrStackedPre = nrStacked;

            nrStacked += stackUpdate;

            RespondStackUpdate(stackUpdate);

            if (nrStackedPre == 0 && nrStacked != 0)
            {
                AddFunctionToRules();
                if (triggerRecalcDamageSelf) actor.RecalcDamage();
                if (triggerRecalcDamageEnemy)
                    foreach (CombatActor enemy in actor.enemies)
                        enemy.RecalcDamage();
            }

            if (nrStacked == 0) Dismantle();
            yield return new WaitForSeconds(applyEffectWait);
        }
    }
    public virtual void RespondStackUpdate(int update)
    {
    }

    public virtual void AddFunctionToRules()
    {
    }

    public virtual void RemoveFunctionFromRules()
    {
    }


    protected virtual IEnumerator _OnNewTurn()
    {
        yield return null; //this method is always meant to be overwritten if used 
    }

    protected virtual IEnumerator _OnEndTurn()
    {
        yield return CombatSystem.instance.StartCoroutine(RecieveInput(-1));
    }

    public virtual void OnActorDeath()
    {

    }


    public virtual string strStacked()
    {
        return stackable ? nrStacked.ToString() : "";
    }

    public virtual void Dismantle()
    {
        RemoveFunctionFromRules();
        if (triggerRecalcDamageSelf) actor.RecalcDamage();
        if (triggerRecalcDamageEnemy)
            foreach (CombatActor enemy in actor.enemies)
                enemy.RecalcDamage();


        actor.effectTypeToRule.Remove(info);
    }

    public void UpdateEffectUI()
    {
        if (nrStacked == 0)
        {
            if (UI == null) return;
            actor.healthEffectsUI.RetireEffectDisplay(UI);
            return;
        }
        else if(UI == null)
            UI = actor.healthEffectsUI.GetEffectDisplay(this);

        UI.RefreshLabel();
    }

}

using System;
using System.Collections;
using UnityEngine;

public abstract class CardEffect
{
    public CombatActor actor;
    public string effectName { get { return GetType().ToString().Substring(10); } }
    public abstract bool isBuff { get; }
    public virtual bool triggerRecalcDamageSelf { get { return false; } }
    public virtual bool triggerRecalcDamageEnemy { get { return false; } }
    public virtual bool stackable { get { return true; } }

    public float applyEffectWait = 0.1f;

    public Func<IEnumerator> OnNewTurn;
    public Func<IEnumerator> OnEndTurn;

    public EffectType type
    {
        get
        {
            EffectType retType;
            Enum.TryParse<EffectType>(effectName, out retType);
            return retType;
        }
    }

    public int nrStacked;

    public CardEffect()
    {
        OnNewTurn = null;
        OnEndTurn = _OnEndTurn;
    }

    public virtual IEnumerator RecieveInput(int stackUpdate)
    {
        Debug.Log("Recieving input for effect with nr " + stackUpdate);
        if (stackUpdate != 0)
        {
            int nrStackedPre = nrStacked;

            nrStacked += stackUpdate;

            actor.healthEffectsUI.UpdateEffectUI(this);

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


    internal virtual IEnumerator _OnNewTurn()
    {
        yield return null; //this method is always meant to be overwritten if used 
    }

    public virtual IEnumerator _OnEndTurn()
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


        actor.effectTypeToRule.Remove(type);
    }


}

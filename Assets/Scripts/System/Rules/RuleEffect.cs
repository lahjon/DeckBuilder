using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class RuleEffect
{
    public CombatActor actor;
    public string effectName { get { return GetType().ToString().Substring(10); } }
    public abstract bool isBuff { get; }
    public abstract bool triggerRecalcDamage { get; }

    public EffectType type {get 
        {
            EffectType retType;
            Enum.TryParse<EffectType>(effectName, out retType);
            return retType;
        }
    }

    public bool stackable = true;
    public int nrStacked;

    public virtual void AddFunctionToRules()
    {

    }

    public virtual void RemoveFunctionFromRules()
    {

    }

    public virtual void OnNewTurnBehaviour()
    {
        nrStacked--;
    }

    public virtual void OnEndTurnBehaviour()
    {
    }

    public virtual string strStacked()
    {
        return stackable ? nrStacked.ToString() : "";
    }



}

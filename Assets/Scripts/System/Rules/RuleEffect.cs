using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RuleEffect
{
    public HealthEffects healthEffects;
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

    public virtual string strStacked()
    {
        return stackable ? nrStacked.ToString() : "";
    }



}

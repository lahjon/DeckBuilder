using System;

public abstract class RuleEffect
{
    public CombatActor actor;
    public string effectName { get { return GetType().ToString().Substring(10); } }
    public abstract bool isBuff { get; }
    public virtual bool triggerRecalcDamage { get { return false; } }

    public EffectType type
    {
        get
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

    public virtual void OnNewTurn()
    {
        nrStacked--;
    }

    public virtual void OnEndTurn()
    {
    }

    public virtual void OnActorDeath()
    {

    }


    public virtual string strStacked()
    {
        return stackable ? nrStacked.ToString() : "";
    }



}

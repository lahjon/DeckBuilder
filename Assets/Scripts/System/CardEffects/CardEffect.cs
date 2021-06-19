using System;

public abstract class CardEffect
{
    public CombatActor actor;
    public CombatController combatController { get { return actor.combatController; } }
    public string effectName { get { return GetType().ToString().Substring(10); } }
    public abstract bool isBuff { get; }
    public virtual bool triggerRecalcDamage { get { return false; } }
    public virtual bool stackable { get { return true; } }

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

    public virtual void RecieveInput(CardEffectInfo effect)
    {
        if (effect.Times == 0 || effect.Value == 0) return;

        RecieveInput(effect.Times * effect.Value);
    }

    public virtual void RecieveInput(int stackUpdate)
    {
        if (stackUpdate == 0) return;

        int nrStackedPre = nrStacked;

        nrStacked += stackUpdate;

        actor.healthEffectsUI.UpdateEffectUI(this);

        RespondStackUpdate(stackUpdate);

        if (nrStackedPre == 0 && nrStacked != 0)
        {
            AddFunctionToRules();
            if (triggerRecalcDamage) combatController.RecalcAllCardsDamage();
        }

        if (nrStacked == 0) Dismantle();
    }

    public virtual void AddFunctionToRules()
    {
    }

    public virtual void RemoveFunctionFromRules()
    {
    }

    public virtual void RespondStackUpdate(int update)
    {
    }

    public virtual void OnNewTurn()
    {
        RecieveInput(-1);
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

    public virtual void Dismantle()
    {
        RemoveFunctionFromRules();
        if (triggerRecalcDamage) combatController.RecalcAllCardsDamage();
        actor.effectTypeToRule.Remove(type);
    }


}

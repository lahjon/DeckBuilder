using System;

public abstract class RuleEffect
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

    public virtual void RecieveInput(CardEffect effect)
    {
        if (effect.Times == 0 || effect.Value == 0) return;

        int nrStackedPre = nrStacked;

        nrStacked += effect.Times * effect.Value;

        actor.healthEffectsUI.UpdateEffectUI(this);

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

    public virtual void OnNewTurn()
    {
        nrStacked--;
        actor.healthEffectsUI.ModifyEffectUI(this);

        if (nrStacked == 0) Dismantle();
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
        actor.effectTypeToRule.Remove(this.type);
    }


}

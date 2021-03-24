using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TokenSuperSword : Token
{
    public override void Init()
    {
        base.Init();
    }

    public override void AddActivity()
    {
        WorldSystem.instance.combatManager.combatController.OnCombatStart.Add(GetThornsOnStart);
    }

    public override void RemoveActivity()
    {
        Debug.Log("Remove Activity");
    }

    void GetThornsOnStart()
    {
        CardEffect thorns = new CardEffect(EffectType.Thorns, 3,1, CardTargetType.Self);
        WorldSystem.instance.combatManager.combatController.Hero.healthEffects.RecieveEffectNonDamageNonBlock(thorns);
    }

}

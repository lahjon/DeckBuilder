using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardActivityExhaustDiscard : CardActivity
{

    public override IEnumerator Execute(CardActivitySetting data)
    {
        CombatActor target = CombatSystem.instance.InProcessTarget;
        
        for(int i = 0; i < data.val && target.discard.Count >= 0; i++)
        {
            Card card = target.discard[0];
            target.discard.RemoveAt(0);
            CombatActor.Destroy(card.gameObject);
            if(target == CombatSystem.instance.Hero)
            {
                WorldSystem.instance.uiManager.UIWarningController.CreateWarning("Top in discard was exhausted!");
                CombatSystem.instance.UpdateDeckTexts();
            }
        }

        yield return null;
    }

    public override string GetDescription(CardActivitySetting data)
    {
        return "Exhaust " + data.val + " in the targets discard.";
    }

    public override string GetToolTip(CardActivitySetting data)
    {
        return $"When you play this card, the top  " + (data.val == 1 ? " card " : data.val + " cards") + " in the targets discard will be exhausted";
    }
}

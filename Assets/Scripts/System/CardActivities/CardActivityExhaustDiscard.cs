using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardActivityExhaustDiscard : CardActivity
{

    public override IEnumerator Execute(string input)
    {
        int x = Int32.Parse(input);

        CombatActor target = combatController.InProcessTarget;
        
        for(int i = 0; i < x && target.discard.Count >= 0; i++)
        {
            Card card = target.discard[0];
            target.discard.RemoveAt(0);
            CombatActor.Destroy(card.gameObject);
            if(target == combatController.Hero)
            {
                WorldSystem.instance.uiManager.UIWarningController.CreateWarning("Top in discard was exhausted!");
                combatController.UpdateDeckTexts();
            }
        }

        yield return null;
    }

    public override string GetDescription(string input)
    {
        return "Exhaust " + input + " in the targets discard.";
    }

    public override string GetToolTip(string input)
    {
        return $"When you play this card, the top  " + (input == "1" ? " card " : input + " cards") + " in the targets discard will be exhausted";
    }
}

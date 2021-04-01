using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardActivityExhaustDiscard : CardActivity
{

    public override IEnumerator Execute(string input)
    {
        int x = Int32.Parse(input);

         CombatActor target = combatController.CardInProcess.target;
        
        if(target == combatController.Hero)
        {
            for(int i = 0; i < x && combatController.Discard.Count != 0; i++)
            {
                CardCombat card = combatController.Discard[combatController.Discard.Count -1];
                combatController.Discard.Remove(card);
                CombatController.Destroy(card.gameObject);
                WorldSystem.instance.uiManager.UIWarningController.CreateWarning("Top in discard was exhausted!");
                combatController.UpdateDeckTexts();
            }
        }
        else {
            CombatActorEnemy enemy = (CombatActorEnemy)target;
            for (int i = 0; i < x && enemy.discard.Count != 0; i++)
            {
                Card card = enemy.discard[enemy.discard.Count-1];
                enemy.discard.Remove(card);
                CombatController.Destroy(card.gameObject);
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

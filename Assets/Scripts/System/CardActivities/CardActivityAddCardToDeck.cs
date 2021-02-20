using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CardActivityAddCardToDeck : CardActivity
{
    public override IEnumerator Execute(string input)
    {
        Debug.Log("Starting AddCard");
        CardData cd = DatabaseSystem.instance.cardDatabase.allCards.Where(x => x.name == input).FirstOrDefault();
        if (cd is null)
        {
            Debug.LogError($"No such card named {input}");
            yield return null;
        }

        CardCombatAnimated card = CardCombatAnimated.CreateCardFromData(cd, combatController);
        combatController.Deck.Add(card);
        WorldSystem.instance.uiManager.UIWarningController.CreateWarning($"Added card {input} to Deck!");
        combatController.UpdateDeckTexts();
    }

    public override string GetDescription(string input)
    {
        return $"Add a <b>{input}</b> to your deck.";
    }
}

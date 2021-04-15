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
        }
        else
        {
            CardCombat card = CardCombat.CreateCardCombatFromData(cd);
            card.owner.AddToDeck(card);
            WorldSystem.instance.uiManager.UIWarningController.CreateWarning($"Added card {card.cardName} to Deck!");
            combatController.UpdateDeckTexts();
        }
        yield return null;
    }

    public override string GetDescription(string input)
    {
        return $"Add a <b>{input}</b> to your deck.";
    }

    public override string GetToolTip(string input)
    {
        return $"When you play this card, a <b>{input}</b> will be added to your deck.";
    }
}

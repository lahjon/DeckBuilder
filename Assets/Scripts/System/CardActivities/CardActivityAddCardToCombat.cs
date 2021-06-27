using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class CardActivityAddCardToCombat : CardActivity
{
    string databaseName;
    public override IEnumerator Execute(string input)
    {
        //Debug.Log("Starting AddCard");
        string[] inputSplit = input.Split(';');
        List<string> cardNames = new List<string>();
        List<CardCombat> cards = new List<CardCombat>();
        List<CardLocation> targetLocations = new List<CardLocation>();

        foreach(string s in inputSplit)
        {
            string[] data = s.Split(',');
            cardNames.Add(data[0]);
            CardLocation locale;
            Enum.TryParse(data[1], out locale);
            targetLocations.Add(locale);
        }

        List<CardData> cd = new List<CardData>();
        cardNames.ForEach(c => cd.Add(DatabaseSystem.instance.cardDatabase.allCards.Where(x => c == x.name).First()));

        cd.ForEach(d => cards.Add(CardCombat.CreateCardCombatFromData(d)));
        combatController.cardPresenter.DisplayCards(cards, targetLocations);

        yield return null;
    }

    public override string GetDescription(string input)
    {
        databaseName = "poo"; // DatabaseSystem.instance.cardDatabase.allCards.Where(x => x.name == input).FirstOrDefault().cardName;
        return $"Add a <b>{databaseName}</b> to your deck.";
    }

    public override string GetToolTip(string input)
    {
        return $"When you play this card, a <b>{databaseName}</b> will be added to your deck.";
    }
}

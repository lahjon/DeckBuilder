using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class CardActivityAddCardToCombat : CardActivity
{
    string databaseName;
    public override IEnumerator Execute(CardActivitySetting data)
    {
        //Debug.Log("Starting AddCard");
        string[] inputSplit = data.strParameter.Split(';');
        List<string> cardNames = new List<string>();
        List<CardCombat> cards = new List<CardCombat>();
        List<CardLocation> targetLocations = new List<CardLocation>();

        foreach(string s in inputSplit)
        {
            string[] cardData = s.Split(',');
            cardNames.Add(cardData[0]);
            CardLocation locale;
            Enum.TryParse(cardData[1], out locale);
            targetLocations.Add(locale);
        }

        List<CardData> cd = new List<CardData>();
        cd.AddRange(DatabaseSystem.instance.GetCardsByName(cardNames));
        cd.ForEach(d => cards.Add(CardCombat.Factory(d)));
        CombatSystem.instance.cardPresenter.DisplayCards(cards, targetLocations);

        yield return null;
    }

    public override string GetDescription(CardActivitySetting data)
    {
        databaseName = "poo"; // DatabaseSystem.instance.cardDatabase.allCards.Where(x => x.name == input).FirstOrDefault().cardName;
        return $"Add a <b>{databaseName}</b> to your deck.";
    }

    public override string GetToolTip(CardActivitySetting data)
    {
        return $"When you play this card, a <b>{databaseName}</b> will be added to your deck.";
    }
}

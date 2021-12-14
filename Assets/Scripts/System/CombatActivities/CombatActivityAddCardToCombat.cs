using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class CombatActivityAddCardToCombat : CombatActivity
{
    string databaseName;
    public override IEnumerator Execute(CombatActivitySetting data)
    {
        //Debug.Log("Starting AddCard");
        string[] inputSplit = data.strParameter.Split(';');
        List<string> ids = new List<string>();
        List<CardCombat> cards = new List<CardCombat>();
        List<CardLocation> targetLocations = new List<CardLocation>();

        foreach(string s in inputSplit)
        {
            string[] cardData = s.Split(',');
            ids.Add(cardData[0]);
            CardLocation locale;
            Enum.TryParse(cardData[1], out locale);
            targetLocations.Add(locale);
        }

        List<CardData> cd = new List<CardData>();
        foreach(string id in ids)
            cd.Add(DatabaseSystem.instance.cards.FirstOrDefault(c=> c.id == id));
        cd.ForEach(d => cards.Add(CardCombat.Factory(d)));
        CombatSystem.instance.cardPresenter.DisplayCards(cards, targetLocations);

        yield return null;
    }

    public override string GetDescription(CombatActivitySetting data)
    {
        databaseName = "poo"; // DatabaseSystem.instance.cardDatabase.allCards.Where(x => x.name == input).FirstOrDefault().cardName;
        return $"Add a <b>{databaseName}</b> to your deck.";
    }

    public override string GetToolTip(CombatActivitySetting data)
    {
        return $"When you play this card, a <b>{databaseName}</b> will be added to your deck.";
    }
}

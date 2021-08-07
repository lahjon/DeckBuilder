using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DatabaseSystem : MonoBehaviour
{
    public static DatabaseSystem instance;

    public List<StartingCardSet> StartingCards = new List<StartingCardSet>();

    public List<CardData> cards = new List<CardData>();

    public List<EncounterDataCombat> encountersCombat = new List<EncounterDataCombat>();
    public List<EncounterDataRandomEvent> encounterEvent = new List<EncounterDataRandomEvent>();
    public List<Sprite> allOverworldIcons = new List<Sprite>();

    public List<ArtifactData> artifacts = new List<ArtifactData>();


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public List<CardData> GetCardsByName(List<string> cardNames)
    {
        List<CardData> retList = new List<CardData>();

        Dictionary<string, int> counts = new Dictionary<string, int>();
        foreach(string name in cardNames)
        {
            if (counts.ContainsKey(name))
                counts[name]++;
            else
                counts[name] = 1;
        }

        foreach (CardData card in cards)
            if (counts.ContainsKey(card.name))
                for (int i = 0; i < counts[card.name];i++)
                    retList.Add(card);

        return retList;
    }

    public CardData GetRandomCard(CardClassType cardClass = CardClassType.None)
    {
        int idx;

        if(cardClass == CardClassType.None)
        {
            idx = Random.Range(0,cards.Count);
            return cards[idx];
        }
        else
        {
            List<CardData> classCards = cards.Where(x => x.cardClass == cardClass).ToList();
            idx = Random.Range(0, classCards.Count);
            return classCards[idx];
        }

    }

    public CardData GetRandomCard(CardFilter cardFilter)
    {
        int idx;

        if (cardFilter == null)
        {
            idx = Random.Range(0, cards.Count);
            return cards[idx];
        }
        else
        {
            List<CardData> classCards = cards.Where(x => CardFilter.Filterer(x, cardFilter)).ToList();
            if (classCards.Count == 0) return null;
            idx = Random.Range(0, classCards.Count);
            return classCards[idx];
        }
    }

    public Sprite GetOverWorldIcon(OverworldEncounterType type)
    {
        return allOverworldIcons.Where(x => x.name == "Overworld" + type.ToString()).First();
    }

    public List<CardData> GetStartingDeck(CharacterClassType character, Profession profession = Profession.Base)
    {
        Debug.Log(character + "  " +  profession);
        Debug.Log(StartingCards.Where(x => x.characterClass == character && x.profession == profession).Select(x => x.startingCards).FirstOrDefault().Count());
        return StartingCards.Where(x => x.characterClass == character && x.profession == profession).Select(x => x.startingCards).FirstOrDefault();
        //return StartingCards.Where(x => (x.characterClass == character && (x.profession == profession || x.profession == Profession.Base)).Select(x => x.startingCards).FirstOrDefault();
    }

    public EncounterDataCombat GetRndEncounterCombat(OverworldEncounterType type)
    {
        List<EncounterDataCombat> encounters = encountersCombat.Where(e => (int)e.type == (int)type).ToList();
        int id = Random.Range(0, encounters.Count);
        return encounters[id];
    }

    public EncounterDataRandomEvent GetRndEncounterEvent()
    {
        List<EncounterDataRandomEvent> encounters = encounterEvent.Where(e => e.FindInRandom).ToList();
        int id = Random.Range(0, encounters.Count);
        return encounters[id];
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DatabaseSystem : MonoBehaviour
{
    public CardDatabase cardDatabase;
    public CardDatabase StartingCardsBrute;
    public static DatabaseSystem instance;

    public List<EncounterDataCombat> encountersCombat = new List<EncounterDataCombat>();
    public List<EncounterDataRandomEvent> encounterEvent = new List<EncounterDataRandomEvent>();
    public List<Sprite> allOverworldIcons = new List<Sprite>();

    private Dictionary<string, CardDatabase> StartingCards = new Dictionary<string, CardDatabase>();

    private List<CardData> allCards { get { return cardDatabase.allCards; } }

    public HashSet<EffectType> effectsStackable = new HashSet<EffectType>();

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

        StartingCards["Brute"] = StartingCardsBrute;

    }


    public void FetchCards(List<CardData> allCards)
    {
        foreach(CardData card in allCards)
        {
            Debug.Log(card);
        }
        cardDatabase.UpdateDatabase(allCards);
    }
    public List<CardData> GetCardsByName(List<string> cardNames)
    {
        if (cardNames.Count < 1)
        {
            return null;
        }
        
        List<CardData> result = new List<CardData>();
        List<string> tempList = new List<string>();
        allCards.ForEach(x => tempList.Add(x.name));

        for (int i = 0; i < cardNames.Count; i++)
        {
            result?.Add(allCards[tempList.IndexOf(cardNames[i])]);
            // if (tempList.Contains(cardNames[i]))
            // {
            //     result.Add(allCards[tempList.IndexOf(cardNames[i])]);
            // }
        }

        return result;
    }

    public CardData GetRandomCard(CharacterClassType characterClass = CharacterClassType.None)
    {
        int idx;
        if(characterClass == CharacterClassType.None)
        {
            idx = Random.Range(0,allCards.Count);
            return allCards[idx];
        }
        else
        {
            List<CardData> classCards = allCards.Where(x => x.characterClass == characterClass).ToList();
            idx = Random.Range(0, classCards.Count);
            return classCards[idx];
        }
    }

    public Sprite GetOverWorldIcon(OverworldEncounterType type)
    {
        return allOverworldIcons.Where(x => x.name == "Overworld" + type.ToString()).First();
    }

    public List<CardData> GetStartingDeck(string Character = "Brute")
    {
        //Denn ska v�lja bara kort f�r relevant gubbe sen
        return StartingCards[Character].allCards;
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


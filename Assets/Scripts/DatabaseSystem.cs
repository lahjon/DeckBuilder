using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DatabaseSystem : MonoBehaviour
{
    public CardDatabase cardDatabase;
    public CardDatabase StartingCardsBrute;
    public IconDatabase iconDatabase;
    public EncounterDatabase EncounterDatabase;
    public static DatabaseSystem instance;

    private Dictionary<string, CardDatabase> StartingCards = new Dictionary<string, CardDatabase>();

    private List<CardData> allCards { get { return cardDatabase.allCards; } }

    public Dictionary<EffectType, int> effectEndOfTurnBehavior = new Dictionary<EffectType, int>();
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

    public void Start()
    {
        effectsStackable.Add(EffectType.Poison);
        effectsStackable.Add(EffectType.Weak);
        effectsStackable.Add(EffectType.Vurnerable);

        effectEndOfTurnBehavior[EffectType.Poison] = -1;
        effectEndOfTurnBehavior[EffectType.Weak] = -1;
        effectEndOfTurnBehavior[EffectType.Vurnerable] = -1;
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
            idx = Random.Range(0,allCards.Count);
            return allCards[idx];
    }

    public List<CardData> GetStartingDeck(string Character = "Brute")
    {
        //Denn ska v�lja bara kort f�r relevant gubbe sen
        return StartingCards[Character].allCards;
    }

    public EncounterData GetRandomEncounter()
    {
        int id = Random.Range(0, EncounterDatabase.allOverworld.Count);
        return EncounterDatabase.allOverworld[id];
    }

    public EncounterData GetRandomEncounterBoss()
    {
        int id = Random.Range(0, EncounterDatabase.bossEncounters.Count);
        return EncounterDatabase.bossEncounters[id];
    }
}


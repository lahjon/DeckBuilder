using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DatabaseSystem : MonoBehaviour
{
    public CardDatabase cardDatabase;
    public CardDatabase StartingCardsBrute;
    public IconDatabase iconDatabase;
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

    public CardData GetRandomCard()
    {
        int idx = Random.Range(0,allCards.Count);
        return allCards[idx];
    }

    public List<CardData> GetStartingDeck(string Character = "Brute")
    {
        //Denn ska v�lja bara kort f�r relevant gubbe sen
        return StartingCards[Character].allCards;
    }
}


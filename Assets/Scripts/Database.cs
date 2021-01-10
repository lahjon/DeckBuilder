using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Database : MonoBehaviour
{
    public CardDatabase cardDatabase;
    public static Database instance; 

    private List<Card> allCards { get { return cardDatabase.allCards; } }

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

    public void FetchCards(List<Card> allCards)
    {
        foreach(Card card in allCards)
        {
            Debug.Log(card);
        }
        cardDatabase.UpdateDatabase(allCards);
        
    }

    public Card GetRandomCard()
    {
        int idx = Random.Range(0,allCards.Count);
        return allCards[idx];
    }

    public List<Card> GetStartingDeck(string Character = "")
    {
        //Denn ska välja bara kort för relevant gubbe sen
        return allCards;
    }
}


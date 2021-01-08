using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Database : MonoBehaviour
{
    public CardDatabase cardDatabase;
    public static Database instance; 

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
        int idx = Random.Range(0,cardDatabase.allCards.Count);
        Debug.Log(idx);
        Debug.Log(cardDatabase.allCards.Count);
        return cardDatabase.allCards[idx];
    }
}


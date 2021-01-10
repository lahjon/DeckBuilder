using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CardManager : MonoBehaviour
{
    public GameObject TemplateCard;
    public GameObject canvasObject;
    public GameObject CardPanel;

    public Card cardType;

    public int HandSize = 10;
    public int DrawCount = 5;
    
    private List<Card> DeckData;
    private List<GameObject> Deck = new List<GameObject>();
    private List<GameObject> Hand = new List<GameObject>();
    private List<GameObject> Discard = new List<GameObject>();

    void Start()
    {
        //AddCard();
        DeckData = Database.instance.GetStartingDeck();
        DeckData.ForEach(x => Deck.Add(CreateCardFromData(x)));
        ShuffleDeck();
        DrawCards(DrawCount);
        DisplayHand();
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    //Behöver antagligen bytas till nåt med Horizontal group? vet ej. Måste googlas.
    private void DisplayHand()
    {
        for (int i = 0; i < Hand.Count; i++)
            Hand[i].transform.position = new Vector3(-400 + 100 * i, 75, 0);
    }

    //Denna måste ändras till att blanda in discard om korten är slut! 
    private void DrawCards(int x)
    {
        for(int i = 0; i < Mathf.Min(x, Deck.Count); i++)
        {
            Hand.Add(Deck[0]);
            Deck.RemoveAt(0);
        }
    }

    private void ShuffleDeck()
    {
        for(int i = 0; i < Deck.Count; i++)
        {
            GameObject temp = Deck[i];
            int index = Random.Range(i, Deck.Count);
            Deck[i] = Deck[index];
            Deck[index] = temp;
        }
    }

    GameObject CreateCardFromData(Card cardData)
    {
        GameObject aCard = Instantiate(TemplateCard, new Vector3(-10000, -10000, -10000), Quaternion.Euler(0, 0, 0)) as GameObject;
        aCard.transform.SetParent(CardPanel.transform, false);
        aCard.transform.localScale = new Vector3(.4f, .4f, .4f);
        CardDisplay cardDisplay = aCard.GetComponent<CardDisplay>();
        cardDisplay.card = cardData;
        return aCard;
    }

    GameObject AddCard()
    {
        GameObject aCard = Instantiate(TemplateCard, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0)) as GameObject;
        aCard.transform.SetParent(canvasObject.transform, false);
        aCard.transform.localScale = new Vector3(.4f, .4f, .4f);
        CardDisplay cardDisplay = aCard.GetComponent<CardDisplay>();
        cardDisplay.card = Database.instance.GetRandomCard();
        return aCard;
    }
}

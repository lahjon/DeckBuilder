using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CardManager : MonoBehaviour
{
    public GameObject TemplateCard;

    public Text txtDeck;
    public Text txtDiscard;
    public int HandSize = 10;
    public int DrawCount = 5;
    public float offset = 50;
    
    private List<CardData> DeckData;
    public List<GameObject> Deck = new List<GameObject>();
    public List<GameObject> Hand = new List<GameObject>();
    public List<GameObject> Discard = new List<GameObject>();

    [HideInInspector]
    public bool isCardSelected = false;

    void Start()
    {
        //AddCard();
        DeckData = DatabaseSystem.instance.GetStartingDeck();
        Debug.Log(DeckData.Count);
        DeckData.ForEach(x => Discard.Add(CreateCardFromData(x)));
        NextTurn();
    }
    // Update is called once per frame
    void Update()
    {
        /*
        Vector2 outCoordinates;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), Input.mousePosition, null, out outCoordinates);
        Debug.Log("Mouse:" + Input.mousePosition.x + "," + Input.mousePosition.y);
        Debug.Log(outCoordinates.x + "," + outCoordinates.y);
        Hand[2].GetComponent<RectTransform>().localPosition = outCoordinates;
        */
    }



    private void DisplayHand()
    {
        int n = Hand.Count;
        float width = GetComponent<RectTransform>().rect.width;
        float midPoint = width / 2;
        float localoffset = (n % 2 == 0) ? offset : 0;

        for (int i = 0; i < Hand.Count; i++)
        {
            Hand[i].transform.localPosition = new Vector3(midPoint + (i - n / 2) * 75 + localoffset, -75,0);
            Hand[i].SetActive(true);
        }

        ResetSiblingIndexes();
    }

    internal void ResetSiblingIndexes()
    {
        for (int i = 0; i < Hand.Count; i++)
            Hand[i].transform.SetSiblingIndex(i);
    }

    //Denna måste ändras till att blanda in discard om korten är slut! 
    private void DrawCards(int x)
    {
        for(int i = 0; i < x; i++)
        {
            if(Deck.Count == 0)
            {
                Deck.AddRange(Discard);
                Discard.Clear();
                ShuffleDeck();
            }

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

    GameObject CreateCardFromData(CardData cardData)
    {
        GameObject CardObject = Instantiate(TemplateCard, new Vector3(-10000, -10000, -10000), Quaternion.Euler(0, 0, 0)) as GameObject;
        CardObject.transform.SetParent(transform, false);
        CardObject.transform.localScale = new Vector3(.4f, .4f, .4f);
        Card Card = CardObject.GetComponent<Card>();
        Card.cardData = cardData;
        Card.cardManager = this;
        Card.UpdateDisplay();
        HideCard(CardObject);
        return CardObject;
    }

    internal void CancelCardSelection(GameObject gameObject)
    {
        int n = Hand.Count;
        float width = GetComponent<RectTransform>().rect.width;
        float midPoint = width / 2;
        float localoffset = (n % 2 == 0) ? offset : 0;

        int i = Hand.IndexOf(gameObject);
        Hand[i].transform.localPosition = new Vector3(midPoint + (i - n / 2) * 75 + localoffset, -75, 0);
        isCardSelected = false;

        ResetSiblingIndexes();
    }

    public void NextTurn()
    {
        Hand.ForEach(x => HideCard(x));
        Discard.AddRange(Hand);
        Hand.Clear();
        DrawCards(DrawCount);
        DisplayHand();
        Debug.Log("New turn started. Cards in Deck, Hand, Discard: " + Deck.Count + "," + Hand.Count + "," + Discard.Count);
        txtDeck.text = "Deck:\n" + Deck.Count;
        txtDiscard.text = "Discard:\n" + Discard.Count;
    }


    private void HideCard(GameObject gameObject)
    {
        gameObject.transform.position = new Vector3(-10000, -10000, -10000);
        gameObject.SetActive(false);
    } 


}

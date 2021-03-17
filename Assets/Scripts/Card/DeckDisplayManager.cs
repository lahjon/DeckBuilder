using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DeckDisplayManager : Manager
{
    public List<CardData> allCardsData;
    public GameObject cardPrefab;
    public RectTransform content;
    public Canvas canvas;
    public GameObject deckDisplay;
    public List<GameObject> allDisplayedCards;
    public GameObject startPos;
    public float height = 400.0f;
    public Vector3 offsetHorizontal = new Vector3(400,0,0);
    public Vector3 offsetVertical = new Vector3(0,400,0);
    public int rows = 4;
    private WorldState previousState;
    public Card selectedCard;
    [HideInInspector]
    public int siblingIndex;
    public GameObject scroller;
    public Vector3 previousPosition;
    public GameObject placeholderCard;
    public GameObject clickableArea;
    public GameObject backgroundPanel;

    protected override void Awake()
    {
        base.Awake(); 
        world.deckDisplayManager = this;
        canvas.gameObject.SetActive(true);
        backgroundPanel.SetActive(false);
        clickableArea.SetActive(false);
        deckDisplay.SetActive(false);
    }

    public void UpdateAllCards()
    {

        allCardsData = WorldSystem.instance.characterManager.playerCardsData;

        if(allCardsData.Count > allDisplayedCards.Count)
        {
            while (allCardsData.Count > allDisplayedCards.Count)
            {
                GameObject newCard = Instantiate(cardPrefab, Vector3.zero, Quaternion.Euler(0, 0, 0)) as GameObject;
                newCard.transform.SetParent(content.gameObject.transform);
                newCard.transform.localScale = new Vector3(1, 1, 1);
                allDisplayedCards.Add(newCard);
                newCard.GetComponent<Card>().cardData = allCardsData[allDisplayedCards.Count - 1];
                newCard.GetComponent<Card>().BindCardData();
            }
        }
        else if(allCardsData.Count < allDisplayedCards.Count)
        {
            while (allCardsData.Count < allDisplayedCards.Count)
            {   
                DestroyImmediate(allDisplayedCards[(allDisplayedCards.Count - 1)]);
                allDisplayedCards.RemoveAt(allDisplayedCards.Count - 1);
            }
        }
    }

    public void RemoveCardAtIndex(int index)
    {
        DestroyImmediate(allDisplayedCards[index]);
        allDisplayedCards.RemoveAt(index);
        Debug.Log(allDisplayedCards.Count);
        Debug.Log(index);
    }

    public void ResetCardDisplay()
    {
        // called from click in inspector
        if (selectedCard != null)
        {
            selectedCard.ResetCardPosition();
            selectedCard = null;
        }
    }
    // void UpdateDisplayArea()
    // {
    //     content.sizeDelta = new Vector2(0, height);
    // }

    // void UpdateDeckDisplay()
    // {
    //     allCardsData = WorldSystem.instance.characterManager.playerCardsData;
        
    //     for (int i = 0; i < allDisplayedCards.Count; i++)
    //     {
    //         allDisplayedCards[i].GetComponent<Card>().cardData = allCardsData[i];
    //         allDisplayedCards[i].GetComponent<Card>().BindCardData();
    //     }
    // }

    public void CloseDeckDisplay()
    {
        backgroundPanel.SetActive(false);
        clickableArea.SetActive(false);
        deckDisplay.SetActive(false);
        selectedCard = null;
    }

    public void ButtonClose()
    {
        WorldStateSystem.TriggerClear();
    }
    // public void DisplayNextCard(int direction)
    // {
        
    //     int index = allDisplayedCards.IndexOf(selectedCard.gameObject);
    //     Debug.Log(index);
    //     if(direction == 1) 
    //     {
    //         if(index != allDisplayedCards.Count - 1)
    //         {
    //             selectedCard.ResetCardPositionNext();
    //             placeholderCard.GetComponent<Card>().cardData = selectedCard.cardData;

    //         }
    //     }
    //     else if(direction == -1)
    //     {
    //         if(index != 0)
    //         {
    //             selectedCard.ResetCardPositionNext();
    //             placeholderCard.GetComponent<Card>().cardData = selectedCard.cardData;

    //         }
    //     }
    // }
}

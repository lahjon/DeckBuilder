using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class DeckDisplayManager : Manager
{
    public List<CardData> allCardsData;
    public GameObject cardPrefab;
    public RectTransform content;
    public Canvas canvas;
    public GameObject deckDisplay;
    public List<CardDisplay> allDisplayedCards;
    public CardVisual selectedCard;
    public ScrollRect scroller;
    public Vector3 previousPosition;
    public CardDisplay placeholderCard;
    public GameObject backgroundPanel;
    public Transform deckDisplayPos;
    public CardDisplay animatedCard;
    public TMP_Text titleText;

    protected override void Awake()
    {
        base.Awake(); 
        world.deckDisplayManager = this;
        canvas.gameObject.SetActive(true);
        backgroundPanel.SetActive(false);
        deckDisplay.SetActive(false);
    }

    public void UpdateAllCards()
    {
        allCardsData.Clear();
        WorldSystem.instance.characterManager.playerCardsData.ForEach(x => allCardsData.Add(x));

        if(allCardsData.Count > allDisplayedCards.Count)
        {
            while (allCardsData.Count > allDisplayedCards.Count)
            {
                CardDisplay newCard = Instantiate(cardPrefab,content.gameObject.transform).GetComponent<CardDisplay>();
                newCard.transform.SetParent(content.gameObject.transform);
                newCard.gameObject.SetActive(true);
                allDisplayedCards.Add(newCard);
            }
        }
        else if(allCardsData.Count < allDisplayedCards.Count)
        {
            while (allCardsData.Count < allDisplayedCards.Count)
            {   
                Destroy(allDisplayedCards[(allDisplayedCards.Count - 1)].gameObject);
                allDisplayedCards.RemoveAt(allDisplayedCards.Count - 1);
            }
        }

        for (int i = 0; i < allCardsData.Count; i++)
        {
            allDisplayedCards[i].cardData = allCardsData[i];
            allDisplayedCards[i].BindCardData();
            allDisplayedCards[i].BindCardVisualData();
        }
    }

    public void DisplayCard(CardVisual aCard)
    {
        if(selectedCard == null)
        {
            aCard.OnMouseExit();
            previousPosition = aCard.transform.position;
            selectedCard = aCard;
            placeholderCard.cardData = selectedCard.cardData;
            placeholderCard.BindCardData();
            placeholderCard.BindCardVisualData();
            backgroundPanel.SetActive(true);
            scroller.enabled = false;
            selectedCard.transform.position = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0.1f);
        }
        else
        {
            ResetCardPosition();
        }
    }
    public void ResetCardPosition()
    {
        backgroundPanel.SetActive(false);
        scroller.enabled = true;
        selectedCard.transform.position = previousPosition;
        previousPosition = transform.position;
        selectedCard = null;
    }

    public void RemoveCardAtIndex(int index)
    {
        Destroy(allDisplayedCards[index]);
        allDisplayedCards.RemoveAt(index);
    }

    // public void ResetCardDisplay()
    // {
    //     if (selectedCard != null)
    //     {
    //         ResetCardPosition();
    //     }
    // }

    public void CloseDeckDisplay()
    {
        WorldStateSystem.TriggerClear();
        backgroundPanel.SetActive(false);
        deckDisplay.SetActive(false);
        selectedCard = null;
    }

    public void ButtonClose()
    {
        world.deckDisplayManager.CloseDeckDisplay();
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

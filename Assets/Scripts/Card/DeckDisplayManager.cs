using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DeckDisplayManager : MonoBehaviour
{
    public List<Card> allCards;
    public List<CardData> allCardsData;
    public GameObject cardPrefab;
    public RectTransform content;
    public Canvas canvas;
    public List<GameObject> createdObjects;
    public GameObject startPos;
    public float height = 400.0f;
    public Vector3 offsetHorizontal = new Vector3(400,0,0);
    public Vector3 offsetVertical = new Vector3(0,400,0);
    public int rows = 4;
    private WorldState previousState;
    public Card selectedCard;
    [HideInInspector]
    public Vector3 previousPosition;
    public int siblingIndex;
    public GameObject placeholder;
    public Canvas tempCanvas;

    void UpdateAllCards()
    {
        allCardsData = WorldSystem.instance.characterManager.playerCardsData;

        if(allCardsData.Count > createdObjects.Count)
        {
            while (allCardsData.Count > createdObjects.Count)
            {
                GameObject newCard = Instantiate(cardPrefab, Vector3.zero, Quaternion.Euler(0, 0, 0)) as GameObject;
                newCard.transform.SetParent(content.gameObject.transform);
                newCard.transform.localScale = new Vector3(1, 1, 1);
                createdObjects.Add(newCard);
            }
        }
        else if(allCardsData.Count < createdObjects.Count)
        {
            while (allCardsData.Count < createdObjects.Count)
            {   
                DestroyImmediate(createdObjects[(createdObjects.Count - 1)]);
                createdObjects.RemoveAt(createdObjects.Count - 1);
            }
        }
        UpdateDeckDisplay();
    }

    public void ResetCardDisplay()
    {
        if (selectedCard != null)
        {
            selectedCard.ResetCardPosition();
            selectedCard = null;
        }
    }
    void UpdateDisplayArea()
    {
        content.sizeDelta = new Vector2(0, height);
    }

    void UpdateDeckDisplay()
    {
        allCardsData = WorldSystem.instance.characterManager.playerCardsData;
        
        for (int i = 0; i < createdObjects.Count; i++)
        {
            createdObjects[i].GetComponent<Card>().cardData = allCardsData[i];
            createdObjects[i].GetComponent<Card>().UpdateDisplay();
        }
    }

    public void DisplayDeck()
    {
        if(!this.gameObject.activeSelf)
        {
            previousState = WorldSystem.instance.worldState;
            WorldSystem.instance.worldState = WorldState.Display;
            this.gameObject.SetActive(true);
            UpdateAllCards();
            
        }         
        else
        {
            WorldSystem.instance.worldState = previousState;
            this.gameObject.SetActive(false);
        }
    }


}

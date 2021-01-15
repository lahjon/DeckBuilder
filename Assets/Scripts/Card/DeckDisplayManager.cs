using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DeckDisplayManager : MonoBehaviour
{
    public List<Card> allCards;
    public GameObject cardPrefab;
    public RectTransform content;
    public Canvas canvas;
    public List<GameObject> createdObjects;
    public GameObject startPos;
    public float height = 400.0f;
    public Vector3 offsetHorizontal = new Vector3(400,0,0);
    public Vector3 offsetVertical = new Vector3(0,400,0);
    public int rows = 4;

    void UpdateAllCards()
    {
        allCards = WorldSystem.instance.characterManager.playerCards;

        if(allCards.Count > createdObjects.Count)
        {
            while (allCards.Count > createdObjects.Count)
            {
                GameObject newCard = Instantiate(cardPrefab, new Vector3(50,-50,0), Quaternion.Euler(0, 0, 0)) as GameObject;
                newCard.transform.SetParent(content.gameObject.transform);
                newCard.transform.localScale = new Vector3(1, 1, 1);
                //Vector3 pos = startPos.transform.position + (offsetHorizontal * (createdObjects.Count % rows)) + (offsetVertical * (createdObjects.Count / rows));
                //newCard.transform.position = pos;
                createdObjects.Add(newCard);
            }
        }
        else if(allCards.Count < createdObjects.Count)
        {
            while (allCards.Count < createdObjects.Count)
            {   
                DestroyImmediate(createdObjects[(createdObjects.Count - 1)]);
                createdObjects.RemoveAt(createdObjects.Count - 1);
            }
        }
        UpdateDeckDisplay();
    }

    void UpdateDisplayArea()
    {
        content.sizeDelta = new Vector2(0, height);
    }

    void UpdateDeckDisplay()
    {
        for (int i = 0; i < createdObjects.Count; i++)
        {
            createdObjects[i].GetComponent<Card>().cardData = allCards[i].cardData;
            createdObjects[i].GetComponent<Card>().UpdateDisplay();
        }
    }

    public void ToggleCanvas()
    {
        if(!this.gameObject.activeSelf)
        {
            this.gameObject.SetActive(true);
            UpdateAllCards();
            
        }
            
        else
        {
            this.gameObject.SetActive(false);
        }
    }


}

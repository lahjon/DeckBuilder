using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public GameObject myCard;
    public GameObject canvasObject;
    public Card cardType;

    void Start()
    {
        AddCard();  
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    GameObject AddCard()
    {
        GameObject aCard = Instantiate(myCard, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0)) as GameObject;
        aCard.transform.SetParent(canvasObject.transform, false);
        aCard.transform.localScale = new Vector3(.4f, .4f, .4f);
        CardDisplay cardDisplay = aCard.GetComponent<CardDisplay>();
        cardDisplay.card = Database.instance.GetRandomCard();
        return aCard;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : Card
{
    private float startDragPos;

    private DeckDisplayManager deckDisplayManager;

    void Awake()
    {
        deckDisplayManager = WorldSystem.instance.deckDisplayManager;
    }

    public override void OnMouseEnter()
    {
        transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
    }

    public override void OnMouseExit()
    {
        transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);
    }

    public override void ResetScale()
    {
        transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);
    }
    public override void OnMouseClick()
    {
        switch (WorldSystem.instance.worldState)
        {
            case WorldState.Shop:

                WorldSystem.instance.shopManager.shop.PurchaseCard(this);
                break;

            case WorldState.Display:

                DisplayCard();
                break;

            default:
                break;
        }
    }
    public void DisplayCard()
    {
        if(deckDisplayManager.selectedCard == null)
        {
            deckDisplayManager.previousPosition = transform.position;
            deckDisplayManager.selectedCard = this;
            deckDisplayManager.placeholderCard.GetComponent<Card>().cardData = deckDisplayManager.selectedCard.cardData;
            deckDisplayManager.placeholderCard.GetComponent<Card>().BindCardData();
            deckDisplayManager.backgroundPanel.SetActive(true);
            deckDisplayManager.clickableArea.SetActive(true);
            deckDisplayManager.scroller.GetComponent<ScrollRect>().enabled = false;
            transform.localPosition = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0.1f);
        }
        else
        {
            ResetCardPosition();
        }
    }
        public void OnMouseScroll()
    {
        if(WorldSystem.instance.worldState == WorldState.Display)
        {
            float sensitivity = deckDisplayManager.scroller.GetComponent<ScrollRect>().scrollSensitivity;
            Vector2 scrollPos = new Vector2(0, Input.mouseScrollDelta.y * sensitivity * -1);
            deckDisplayManager.content.GetComponent<RectTransform>().anchoredPosition += scrollPos;
        }
    }

    public void OnMouseBeginDrag()
    {
        if(WorldSystem.instance.worldState == WorldState.Display)
        {
            startDragPos = Input.mousePosition.y;
        }
    }
    public void OnMouseDrag()
    {
        if(WorldSystem.instance.worldState == WorldState.Display)
        {
            float sensitivity = deckDisplayManager.scroller.GetComponent<ScrollRect>().scrollSensitivity;
            float currentPos = Input.mousePosition.y;
            float direction;
            if(currentPos > startDragPos)
                direction = -1;
            else if(currentPos < startDragPos)
                direction = 1;
            else
                direction = 0;
            Vector2 scrollPos = new Vector2(0, direction * sensitivity * 0.3f * -1);
            deckDisplayManager.content.GetComponent<RectTransform>().anchoredPosition += scrollPos;
        }
    }

    public void ResetCardPosition()
    {
        deckDisplayManager.backgroundPanel.SetActive(false);
        deckDisplayManager.clickableArea.SetActive(false);
        deckDisplayManager.scroller.GetComponent<ScrollRect>().enabled = true;
        deckDisplayManager.selectedCard.transform.position = deckDisplayManager.previousPosition;
        deckDisplayManager.previousPosition = transform.position;
        deckDisplayManager.selectedCard = null;
    }
    public void ResetCardPositionNext()
    {
        deckDisplayManager.selectedCard.transform.position = deckDisplayManager.previousPosition;
        deckDisplayManager.previousPosition = Vector3.zero;
        deckDisplayManager.selectedCard = null;
    }

    public override void OnMouseRightClick()
    {
        return;
    }

}

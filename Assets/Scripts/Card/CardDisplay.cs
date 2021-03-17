using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : Card
{
    private float startDragPos;
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
        if(WorldStateSystem.instance.currentOverlayState == OverlayState.Display)
        {
            DisplayCard();
        }
        else if (WorldStateSystem.instance.currentWorldState == WorldState.Shop)
        {
            WorldSystem.instance.shopManager.shop.PurchaseCard(this);
        }
        else if (WorldStateSystem.instance.currentWorldState == WorldState.Reward)
        {
            AddCardToDeck(this.cardData);
            WorldSystem.instance.uiManager.rewardScreen.rewardScreenCard.SetActive(false);
            WorldSystem.instance.uiManager.rewardScreen.ResetCurrentReward();
        }
    }

    public override void OnMouseRightClick(bool allowDisplay = true)
    {
        if (WorldStateSystem.instance.currentOverlayState == OverlayState.Display || WorldStateSystem.instance.currentWorldState != WorldState.Combat)
        {
            DisplayCard();
        }
    }

    public void OnMouseScroll()
    {
        if(WorldStateSystem.instance.currentOverlayState == OverlayState.Display && WorldSystem.instance.deckDisplayManager.selectedCard == null)
        {
            float sensitivity = WorldSystem.instance.deckDisplayManager.scroller.GetComponent<ScrollRect>().scrollSensitivity;
            Vector2 scrollPos = new Vector2(0, Input.mouseScrollDelta.y * sensitivity * -1);
            WorldSystem.instance.deckDisplayManager.content.GetComponent<RectTransform>().anchoredPosition += scrollPos;
        }
    }

    public void AddCardToDeck(CardData cardData, bool callRewardScreen = false)
    {
        WorldSystem.instance.characterManager.playerCardsData.Add(cardData);

        if(callRewardScreen == true)
        {
            WorldSystem.instance.uiManager.rewardScreen.currentReward.OnClick();
        }
    }

    public void OnMouseBeginDrag()
    {
        if(WorldStateSystem.instance.currentOverlayState == OverlayState.Display)
        {
            startDragPos = Input.mousePosition.y;
        }
    }
    public void OnMouseDrag()
    {
        if(WorldStateSystem.instance.currentOverlayState == OverlayState.Display && WorldSystem.instance.deckDisplayManager.selectedCard == null)
        {
            float sensitivity = WorldSystem.instance.deckDisplayManager.scroller.GetComponent<ScrollRect>().scrollSensitivity;
            float currentPos = Input.mousePosition.y;
            float direction;
            if(currentPos > startDragPos)
                direction = -1;
            else if(currentPos < startDragPos)
                direction = 1;
            else
                direction = 0;
            Vector2 scrollPos = new Vector2(0, direction * sensitivity * 0.3f * -1);
            WorldSystem.instance.deckDisplayManager.content.GetComponent<RectTransform>().anchoredPosition += scrollPos;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : Card
{
    private float startDragPos;

    public DeckDisplayManager deckDisplayManager;

    void Start()
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

            case WorldState.Reward:
                    AddCardToDeck(this.cardData);
                    WorldSystem.instance.uiManager.rewardScreen.ResetCurrentReward();
                    WorldSystem.instance.SwapState(WorldState.Overworld);
                break;

            default:
                break;
        }
    }

    public override void OnMouseRightClick(bool allowDisplay = true)
    {
        switch (WorldSystem.instance.worldState)
        {
            case WorldState.Shop:

                DisplayCard();
                break;

            case WorldState.Display:

                DisplayCard();
                break;

            case WorldState.Reward:
                DisplayCard();
                break;

            default:
                break;
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
}

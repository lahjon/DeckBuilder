using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : CardVisual
{
    private float startDragPos;
    public bool disable;
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
        if(WorldStateSystem.instance.currentOverlayState == OverlayState.Display && !disable)
        {
            DisplayCard();
        }
        else if (WorldStateSystem.instance.currentWorldState == WorldState.Shop && !disable)
        {
            WorldSystem.instance.deckDisplayManager.StartCoroutine(AnimateCardToDeck());
            WorldSystem.instance.shopManager.shop.PurchaseCard(this);
        }
        else if (WorldStateSystem.instance.currentWorldState == WorldState.Reward && !disable)
        {
            WorldSystem.instance.deckDisplayManager.StartCoroutine(AnimateCardToDeck());
            RewardCallback();
        }
    }

    void RewardCallback()
    {
        AddCardToDeck(this.cardData);
        if (WorldSystem.instance.rewardManager.draftAmount > 0)
        {
            WorldSystem.instance.rewardManager.draftAmount--;
            WorldSystem.instance.rewardManager.OpenDraftMode();
        }
        else
        {
            WorldSystem.instance.rewardManager.rewardScreen.ResetCurrentReward();
        }
    }

    IEnumerator AnimateCardToDeck()
    {
        Vector3 posEnd = WorldSystem.instance.deckDisplayManager.deckDisplayPos.position;
        Vector3 posStart = transform.position;

        GameObject animateCard = WorldSystem.instance.deckDisplayManager.animatedCard.gameObject;
        animateCard.SetActive(true);

        animateCard.GetComponent<CardDisplay>().cardData = cardData;
        animateCard.GetComponent<CardDisplay>().BindCardData();
        animateCard.GetComponent<CardDisplay>().BindCardVisualData();

        Vector3 newScale = new Vector3(1.1f, 1.1f, 1.1f);

        float elapsedTime = 0.0f;
        float waitTime = 0.3f;

        while (elapsedTime < waitTime)
        {
            animateCard.transform.position = Vector3.Lerp(posStart, posEnd, (elapsedTime / waitTime));
            animateCard.transform.localScale = Vector3.Lerp(newScale, new Vector3(0.1f, 0.1f, 0.1f), (elapsedTime / waitTime));
            elapsedTime += Time.deltaTime;
            if (Vector3.Distance(animateCard.transform.position, posEnd) < 1.0f)
            {
                break;
            }
            yield return null;
        }  
        animateCard.SetActive(false);
        //Callback();
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
            WorldSystem.instance.rewardManager.rewardScreen.currentReward.OnClick();
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

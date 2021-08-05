using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardDisplay : CardVisual
{
    private float startDragPos;
    bool dragging;
    public System.Action clickCallback;

    void OnEnable()
    {
        transform.localScale = Vector3.one;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (transform.localScale.x <= 1f)
            transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
    }
    

    public override void OnPointerExit(PointerEventData eventData)
    {

        if (transform.localScale.x >= 1f)
            transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);
        
    }

    public override void ResetScale()
    {
        transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);
    }
    public override void OnMouseClick()
    {
        base.OnMouseClick();
        if (clickCallback != null)
        {
            //WorldSystem.instance.characterManager.playerCardsData.Add(cardData);
            clickCallback.Invoke();
        }
        else if(WorldStateSystem.instance.currentOverlayState == OverlayState.Display)
        {
            WorldSystem.instance.deckDisplayManager.DisplayCard(this);
        }
        else if (WorldStateSystem.instance.currentWorldState == WorldState.Shop)
        {
            bool success = WorldSystem.instance.shopManager.shop.PurchaseCard(this);

            if (success)
            {
                WorldSystem.instance.deckDisplayManager.StartCoroutine(AnimateCardToDeck());
            }
        }
        else if (WorldStateSystem.instance.currentWorldState == WorldState.Reward)
        {
            WorldSystem.instance.deckDisplayManager.StartCoroutine(AnimateCardToDeck());
            RewardCallback();
        }
    }

    void RewardCallback()
    {
        WorldSystem.instance.characterManager.playerCardsData.Add(cardData);
        if (WorldSystem.instance.rewardManager.draftAmount > 0)
        {
            WorldSystem.instance.rewardManager.draftAmount--;
            WorldSystem.instance.rewardManager.OpenDraftMode();
        }
        else
        {
            WorldSystem.instance.rewardManager.rewardScreenCombat.ResetReward();
            WorldSystem.instance.rewardManager.rewardScreenCardSelection.canvas.SetActive(false);
        }
    }

    public IEnumerator AnimateCardToDeck()
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
        float waitTime = 0.15f;

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
        if (WorldStateSystem.instance.currentOverlayState == OverlayState.Display || WorldStateSystem.instance.currentWorldState == WorldState.Reward)
        {
            WorldSystem.instance.deckDisplayManager.DisplayCard(this);
        }
    }

    public void OnMouseScroll()
    {
        if(WorldStateSystem.instance.currentOverlayState == OverlayState.Display && WorldSystem.instance.deckDisplayManager.selectedCard == null)
        {
            float sensitivity = WorldSystem.instance.deckDisplayManager.scroller.GetComponent<ScrollRect>().scrollSensitivity;
            Vector2 scrollPos = new Vector2(0, Input.mouseScrollDelta.y * sensitivity * 7.0f * -1);
            WorldSystem.instance.deckDisplayManager.content.GetComponent<RectTransform>().anchoredPosition += scrollPos;
        }
    }

    public void AddCardToDeck(CardData cardData, bool callRewardScreen = false)
    {
        
    }

    public void OnMouseBeginDrag()
    {
        if(WorldStateSystem.instance.currentOverlayState == OverlayState.Display)
        {
            startDragPos = Input.mousePosition.y;
            dragging = true;
        }
    }

    public void OnMouseEndDrag()
    {
        if(WorldStateSystem.instance.currentOverlayState == OverlayState.Display)
        {
            dragging = false;
        }
    }
    public void OnMouseDrag()
    {
        if(dragging && WorldStateSystem.instance.currentOverlayState == OverlayState.Display && WorldSystem.instance.deckDisplayManager.selectedCard == null)
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

            Vector2 scrollPos = new Vector2(0, direction * sensitivity * 5.0f * -1);
            WorldSystem.instance.deckDisplayManager.content.GetComponent<RectTransform>().anchoredPosition += scrollPos;
        }
    }
}

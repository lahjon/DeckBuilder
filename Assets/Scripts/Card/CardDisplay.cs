using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CardDisplay : CardVisual
{
    public int idx;
    public System.Action clickCallback;
    Tween tween;
    public GameObject nonSelectableImage;
    public bool _selectable;
    public bool selectable
    {
        get => _selectable;
        set
        {
            _selectable = value;
            if (_selectable) nonSelectableImage.SetActive(false);
            else nonSelectableImage.SetActive(true);
        }
    }
    bool _selected;
    public bool selected
    {
        get => _selected;
        set
        {
            _selected = value;
            if (_selected)
                cardHighlightType = CardHighlightType.Selected;
            else
                cardHighlightType = CardHighlightType.None;

        }
    }

    void OnEnable()
    {
        transform.localScale = Vector3.one;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (transform.localScale.x <= 1f)
            transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
    }
    

    public override void OnPointerExit(PointerEventData eventData)
    {

        if (transform.localScale.x >= 1f)
            transform.localScale = new Vector3(1f, 1f, 1f);
        
    }

    public void SelectCard()
    {
        int maxAmount = CombatSystem.instance.combatDeckDisplay.selectAmount;
        int currentAmount = CombatSystem.instance.combatDeckDisplay.selectedCards.Count;

        if (selected) 
        {
            DeselectCard();
            return;
        }
        if (currentAmount >= maxAmount) return;

        selected = true;
        CombatSystem.instance.combatDeckDisplay.AddCard(this);

        tween = transform.DOScale(transform.localScale + new Vector3(0.05f, 0.05f, 0.05f), .1f).SetLoops(1, LoopType.Yoyo).OnComplete(
            () => transform.localScale = Vector3.one
        ).OnKill(
            () => transform.localScale = Vector3.one
        );
    }
    public void DeselectCard()
    {
        tween = transform.DOScale(Vector3.one, .1f).SetLoops(1, LoopType.Yoyo).OnComplete(
            () => transform.localScale = Vector3.one
        ).OnKill(
            () => transform.localScale = Vector3.one
        );
        CombatSystem.instance.combatDeckDisplay.RemoveCard(this);
        selected = false;
    }

    public override void ResetScale()
    {
        transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);
    }
    public override void OnMouseClick()
    {
        base.OnMouseClick();
        Debug.Log("Clicky?");
        clickCallback?.Invoke();
    }

    void ShopCallback()
    {
        if (WorldSystem.instance.shopManager.shop.PurchaseCard(this)) WorldSystem.instance.deckDisplayManager.StartCoroutine(AnimateCardToDeck());
    }

    public void RewardCallback()
    {
        WorldSystem.instance.deckDisplayManager.StartCoroutine(AnimateCardToDeck());

        WorldSystem.instance.characterManager.playerCardsData.Add(cardData);

        WorldSystem.instance.rewardManager.rewardScreenCombat.ResetReward();
        WorldSystem.instance.rewardManager.rewardScreenCardSelection.canvas.SetActive(false);
        
    }

    public IEnumerator AnimateCardToDeck()
    {
        Vector3 posEnd = WorldSystem.instance.deckDisplayManager.deckDisplayPos.position;
        Vector3 posStart = transform.position;

        CardDisplay animateCard = WorldSystem.instance.deckDisplayManager.animatedCard.GetComponent<CardDisplay>();
        animateCard.gameObject.SetActive(true);

        animateCard.cardData = cardData;
        animateCard.BindCardData();
        animateCard.BindCardVisualData();

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
        animateCard.gameObject.SetActive(false);
    }

    public override void OnMouseRightClick(bool allowDisplay = true)
    {
        // if (WorldStateSystem.instance.currentOverlayState == OverlayState.Display || WorldStateSystem.instance.currentWorldState == WorldState.Reward)
        // {
        WorldSystem.instance.deckDisplayManager.DisplayCard(this);
        // }
    }

    // public void OnMouseScroll()
    // {
    //     if(WorldStateSystem.instance.currentOverlayState == OverlayState.Display && WorldSystem.instance.deckDisplayManager.selectedCard == null)
    //     {
    //         float sensitivity = WorldSystem.instance.deckDisplayManager.scroller.GetComponent<ScrollRect>().scrollSensitivity;
    //         Vector2 scrollPos = new Vector2(0, Input.mouseScrollDelta.y * sensitivity * 7.0f * -1);
    //         WorldSystem.instance.deckDisplayManager.content.GetComponent<RectTransform>().anchoredPosition += scrollPos;
    //     }
    // }

    public void AddCardToDeck(CardData cardData, bool callRewardScreen = false)
    {
        
    }

    // public void OnMouseBeginDrag()
    // {
    //     if(WorldStateSystem.instance.currentOverlayState == OverlayState.Display)
    //     {
    //         startDragPos = Input.mousePosition.y;
    //         dragging = true;
    //     }
    // }

    // public void OnMouseEndDrag()
    // {
    //     if(WorldStateSystem.instance.currentOverlayState == OverlayState.Display)
    //     {
    //         dragging = false;
    //     }
    // }
    // public void OnMouseDrag()
    // {
    //     if(dragging && WorldStateSystem.instance.currentOverlayState == OverlayState.Display && WorldSystem.instance.deckDisplayManager.selectedCard == null)
    //     {
    //         float sensitivity = WorldSystem.instance.deckDisplayManager.scroller.GetComponent<ScrollRect>().scrollSensitivity;
    //         float currentPos = Input.mousePosition.y;
    //         float direction;

    //         if(currentPos > startDragPos)
    //             direction = -1;
    //         else if(currentPos < startDragPos)
    //             direction = 1;
    //         else
    //             direction = 0;

    //         Vector2 scrollPos = new Vector2(0, direction * sensitivity * 5.0f * -1);
    //         WorldSystem.instance.deckDisplayManager.content.GetComponent<RectTransform>().anchoredPosition += scrollPos;
    //     }
    // }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CardDisplay : CardVisual
{
    public System.Action OnClick;
    public ShopCost shopCost;
    Tween tween;
    public GameObject nonSelectableImage;
    public bool _selectable;
    public bool selectable
    {
        get => _selectable;
        set
        {
            _selectable = value;
            nonSelectableImage.SetActive(!_selectable);
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
            {
                cardHighlightType = CardHighlightType.Selected;
                tween = transform.DOScale(transform.localScale + new Vector3(0.05f, 0.05f, 0.05f), .1f).SetLoops(1, LoopType.Yoyo).OnComplete(
                    () => transform.localScale = Vector3.one
                ).OnKill(
                    () => transform.localScale = Vector3.one
                );
            }
            else
            {
                cardHighlightType = CardHighlightType.None;
                tween = transform.DOScale(Vector3.one, .1f).SetLoops(1, LoopType.Yoyo).OnComplete(
                    () => transform.localScale = Vector3.one
                ).OnKill(
                    () => transform.localScale = Vector3.one
                );
            }
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
        if (selected) 
        {
            DeselectCard();
            return;
        }
        if (!CombatSystem.instance.combatDeckDisplay.CanSelectMore) return;

        CombatSystem.instance.combatDeckDisplay.AddCard(this);
        selected = true;
    }
    public void DeselectCard()
    {
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
        OnClick?.Invoke();
    }

    void ShopCallback()
    {
        if (WorldSystem.instance.shopManager.shop.PurchaseCard(this)) WorldSystem.instance.deckDisplayManager.StartCoroutine(AnimateCardToDeck());
    }

    public void RewardCallback()
    {
        WorldSystem.instance.deckDisplayManager.StartCoroutine(AnimateCardToDeck());

        WorldSystem.instance.characterManager.AddCardToDeck(this);

        WorldSystem.instance.combatRewardManager.rewardScreenCombat.ResetReward();
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
        WorldSystem.instance.deckDisplayManager.DisplayCard(this);
    }


}

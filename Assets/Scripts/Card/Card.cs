using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour, IPointerClickHandler
{

    public CardData cardData;

    public Text nameText;
    public Text descriptionText;

    public Image artworkImage;

    public Text costText;
    public Text damageText;
    public Text blockText;

    [HideInInspector]
    public CombatController combatController;
    IEnumerator CardFollower;
    private DeckDisplayManager deckDisplayManager;
    private float startDragPos;


    public void Awake()
    {
        CardFollower = FollowMouseIsSelected();
        // DEBUG SKA BORT KANSKE 
        if(WorldSystem.instance.worldState != WorldState.Combat)
            deckDisplayManager = WorldSystem.instance.deckDisplayManager;
    }

    public void UpdateDisplay()
    {
        nameText.text = cardData.name;

        artworkImage.sprite = cardData.artwork;

        costText.text = cardData.cost.ToString();

        descriptionText.text = "";

        for(int i = 0; i < cardData.Effects.Count; i++)
        {
            descriptionText.text += cardData.Effects[i].Type.ToString() + ":" + cardData.Effects[i].Value;
            if (cardData.Effects[i].Times != 1) descriptionText.text += " " + cardData.Effects[i].Times + " times.";
            if (i != cardData.Effects.Count - 1) descriptionText.text += "\n";
        }
    }

    public void OnMouseEnter()
    {
        transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);

        if(WorldSystem.instance.worldState == WorldState.Combat)
        {
            transform.SetAsLastSibling();
        }
    }

    public void OnMouseExit()
    {
        transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);

        if(WorldSystem.instance.worldState == WorldState.Combat)
        {
            combatController.ResetSiblingIndexes();
        }
    }

    public void ResetScale()
    {
        transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);
    }
    private IEnumerator FollowMouseIsSelected()
    {
        while (true)
        {
            Vector2 outCoordinates;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(combatController.GetComponent<RectTransform>(), Input.mousePosition, null, out outCoordinates);
            GetComponent<RectTransform>().localPosition = outCoordinates;
            yield return new WaitForSeconds(0);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            OnMouseClick();
        else if (eventData.button == PointerEventData.InputButton.Right)
            OnMouseRightClick();
    }

    public void OnMouseClick()
    {
        switch (WorldSystem.instance.worldState)
        {
            case WorldState.Combat:

                if (!combatController.CardisSelectable(this))
                    break;

                combatController.ActiveCard = this;
                StartCoroutine(CardFollower);
                break;

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

    IEnumerator LerpPosition(Vector3 endValue, float duration)
    {
        float time = 0;
        Vector3 startValue = transform.position;

        while (time < duration)
        {
            transform.position = Vector3.Lerp(startValue, endValue, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = endValue;
        
        
        transform.localPosition = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0.1f);
    }

    public void DisplayCard()
    {
        if(deckDisplayManager.selectedCard == null)
        {
            deckDisplayManager.previousPosition = transform.position;
            deckDisplayManager.selectedCard = this;
            deckDisplayManager.placeholderCard.GetComponent<Card>().cardData = deckDisplayManager.selectedCard.cardData;
            deckDisplayManager.placeholderCard.GetComponent<Card>().UpdateDisplay();
            deckDisplayManager.backgroundPanel.SetActive(true);
            deckDisplayManager.clickableArea.SetActive(true);
            deckDisplayManager.scroller.GetComponent<ScrollRect>().enabled = false;
            transform.localPosition = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0.1f);
            //StartCoroutine(LerpPosition(new Vector3 (Screen.width * 0.5f, Screen.height * 0.5f, 0), 0.1f));
            //transform.localScale += new Vector3(0.5f, 0.5f, 0.5f);
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
        //deckDisplayManager.selectedCard.transform.localScale -= new Vector3(0.5f, 0.5f, 0.5f);
        deckDisplayManager.previousPosition = transform.position;
        deckDisplayManager.selectedCard = null;
    }
    public void ResetCardPositionNext()
    {
        deckDisplayManager.selectedCard.transform.position = deckDisplayManager.previousPosition;
        deckDisplayManager.previousPosition = Vector3.zero;
        deckDisplayManager.selectedCard = null;
    }

    public void OnMouseRightClick()
    {
        if(WorldSystem.instance.worldState == WorldState.Combat)
        {
            combatController.CancelCardSelection(this.gameObject);
            StopCoroutine(CardFollower);
        }
    }
}

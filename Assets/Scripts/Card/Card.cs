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

    public void Awake()
    {
        CardFollower = FollowMouseIsSelected();
    }

    public void UpdateDisplay()
    {
        nameText.text = cardData.name;
        //descriptionText.text = card.description;

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
        //Debug.Log("Mouse over card");
        transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
        transform.SetAsLastSibling();
    }

    public void OnMouseExit()
    {
        transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);
        if(WorldSystem.instance.worldState == WorldState.Combat)
        {
            //Debug.Log("Mouse left card");
            combatController.ResetSiblingIndexes();
        }
        else if(WorldSystem.instance.worldState == WorldState.Shop)
        {
            return;
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
        if(WorldSystem.instance.worldState == WorldState.Combat)
        {
            //Debug.Log("Card Clicked");
            if (!combatController.CardisSelectable(this))
                return;

            combatController.ActiveCard = this;
            StartCoroutine(CardFollower);
        }
        else if(WorldSystem.instance.worldState == WorldState.Shop)
        {
            WorldSystem.instance.shopManager.currentShop.PurchaseCard(this);
            
        }
    }

    public void OnMouseRightClick()
    {
        if(WorldSystem.instance.worldState == WorldState.Combat)
        {
            //Debug.Log("Card Right-Clicked");
            combatController.CancelCardSelection(this.gameObject);
            StopCoroutine(CardFollower);
        }
        else if(WorldSystem.instance.worldState == WorldState.Shop)
        {
            Debug.Log("Shop Right Click");
        }
    }

}

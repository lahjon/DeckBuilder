using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class Card : MonoBehaviour, IPointerClickHandler
{

    public CardData cardData;
    public Text nameText;
    public Text descriptionText;
    public Image artworkImage;

    public Text costText;
    public Text damageText;
    public Text blockText;

<<<<<<< HEAD
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
=======
    public void BindCardData()
>>>>>>> a571c54ce43a855d3f46ab5e9df38fa896375c0b
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

    public abstract void OnMouseEnter();

    public abstract void OnMouseExit();
    public abstract void ResetScale();
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            OnMouseClick();
        else if (eventData.button == PointerEventData.InputButton.Right)
            OnMouseRightClick();
    }

    public abstract void OnMouseClick();

    public abstract void OnMouseRightClick();


}

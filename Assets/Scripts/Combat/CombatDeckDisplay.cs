using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using System;

public class CombatDeckDisplay : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform content;
    public Transform deckDisplay;
    public Transform cardParent;
    public GameObject buttonClose, buttonConfirm;

    public TMP_Text selectAmountText;
    public TMP_Text titleText;
    public int selectAmount;
    public List<CardVisual> sourceCards = new List<CardVisual>();
    public ListEventReporter<CardVisual> selectedCards;
    public List<CardDisplay> allDisplayedCards = new List<CardDisplay>();
    public List<Card> handCopy = new List<Card>();
    public List<Card> customList = new List<Card>();

    public bool CanSelectMore { get => selectedCards.Count < selectAmount; }

    public Action OnSelectionConfirm;

    Dictionary<CardLocation, List<Card>> TypeToPile = new Dictionary<CardLocation, List<Card>>();

    public void Start()
    {
        Debug.Log("kör start combatdisplay");
        selectedCards = new ListEventReporter<CardVisual>(UpdateAmountText);
        TypeToPile[CardLocation.Deck] = CombatSystem.instance.Hero.deck;
        TypeToPile[CardLocation.Discard] = CombatSystem.instance.Hero.discard;
        TypeToPile[CardLocation.Exhaust] = CombatSystem.instance.Hero.exhaust;
        TypeToPile[CardLocation.Hand] = handCopy;
        
    }

    void Open()
    {
        deckDisplay.gameObject.SetActive(true);
        WorldStateSystem.SetInDisplay();
    }

    public void OpenDeckDisplay(CardLocation cardLocation, int aSelectAmount = 0, List<Card> customSubset = null, Action OnSelectionConfirm = null)
    {
        this.OnSelectionConfirm = OnSelectionConfirm;
        handCopy.Clear();
        handCopy.AddRange(CombatSystem.instance.Hand);
        selectAmount = aSelectAmount;
        sourceCards.Clear();
        selectedCards.Clear();

        if (selectAmount > 0)   EnableSelect("Select cards from your " + cardLocation.ToString());
        else                    DisableSelect(cardLocation.ToString());

        if (customSubset is null)
            TypeToPile[cardLocation].ForEach(c => sourceCards.Add((CardVisual)c));
        else
            customSubset.ForEach(c => sourceCards.Add((CardVisual)c));

        UpdateAllCards();
        Open();
    }


    void EnableSelect(string aText)
    {
        titleText.text = aText;
        selectAmountText.text = string.Format("{0} / {1}", selectedCards.Count, selectAmount);
        selectAmountText.gameObject.SetActive(true);
        buttonClose.SetActive(false);
        buttonConfirm.SetActive(true);
    }
    void DisableSelect(string aText)
    {
        titleText.text = aText;
        selectAmountText.gameObject.SetActive(false);
        buttonClose.SetActive(true);
        buttonConfirm.SetActive(false);
    }

    public void UpdateAllCards()
    {
        while (sourceCards.Count > allDisplayedCards.Count)
        {
            CardDisplay newCard = Instantiate(cardPrefab,content.gameObject.transform).GetComponent<CardDisplay>();
            newCard.transform.SetParent(content.gameObject.transform);
            newCard.gameObject.SetActive(true);
            allDisplayedCards.Add(newCard);
        }
        while (sourceCards.Count < allDisplayedCards.Count)
        {   
            Destroy(allDisplayedCards[(allDisplayedCards.Count - 1)].gameObject);
            allDisplayedCards.RemoveAt(allDisplayedCards.Count - 1);
        }

        for (int i = 0; i < sourceCards.Count; i++)
        {
            CardDisplay cd = allDisplayedCards[i];
            cd.Mimic(sourceCards[i]);
            cd.selected = false;

            if (selectAmount > 0) 
                cd.OnClick = () => cd.SelectCard();
            //else newCard.clickCallback = () => WorldSystem.instance.deckDisplayManager.DisplayCard(newCard);
        }
    }

    public void AddCard(CardDisplay aCard)      => selectedCards.Add(sourceCards[allDisplayedCards.IndexOf(aCard)]);
    public void RemoveCard(CardDisplay aCard)   => selectedCards.Remove(sourceCards[allDisplayedCards.IndexOf(aCard)]);

    public void UpdateAmountText() => selectAmountText.text = string.Format("{0} / {1}", selectedCards.Count, selectAmount);

    public void ButtonConfirm()
    {
        if (selectedCards.Count < selectAmount)
        {
            WorldSystem.instance.uiManager.UIWarningController.CreateWarning(string.Format("Select {0} cards!", selectAmount));
            return;
        }
        OnSelectionConfirm?.Invoke();
        ButtonClose();
    }
    public void ButtonClose()
    {
        WorldStateSystem.TriggerClear();
        deckDisplay.gameObject.SetActive(false);
    }
}

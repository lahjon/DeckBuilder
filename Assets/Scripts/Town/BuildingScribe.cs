using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class BuildingScribe : Building, ISaveableCharacter, ISaveableWorld
{
    public GameObject scribe, deckManagement, cardUpgrade; // rooms
    public GameObject cardPrefab;
    public List<CardData> startingCardsBerserker = new List<CardData>();
    public List<string> unlockedCards = new List<string>();
    public List<string> currentCards = new List<string>();
    public List<string> sideCards = new List<string>();
    public List<CardDisplay> allCards = new List<CardDisplay>();
    public Transform deckParent, sideParent;

    void Start()
    {
        DatabaseSystem.instance.GetCardsByName(currentCards).ForEach(x => CreateCard(x, true));
        DatabaseSystem.instance.GetCardsByName(sideCards).ForEach(x => CreateCard(x, false));

        // if (currentCards?.Any() == true)
        // {
        //     DatabaseSystem.instance.GetCardsByName(currentCards).ForEach(x => CreateCard(x, true));
        //     DatabaseSystem.instance.GetCardsByName(sideCards).ForEach(x => CreateCard(x, false));
        // }
        // else
        // {
        //     // if first time starting game
        //     DatabaseSystem.instance.GetCardsByName(startingCardsBerserker.Select(x => x.name).ToList()).ForEach(x => CreateCard(x, true));
        // }
    }

    public void UnlockCard(CardData data)
    {
        unlockedCards.Add(data.name);
        sideCards.Add(data.name);
        CreateCard(data, false);
    }

    void CreateCard(CardData data, bool inDeck)
    {
        Transform parent = inDeck ? deckParent : sideParent;
        CardDisplay display = Instantiate(cardPrefab, parent).GetComponent<CardDisplay>();
        display.name = data.name;
        display.cardData = data;
        display.BindCardData();
        display.BindCardVisualData();
        allCards.Add(display);
        display.clickCallback = () => MoveCard(display);
    }

    public void MoveCard(CardDisplay card)
    {
        if (card.transform.parent == deckParent)
        {
            MoveToSide(card);
        }
        else
        {
            MoveToDeck(card);
        }
    }

    void MoveToDeck(CardDisplay card)
    {
        Debug.Log("To Deck!");
        card.transform.SetParent(deckParent);
        currentCards.Add(card.cardData.name);
        sideCards.Remove(card.cardData.name);
    }
    void MoveToSide(CardDisplay card)
    {
        Debug.Log("To Side!");
        card.transform.SetParent(sideParent);
        Debug.Log(card.name);
        currentCards.Remove(card.cardData.name);
        sideCards.Add(card.cardData.name);
    }

    void UpdateDeck()
    {
        // if(allCardsData.Count > allDisplayedCards.Count)
        // {
        //     while (allCardsData.Count > allDisplayedCards.Count)
        //     {
        //         CardDisplay newCard = Instantiate(cardPrefab,content.gameObject.transform).GetComponent<CardDisplay>();
        //         newCard.transform.SetParent(content.gameObject.transform);
        //         newCard.gameObject.SetActive(true);
        //         allDisplayedCards.Add(newCard);
        //         newCard.clickCallback = () => DisplayCard(newCard);
        //     }
        // }
        // else if(allCardsData.Count < allDisplayedCards.Count)
        // {
        //     while (allCardsData.Count < allDisplayedCards.Count)
        //     {   
        //         Destroy(allDisplayedCards[(allDisplayedCards.Count - 1)].gameObject);
        //         allDisplayedCards.RemoveAt(allDisplayedCards.Count - 1);
        //     }
        // }

        // for (int i = 0; i < allCardsData.Count; i++)
        // {
        //     allDisplayedCards[i].cardData = allCardsData[i];
        //     allDisplayedCards[i].BindCardData();
        //     allDisplayedCards[i].BindCardVisualData();
        // }
    }
    public override void CloseBuilding()
    {
        base.CloseBuilding();
    }
    public override void EnterBuilding()
    {
        base.EnterBuilding();
        StepInto(scribe);
    }
    public void ButtonEnterDeckManagement()
    {
        UpdateDeck();
        StepInto(deckManagement);
    }
    public void ButtonEnterCardUpgrade()
    {
        StepInto(cardUpgrade);
    }

    public void PopulateSaveDataCharacter(SaveDataCharacter a_SaveData)
    {
        a_SaveData.currentCards = currentCards;
        a_SaveData.sideCards = sideCards;
    }

    public void LoadFromSaveDataCharacter(SaveDataCharacter a_SaveData)
    {
        if (a_SaveData.currentCards?.Any() == true)
        {
            currentCards = a_SaveData.currentCards;
            Debug.Log("True: " + currentCards.Count);
        }
        else
        {
            currentCards = startingCardsBerserker.Select(x => x.name).ToList();
            Debug.Log("False: " + currentCards.Count);
        }
        sideCards = a_SaveData.sideCards?.Any() == true ? a_SaveData.sideCards : new List<string>();
    }

    public void PopulateSaveDataWorld(SaveDataWorld a_SaveData)
    {
        a_SaveData.unlockedCards = unlockedCards;
    }

    public void LoadFromSaveDataWorld(SaveDataWorld a_SaveData)
    {
        if (a_SaveData.unlockedCards?.Any() == true)
        {
            unlockedCards = a_SaveData.unlockedCards;
        }
        else
        {
            unlockedCards = new List<string>();
        }
    }
}

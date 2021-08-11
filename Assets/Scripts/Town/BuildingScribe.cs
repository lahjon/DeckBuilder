using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class BuildingScribe : Building, ISaveableCharacter, ISaveableWorld
{
    public GameObject scribe, deckManagement, cardUpgrade; // rooms
    public GameObject cardPrefab;
    public List<string> unlockedCards = new List<string>();
    public List<string> currentCards = new List<string>();
    public List<string> sideCards = new List<string>();
    public List<CardDisplay> allSideCards = new List<CardDisplay>();
    public List<CardDisplay> allDeckCards = new List<CardDisplay>();
    public Transform deckParent, sideParent;

    void Start()
    {
        UpdateDeck();
    }

    void UpdateDeck()
    {
        DatabaseSystem.instance.GetCardsByName(currentCards).ForEach(x => CreateCard(x, true));
        DatabaseSystem.instance.GetCardsByName(sideCards).ForEach(x => CreateCard(x, false));
        SortDeck();
        WorldSystem.instance.characterManager.playerCardsData = DatabaseSystem.instance.GetStartingDeck(true).Concat(allDeckCards.Select(x => x.cardData)).ToList();
    }

    public List<CardData> GetStartDeck()
    {
        return DatabaseSystem.instance.GetStartingDeck(true).Concat(DatabaseSystem.instance.GetStartingDeck(false)).ToList();
    }

    public void UnlockCard(CardData data)
    {
        unlockedCards.Add(data.name);
        sideCards.Add(data.name);
        CreateCard(data, false);
    }

    void CreateCard(CardData data, bool inDeck, CardDisplay aDisplay = null)
    {
        Transform parent = inDeck ? deckParent : sideParent;
        CardDisplay display;
        if (aDisplay == null) 
            display = Instantiate(cardPrefab, parent).GetComponent<CardDisplay>();
        else 
            display = aDisplay;
        display.name = data.name;
        display.cardData = data;
        display.BindCardData();
        display.BindCardVisualData();
        if (inDeck)
        {
            allDeckCards.Add(display);
        }
        else
        {
            allSideCards.Add(display);
        }
        display.clickCallback = () => MoveCard(display);
    }
    void SortDeck()
    {
        // sort deck
        allDeckCards.OrderBy(x => x.name);
        if (allDeckCards.Count == currentCards.Count)
        {
            for (int i = 0; i < currentCards.Count; i++)
            {
                if (allDeckCards[i].name == currentCards[i])
                {
                    allDeckCards[i].transform.SetSiblingIndex(currentCards.IndexOf(currentCards[i]));
                }
            }
        }
    
        // sort side
        allSideCards.OrderBy(x => x.name);
        if (allSideCards.Count == currentCards.Count)
        {
            for (int i = 0; i < currentCards.Count; i++)
            {
                if (allSideCards[i].name == currentCards[i])
                {
                    allSideCards[i].transform.SetSiblingIndex(currentCards.IndexOf(currentCards[i]));
                }
            }
        }
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
        card.transform.SetParent(deckParent);
        currentCards.Add(card.cardData.name);
        allDeckCards.Add(card);
        currentCards.Sort();
        if (currentCards.FirstOrDefault(x => x == card.cardData.name) is string c) card.transform.SetSiblingIndex(currentCards.IndexOf(c));

        allSideCards.Remove(card);
        sideCards.Remove(card.cardData.name);
    }
    void MoveToSide(CardDisplay card)
    {
        card.transform.SetParent(sideParent);
        sideCards.Add(card.cardData.name);
        allSideCards.Add(card);
        sideCards.Sort();
        if (sideCards.FirstOrDefault(x => x == card.cardData.name) is string c) card.transform.SetSiblingIndex(sideCards.IndexOf(c));

        allDeckCards.Remove(card);
        currentCards.Remove(card.cardData.name);
    }

    public void UpdateScribe()
    {
        SaveDataManager.LoadJsonData(GetComponents<ISaveableCharacter>(), (int)WorldSystem.instance.characterManager.selectedCharacterClassType);
        var allCards = allSideCards.Concat(allDeckCards).ToList();
        while (allCards.Count > 0)
        {
            Destroy(allCards[allCards.Count - 1].gameObject);
            allCards.RemoveAt(allCards.Count - 1);
        }
        allSideCards.Clear();
        allDeckCards.Clear();
        UpdateDeck();
    }

    void ConfirmDeck()
    {
        WorldSystem.instance.characterManager.playerCardsData = DatabaseSystem.instance.GetStartingDeck(true).Concat(allDeckCards.Select(x => x.cardData)).ToList();
    }
    public override void CloseBuilding()
    {
        ConfirmDeck();
        base.CloseBuilding();
    }
    public override void EnterBuilding()
    {
        base.EnterBuilding();
        StepInto(scribe);
    }
    public void ButtonEnterDeckManagement()
    {
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
        }
        else
        {
            currentCards = DatabaseSystem.instance.GetStartingDeck(false).Select(x => x.name).ToList();
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

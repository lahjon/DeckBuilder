using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;


public class BuildingScribe : Building, ISaveableCharacter, ISaveableWorld
{
    public GameObject scribe, deckManagement, cardUpgrade; // rooms
    public GameObject cardPrefab;
    public List<CardWrapper> unlockedCards = new List<CardWrapper>();
    public List<string> deckCards = new List<string>();
    public List<string> sideCards = new List<string>();
    public List<CardDisplay> allSideCards = new List<CardDisplay>();
    public List<CardDisplay> allDeckCards = new List<CardDisplay>();
    public List<CardData> extraCards = new List<CardData>();
    public Transform deckParent, sideParent;
    public TMP_Text sideboardAmountText;
    public GameObject warningPrompt;
    public int maxSideboardCards;

    void Start()
    {
        UpdateDeck();
    }

    void UpdateDeck()
    {
        DatabaseSystem.instance.GetCardsByID(deckCards).ForEach(x => CreateCard(x, true));
        DatabaseSystem.instance.GetCardsByID(sideCards).ForEach(x => CreateCard(x, false));
        SortDeck();
        WorldSystem.instance.characterManager.playerCardsData = GetDeck();
    }

    public void UnlockProfessionCard(Profession profession)
    {
        // SWAP TO ID
        DatabaseSystem.instance.GetStartingProfessionCards(profession).ForEach(x => unlockedCards.Add(new CardWrapper(x.cardName)));
    }

    public List<CardData> GetStartingDeck()
    {
        return DatabaseSystem.instance.GetStartingDeck(true).Concat(DatabaseSystem.instance.GetStartingDeck(false)).ToList();
    }
    public List<CardData> GetDeck()
    {
        return DatabaseSystem.instance.GetStartingDeck(true).Concat(allDeckCards.Select(x => x.cardData)).Concat(extraCards).ToList();
    }

    public void UnlockCard(CardData data)
    {
        unlockedCards.Add(new CardWrapper(data.cardName));
        sideCards.Add(data.cardName);
        CreateCard(data, false);
    }

    public void UpgradeCardPermanent(string cardId)
    {
        if (unlockedCards.FirstOrDefault(x => x.cardId == cardId) is CardWrapper card && DatabaseSystem.instance.GetCardByID(cardId) is CardData data)
        {
            int idx = card.timesUpgraded;
            if (idx < data.cardModifiers.Count)
            {
                card.cardModifiersId.Add(data.cardModifiers[idx].id);
            }
        }
    }

    public void AddModifierToCard(string cardId, CardModifierData cardModifierData)
    {
        if (unlockedCards.FirstOrDefault(x => x.cardId == cardId) is CardWrapper card)
        {
            card.cardModifiersId.Add(cardModifierData.id);
        }
    }

    void CreateCard(CardData data, bool inDeck, CardDisplay aDisplay = null)
    {
        Transform parent = inDeck ? deckParent : sideParent;
        CardDisplay display;
        if (aDisplay == null) 
            display = Instantiate(cardPrefab, parent).GetComponent<CardDisplay>();
        else 
            display = aDisplay;
        display.name = data.cardName;
        display.cardData = data;
        display.BindCardData();
        display.BindCardVisualData();
        if (inDeck)
        {
            allDeckCards.Add(display);
            Debug.Log("Dick");
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
        allDeckCards.OrderBy(x => x.cardName);
        if (allDeckCards.Count == deckCards.Count)
        {
            for (int i = 0; i < deckCards.Count; i++)
            {
                if (allDeckCards[i].cardName == deckCards[i])
                {
                    allDeckCards[i].transform.SetSiblingIndex(deckCards.IndexOf(deckCards[i]));
                }
            }
        }
    
        // sort side
        allSideCards.OrderBy(x => x.cardName);
        if (allSideCards.Count == deckCards.Count)
        {
            for (int i = 0; i < deckCards.Count; i++)
            {
                if (allSideCards[i].cardName == deckCards[i])
                {
                    allSideCards[i].transform.SetSiblingIndex(deckCards.IndexOf(deckCards[i]));
                }
            }
        }
    }

    void UpdateCounter()
    {
        sideboardAmountText.text = string.Format("{0} / {1}", deckCards.Count, maxSideboardCards);
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

        UpdateCounter();
    }

    void MoveToDeck(CardDisplay card)
    {
        if (deckCards.Count >= maxSideboardCards) return;

        card.transform.SetParent(deckParent);
        deckCards.Add(card.cardData.cardName);
        allDeckCards.Add(card);
        deckCards.Sort();
        if (deckCards.FirstOrDefault(x => x == card.cardData.cardName) is string c) card.transform.SetSiblingIndex(deckCards.IndexOf(c));

        allSideCards.Remove(card);
        sideCards.Remove(card.cardData.cardName);
    }
    void MoveToSide(CardDisplay card)
    {
        Debug.Log("Move to side");
        card.transform.SetParent(sideParent);
        sideCards.Add(card.cardData.cardName);
        allSideCards.Add(card);
        sideCards.Sort();
        if (sideCards.FirstOrDefault(x => x == card.cardData.cardName) is string c) card.transform.SetSiblingIndex(sideCards.IndexOf(c));

        allDeckCards.Remove(card);
        deckCards.Remove(card.cardData.cardName);
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
        WorldSystem.instance.characterManager.playerCardsData = GetDeck();
    }
    public override void CloseBuilding()
    {
        if (deckCards.Count < maxSideboardCards)
        {
            PromptWarning();
            return;
        }

        ConfirmDeck();
        base.CloseBuilding();
    }

    void PromptWarning()
    {
        WorldSystem.instance.uiManager.UIWarningController.CreateWarning("You need to select " + maxSideboardCards.ToString() + " cards!");
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
        a_SaveData.currentCards = deckCards;
        a_SaveData.sideCards = sideCards;
    }

    public void LoadFromSaveDataCharacter(SaveDataCharacter a_SaveData)
    {
        if (a_SaveData.currentCards?.Any() == true)
        {
            deckCards = a_SaveData.currentCards;
            sideCards = a_SaveData.sideCards;
        }
        // else if (a_SaveData.currentCards == null)
        // {
        //     // if no cards unlocked
        //     currentCards = new List<string>();
        //     sideCards = new List<string>();
        // }
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
            UnlockProfessionCard(Profession.Berserker1);
            deckCards = unlockedCards.Select(x => x.cardId).ToList();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.UI;


public class BuildingScribe : Building, ISaveableCharacter, ISaveableWorld
{
    public GameObject scribe, deckManagement, cardUpgrade; // rooms
    public GameObject upgradedCardWindow;
    public Button confirmButton, cancelButton, upgradeWindowButton;
    public CardDisplay upgradedCard;
    public GameObject cardPrefab, upgradeCostPrefab;
    public List<CardWrapper> unlockedCards = new List<CardWrapper>();
    public List<CardWrapper> deckCards = new List<CardWrapper>();
    public List<CardDisplay> allSideCards = new List<CardDisplay>();
    public List<CardDisplay> allDeckCards = new List<CardDisplay>();
    public List<CardData> extraCards = new List<CardData>();
    public Transform deckParent, sideParent, upgradeParent;
    public TMP_Text sideboardAmountText;
    public int maxSideboardAmount;
    public int lockedSideboardAmount;
    public CardVisual selectedCard;
    public CardWrapper currentCw;
    GridLayoutGroup glg;
    Dictionary<int, GameObject> allCosts = new Dictionary<int, GameObject>();

    void Start()
    {
        glg = upgradeParent.GetComponent<GridLayoutGroup>();
        UpdateScribe();
        UpdateDeck();
        ConfirmDeck();
    }

    void ResetDeck()
    {
        List<CardDisplay> allCards = new List<CardDisplay>();
        allDeckCards.ForEach(x => allCards.Add(x));
        allCards.ForEach(x => MoveToSide(x, false));
        allCosts.Clear();
    }

    void UpdateDeck()
    {
        //ResetDeck();
        List<CardDisplay> cards = new List<CardDisplay>();

        foreach(CardDisplay card in allSideCards)
        {
            if(card.rarity == Rarity.Starting)
                card.OnClick = null;
            else
                card.OnClick = () => MoveCard(card);
            card.selectable = card.rarity != Rarity.Starting;
        }
        
        foreach (CardDisplay c in allSideCards)
            foreach (CardWrapper cw in deckCards)
                if (cw.idx == c.idx && cw.cardId == c.cardId)
                    cards.Add(c);

        cards.ForEach(c => MoveToDeck(c, false));

        SortDeck();
    }

    public void UnlockProfessionCard(Profession profession)
    {
        DatabaseSystem.instance.GetStartingProfessionCards(profession).Concat(DatabaseSystem.instance.GetStartingDeck(true)).ToList().ForEach(x => UnlockCard(x, false));
    }

    public List<CardData> GetStartingDeck()
    {
        return DatabaseSystem.instance.GetStartingDeck(true).Concat(DatabaseSystem.instance.GetStartingDeck(false)).ToList();
    }
    public List<CardData> GetDeck()
    {
        return DatabaseSystem.instance.GetStartingDeck(true).Concat(allDeckCards.Select(x => x.cardData)).Concat(extraCards).ToList();
    }

    public void UnlockCard(CardData data, bool save = true)
    {
        int idx = unlockedCards?.Any() == true ? unlockedCards[0].idx++ : 0;

        CardWrapper cw = new CardWrapper(data.id, idx);
        unlockedCards.Add(cw);

        if (save) WorldSystem.instance.SaveProgression();
        
    }
    void CreateCardManage(CardDisplay display, CardWrapper cw)
    {
        CardData data = DatabaseSystem.instance.GetCardByID(cw.cardId);
        display.cardData = data;
        display.BindCardData();
        display.BindCardVisualData();

        for (int i = 0; i < cw.timesUpgraded; i++)
            display.AddModifierToCard(data.upgrades[i]);

        display.idx = cw.idx;
        //allCosts[display.idx].transform.GetChild(0).GetComponent<TMP_Text>().text = cw.timesUpgraded < data.maxUpgrades ? (int.Parse(display.cardData.upgradeCostShards) * (cw.timesUpgraded + 1)).ToString() + " shards" : "";
    }

    void PreviewUpgradeCard(CardDisplay card)
    {   
        if (unlockedCards.FirstOrDefault(x => x.idx == card.idx && x.cardId == card.cardId) is CardWrapper cw)
        {
            int upgradeCost = int.Parse(card.cardData.upgradeCostShards) * (cw.timesUpgraded + 1);
            
            if (cw.timesUpgraded >= card.cardData.upgrades.Count)
            {
                WorldSystem.instance.uiManager.UIWarningController.CreateWarning("Card is fully upgraded!");
                return;
            }
            else if(WorldSystem.instance.characterManager.shard < upgradeCost)
            {
                WorldSystem.instance.uiManager.UIWarningController.CreateWarning("Not enough shards!");
                return;
            }

            currentCw = cw;
            selectedCard = card;
            glg.enabled = false;
            upgradedCard.Clone(card);
            card.gameObject.SetActive(false);
            upgradedCardWindow.SetActive(true);
        }

    }
    public void ButtonUpgradeCard()
    {
        if (currentCw == null || selectedCard == null) return;
        int upgradeCost = int.Parse(selectedCard.cardData.upgradeCostShards) * (currentCw.timesUpgraded + 1);
        WorldSystem.instance.characterManager.shard -= upgradeCost;
        CardFunctionalityData cardModifierData = selectedCard.cardData.upgrades[currentCw.timesUpgraded];
        currentCw.RegisterUpgrade(cardModifierData.id);
        selectedCard.UpgradeCard();
        //if (selectedCard.UpgradeCard()) allCosts[selectedCard.idx].transform.GetChild(0).GetComponent<TMP_Text>().text = selectedCard.timesUpgraded + 1 < selectedCard.cardData.maxUpgrades ? (int.Parse(selectedCard.cardData.upgradeCostShards) * (selectedCard.timesUpgraded + 2)).ToString() + " shards" : "";
        ButtonCloseUpgrade();
        WorldSystem.instance.SaveProgression();
    }
    public void ButtonCloseUpgrade()
    {
        selectedCard?.gameObject.SetActive(true);
        selectedCard = null;
        glg.enabled = true;
        upgradedCardWindow.SetActive(false);
    }

    void SortDeck()
    {
        // sort deck
        List<CardDisplay> sorted = allDeckCards.OrderBy(x => x.cardName).ToList();

        for (int i = 0; i < sorted.Count; i++)
            sorted[i].transform.SetSiblingIndex(i);
    
        // sort side
        sorted = allSideCards.OrderBy(x => x.cardName).ToList();

        for (int i = 0; i < sorted.Count; i++)
            sorted[i].transform.SetSiblingIndex(i);
    }

    void UpdateCounter()
    {
        sideboardAmountText.text = string.Format("{0} / {1}", allDeckCards.Count, maxSideboardAmount + lockedSideboardAmount);
    }

    public void MoveCard(CardDisplay card)
    {
        if (card.transform.parent == deckParent)
            MoveToSide(card);
        else
            MoveToDeck(card);

        UpdateCounter();
    }

    void MoveToDeck(CardDisplay card, bool adjustLists = true)
    {
        if (allDeckCards.Count >= maxSideboardAmount + lockedSideboardAmount) return;

        allDeckCards.Add(card);
        allSideCards.Remove(card);
        List<CardDisplay> sorted = allDeckCards.OrderBy(x => x.cardName).ToList();

        card.transform.SetParent(deckParent);
        card.transform.SetSiblingIndex(sorted.IndexOf(card));

        if (adjustLists)
        {
            CardWrapper cw = unlockedCards.FirstOrDefault(x => x.cardId == card.cardId && x.idx == card.idx);
            deckCards.Add(cw);
        }
    }
    void MoveToSide(CardDisplay card, bool adjustLists = true)
    {
        allSideCards.Add(card);
        allDeckCards.Remove(card);
        List<CardDisplay> sorted = allSideCards.OrderBy(x => x.cardName).ToList();

        card.transform.SetParent(sideParent);
        card.transform.SetSiblingIndex(sorted.IndexOf(card));

        if (adjustLists)
        {
            CardWrapper cw = unlockedCards.FirstOrDefault(x => x.cardId == card.cardId && x.idx == card.idx);
            deckCards.Remove(cw);
        }
    }

    public void UpdateScribe()
    {
        CardDisplay display;
        ResetDeck();
        List<CardWrapper> subsetCharacterCards = new List<CardWrapper>();

        for (int i = 0; i < unlockedCards.Count; i++)
        {
            if (DatabaseSystem.instance.GetCardByID(unlockedCards[i].cardId) is CardData data)
            {
                if ((int)data.cardClass == (int)WorldSystem.instance.characterManager.selectedCharacterClassType || data.cardClass == CardClassType.Colorless)
                {
                    subsetCharacterCards.Add(unlockedCards[i]);
                }
            }
        }
        
        while (allSideCards.Count < subsetCharacterCards.Count)
        {
            display = Instantiate(cardPrefab, sideParent).GetComponent<CardDisplay>();
            allSideCards.Add(display);
        }
        while (allSideCards.Count > subsetCharacterCards.Count)
        {
            Destroy(allSideCards[allSideCards.Count - 1].gameObject);
            allSideCards.RemoveAt(allSideCards.Count - 1);
        }

        for (int i = 0; i < subsetCharacterCards.Count; i++)
        {
            // GameObject goCost = Instantiate(upgradeCostPrefab, allSideCards[i].transform);
            // allCosts.Add(subsetCharacterCards[i].idx, goCost);
            CreateCardManage(allSideCards[i], subsetCharacterCards[i]);
        }
    }

    public void UpdateUpgradeManagement()
    {
        foreach (CardDisplay display in allSideCards.Concat(allDeckCards).ToList())
        {
            display.OnClick = () => PreviewUpgradeCard(display);
            display.transform.SetParent(upgradeParent);
            display.selectable = true;
        }
        SortDeck();
    }

    void ConfirmDeck()
    {
        WorldSystem.instance.characterManager.ClearDeck();
        allDeckCards.ForEach(c => WorldSystem.instance.characterManager.AddCardToDeck(c));
    }
    public override void CloseBuilding()
    {
        ConfirmDeck();
        base.CloseBuilding();
    }

    void PromptWarning()
    {
        WorldSystem.instance.uiManager.UIWarningController.CreateWarning("You need to select " + maxSideboardAmount.ToString() + " cards!");
    }
    public override void EnterBuilding()
    {
        base.EnterBuilding();
        UpdateScribe();
        UpdateDeck();
        StepInto(scribe);
    }
    public void ButtonEnterDeckManagement()
    {
        UpdateDeck();
        UpdateCounter();
        StepInto(deckManagement);
    }
    public void ButtonEnterCardUpgrade()
    {
        UpdateUpgradeManagement();
        StepInto(cardUpgrade);
    }

    public void ButtonLeaveManagement()
    {
        if (allDeckCards.Count < maxSideboardAmount + lockedSideboardAmount)
        {
            PromptWarning();
        }
        else
        {
            StepBack();
        }
    }

    public void ButtonLeaveUpgrade()
    {
        allSideCards.ForEach(x => x.transform.SetParent(sideParent));
        allDeckCards.ForEach(x => x.transform.SetParent(deckParent));
        StepBack();
    }

    public void PopulateSaveDataCharacter(SaveDataCharacter a_SaveData)
    {
        List<CardWrapper> deckCards = new List<CardWrapper>();
        allDeckCards.ForEach(x => deckCards.Add(new CardWrapper(x.cardId, x.idx)));

        List<CardWrapper> sideCards = new List<CardWrapper>();
        allSideCards.ForEach(x => sideCards.Add(new CardWrapper(x.cardId, x.idx)));
        
        a_SaveData.deckCards = deckCards;
        a_SaveData.sideCards = sideCards;
    }

    public void LoadFromSaveDataCharacter(SaveDataCharacter a_SaveData)
    {
        if (a_SaveData.deckCards?.Any() == true)
        {
            deckCards = a_SaveData.deckCards;
        }
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
            deckCards.AddRange(unlockedCards);
        }
    }
}
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
    }

    void UpdateDeck()
    {
        List<CardDisplay> cards = new List<CardDisplay>();

        foreach(CardDisplay card in allSideCards.Concat(allDeckCards))
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

    public void UnlockProfessionCard(ProfessionType profession)
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
        display.shopCost.SetCost(CurrencyType.FullEmber, CostType.FullyUpgraded, cw.timesUpgraded < data.maxUpgrades ? int.Parse(display.cardData.upgradeCostFullEmber) * (cw.timesUpgraded + 1) : 0);
    }

    void PreviewUpgradeCard(CardDisplay card)
    {   
        if (unlockedCards.FirstOrDefault(x => x.idx == card.idx && x.cardId == card.cardId) is CardWrapper cw)
        {
            int upgradeCost = int.Parse(card.cardData.upgradeCostFullEmber) * (cw.timesUpgraded + 1);
            
            if (cw.timesUpgraded >= card.cardData.upgrades.Count)
            {
                WorldSystem.instance.uiManager.UIWarningController.CreateWarning("Card is fully upgraded!");
                return;
            }
            else if(WorldSystem.instance.characterManager.characterCurrency.fullEmber < upgradeCost)
            {
                WorldSystem.instance.uiManager.UIWarningController.CreateWarning("Not enough full embers!");
                return;
            }

            currentCw = cw;
            selectedCard = card;
            glg.enabled = false;
            upgradedCard.Clone(card);
            upgradedCard.UpgradeCard();
            card.gameObject.SetActive(false);
            upgradedCardWindow.SetActive(true);
        }

    }
    public void ButtonUpgradeCard()
    {
        if (currentCw == null || selectedCard == null) return;
        int upgradeCost = int.Parse(selectedCard.cardData.upgradeCostFullEmber) * (currentCw.timesUpgraded + 1);
        WorldSystem.instance.characterManager.characterCurrency.fullEmber -= upgradeCost;
        CardFunctionalityData cardModifierData = selectedCard.cardData.upgrades[currentCw.timesUpgraded];
        currentCw.RegisterUpgrade(cardModifierData.id);
        //selectedCard.UpgradeCard();
        selectedCard.UpgradeCard();
        if ((CardDisplay)selectedCard is CardDisplay display)
            display.shopCost.SetCost(CurrencyType.FullEmber, CostType.FullyUpgraded, display.timesUpgraded < display.cardData.maxUpgrades ? int.Parse(display.cardData.upgradeCostFullEmber) * (display.timesUpgraded + 1) : 0);
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
            display.shopCost = Instantiate(upgradeCostPrefab, display.transform).GetComponent<ShopCost>();
            display.shopCost.gameObject.SetActive(false);
            allSideCards.Add(display);
        }
        while (allSideCards.Count > subsetCharacterCards.Count)
        {
            Destroy(allSideCards[allSideCards.Count - 1].gameObject);
            allSideCards.RemoveAt(allSideCards.Count - 1);
        }

        for (int i = 0; i < subsetCharacterCards.Count; i++)
        {
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
            display.shopCost.gameObject.SetActive(true);
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
        WorldSystem.instance.characterManager.characterVariablesUI.currencyBar.SetActive(false);
    }

    void PromptWarning()
    {
        WorldSystem.instance.uiManager.UIWarningController.CreateWarning("You need to select " + maxSideboardAmount.ToString() + " cards!");
    }
    public override void EnterBuilding()
    {
        base.EnterBuilding();
        WorldSystem.instance.characterManager.characterVariablesUI.currencyBar.SetActive(true);
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
        allSideCards.ForEach(x => {
            x.transform.SetParent(sideParent);
            x.shopCost.gameObject.SetActive(false);
        });
        allDeckCards.ForEach(x => {
            x.transform.SetParent(deckParent);
            x.shopCost.gameObject.SetActive(false);
        });

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
            UnlockProfessionCard(ProfessionType.Berserker1);
            deckCards.AddRange(unlockedCards);
        }
    }
}
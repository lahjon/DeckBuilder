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
    public CardDisplay upgradedCard;
    public GameObject cardPrefab;
    public List<CardWrapper> unlockedCards = new List<CardWrapper>();
    public List<CardWrapper> deckCards = new List<CardWrapper>();
    public List<CardWrapper> sideCards = new List<CardWrapper>();
    public List<CardDisplay> allSideCards = new List<CardDisplay>();
    public List<CardDisplay> allUpgradeCards = new List<CardDisplay>();
    public List<CardDisplay> allDeckCards = new List<CardDisplay>();
    public List<CardData> extraCards = new List<CardData>();
    public Transform deckParent, sideParent, upgradeParent;
    public TMP_Text sideboardAmountText;
    public int maxSideboardCards;
    public int idxCounter;

    void Start()
    {
        // UpdateDeck();
        // UpdateUpgradeManagement();
    }

    void UpdateDeck()
    {
        List<CardDisplay> cards = new List<CardDisplay>();
        foreach (CardDisplay c in allSideCards)
        {
            foreach (CardWrapper cw in deckCards)
            {
                if (cw.idx == c.idx && cw.cardId == c.cardName)
                {
                    cards.Add(c);
                }
            }
        }
        cards.ForEach(c => MoveToDeck(c));

        while (allDeckCards.Count < maxSideboardCards)
        {
            MoveToDeck(allSideCards[0]);
        }

        SortDeck();
    }

    public void UnlockProfessionCard(Profession profession)
    {
        // SWAP TO ID
        DatabaseSystem.instance.GetStartingProfessionCards(profession).ForEach(x => UnlockCard(x, false));
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
        unlockedCards.Add(new CardWrapper(data.cardName, idxCounter));
        //CreateCardManage(data);
        if (save) WorldSystem.instance.SaveProgression();
        
    }


    public void AddModifierToCard(string cardId, CardFunctionalityData cardModifierData)
    {
        if (unlockedCards.FirstOrDefault(x => x.cardId == cardId) is CardWrapper card)
        {
            card.cardModifiersId.Add(cardModifierData.id);
        }
    }

    void CreateCardManage(CardDisplay display, CardData data, int idxOverride = -1)
    {
        display.name = data.cardName;
        display.cardData = data;
        display.BindCardData();
        display.BindCardVisualData();

        for (int i = 0; i < display.cardModifiers.Count; i++)
        {
            display.UpdateCardVisual();
        }
        
        if (idxOverride >= 0)
        {
            display.idx = idxOverride;
        }
        else
        {
            display.idx = idxCounter;   
            idxCounter++;
        }
        allSideCards.Add(display);
        display.clickCallback = () => MoveCard(display);
    }

    void CreateCardUpgrade(CardData data, int idx)
    {
        CardDisplay display = Instantiate(cardPrefab, upgradeParent).GetComponent<CardDisplay>();

        for (int i = 0; i < display.cardModifiers.Count; i++)
        {
            display.UpdateCardVisual();
        }

        display.name = data.cardName;
        display.cardData = data;
        display.BindCardData();
        display.BindCardVisualData();
        display.idx = idx;

        allUpgradeCards.Add(display);
        display.clickCallback = () => UpgradeCard(display);
    }

    void UpgradeCard(CardDisplay card)
    {   
        if (unlockedCards.FirstOrDefault(x => x.idx == card.idx && x.cardId == card.cardName) is CardWrapper cw)
        {
            int upgradeCost = int.Parse(card.cardData.upgradeCostShards) * (cw.timesUpgraded + 1);
            if (cw.timesUpgraded >= card.cardData.upgrades.Count)
            {
                WorldSystem.instance.uiManager.UIWarningController.CreateWarning("Card is fully upgraded!");
                return;
            }
            if (WorldSystem.instance.characterManager.shard >= upgradeCost)
            {
                CardFunctionalityData cardModifierData = card.cardData.upgrades[cw.timesUpgraded++];
                cw.cardModifiersId.Add(cardModifierData.id);
                card.AddModifierToCard(cardModifierData);
                WorldSystem.instance.characterManager.shard -= upgradeCost;
                upgradedCardWindow.SetActive(true);
                GridLayoutGroup glg = upgradeParent.GetComponent<GridLayoutGroup>();
                glg.enabled = false;
                upgradedCard.Mimic(card);
                card.gameObject.SetActive(false);

                void Callback()
                {
                    glg.enabled = true;
                    card.gameObject.SetActive(true);
                    upgradedCardWindow.SetActive(false);
                }

                upgradedCardWindow.GetComponent<Button>().onClick.AddListener(() => Callback());
                upgradedCard.clickCallback = Callback;
                WorldSystem.instance.SaveProgression();
            }
            else
            {
                WorldSystem.instance.uiManager.UIWarningController.CreateWarning("Not enough shards!");
            }
        }
    }

    void SortDeck()
    {
        // sort deck
        List<CardDisplay> sorted = allDeckCards.OrderBy(x => x.cardName).ToList();
        Debug.Log("SortDeck");
        int amount = allDeckCards.Count;

        for (int i = 0; i < amount; i++)
        {
            sorted[i].transform.SetSiblingIndex(i);
        }
    
        // sort side
        allSideCards.OrderBy(x => x.cardName);
        amount = allSideCards.Count;

        for (int i = 0; i < amount; i++)
        {
            allSideCards[i].transform.SetSiblingIndex(i);
        }
    }

    void UpdateCounter()
    {
        sideboardAmountText.text = string.Format("{0} / {1}", allDeckCards.Count, maxSideboardCards);
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
        if (allDeckCards.Count >= maxSideboardCards) return;

        List<CardDisplay> sorted = new List<CardDisplay>();
        allDeckCards.ForEach(x => sorted.Add(x));
        sorted.Add(card);
        sorted = sorted.OrderBy(x => x.cardName).ToList();
        card.transform.SetParent(deckParent);
        card.transform.SetSiblingIndex(sorted.IndexOf(card));

        allDeckCards.Add(card);
        allSideCards.Remove(card);
    }
    void MoveToSide(CardDisplay card)
    {
        List<CardDisplay> sorted = new List<CardDisplay>();
        allSideCards.ForEach(x => sorted.Add(x));
        sorted.Add(card);
        sorted = sorted.OrderBy(x => x.cardName).ToList();
        card.transform.SetParent(sideParent);
        card.transform.SetSiblingIndex(sorted.IndexOf(card));
        allSideCards.Add(card);
        allDeckCards.Remove(card);
    }

    public void UpdateDeckManagement()
    {
        CardDisplay display;
        SaveDataManager.LoadJsonData(GetComponents<ISaveableCharacter>(), (int)WorldSystem.instance.characterManager.selectedCharacterClassType);
        allDeckCards.ForEach(x => MoveToDeck(x));
        allDeckCards.Clear();
        List<CardWrapper> uCards = new List<CardWrapper>();

        for (int i = 0; i < unlockedCards.Count; i++)
        {
            if (DatabaseSystem.instance.GetCardByID(unlockedCards[i].cardId) is CardData data)
            {
                if ((int)data.cardClass == (int)WorldSystem.instance.characterManager.selectedCharacterClassType)
                {
                    uCards.Add(unlockedCards[i]);
                }
            }
        }
        
        while (allSideCards.Count < uCards.Count)
        {
            display = Instantiate(cardPrefab, sideParent).GetComponent<CardDisplay>();
            allSideCards.Add(display);
        }
        while (allSideCards.Count > uCards.Count)
        {
            Destroy(allSideCards[allSideCards.Count - 1].gameObject);
            allSideCards.RemoveAt(allSideCards.Count - 1);
        }

        for (int i = 0; i < uCards.Count; i++)
        {
            CreateCardManage(allSideCards[i], DatabaseSystem.instance.GetCardByID(uCards[i].cardId), uCards[i].idx);
        }

        UpdateDeck();
    }

    public void UpdateUpgradeManagement()
    {
        List<CardDisplay> allCards = allDeckCards;
        allCards.ForEach(x => MoveToDeck(x));
        allCards.ForEach(x => CreateCardUpgrade(x.cardData, x.idx));
    }

    void ConfirmDeck()
    {
        WorldSystem.instance.characterManager.playerCardsData = GetDeck();
    }
    public override void CloseBuilding()
    {
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
        UpdateDeckManagement();
        StepInto(deckManagement);
    }
    public void ButtonEnterCardUpgrade()
    {
        UpdateDeckManagement();
        UpdateUpgradeManagement();
        StepInto(cardUpgrade);
    }

    protected override void StepBack()
    {
        if (allDeckCards.Count < maxSideboardCards)
        {
            PromptWarning();
            return;
        }
        base.StepBack();
    }

    public void PopulateSaveDataCharacter(SaveDataCharacter a_SaveData)
    {
        List<CardWrapper> deckCards = new List<CardWrapper>();
        allDeckCards.ForEach(x => deckCards.Add(new CardWrapper(x.cardName, x.idx)));

        List<CardWrapper> sideCards = new List<CardWrapper>();
        allSideCards.ForEach(x => sideCards.Add(new CardWrapper(x.cardName, x.idx)));
        
        a_SaveData.deckCards = deckCards;
        a_SaveData.sideCards = sideCards;
    }

    public void LoadFromSaveDataCharacter(SaveDataCharacter a_SaveData)
    {
        if (a_SaveData.deckCards?.Any() == true)
        {
            deckCards = a_SaveData.deckCards;
            sideCards = a_SaveData.sideCards;
        }
    }

    public void PopulateSaveDataWorld(SaveDataWorld a_SaveData)
    {
        a_SaveData.unlockedCards = unlockedCards;
        a_SaveData.idxCounter = idxCounter;
    }

    public void LoadFromSaveDataWorld(SaveDataWorld a_SaveData)
    {
        idxCounter = a_SaveData.idxCounter;
        if (a_SaveData.unlockedCards?.Any() == true)
        {
            unlockedCards = a_SaveData.unlockedCards;
        }
        else
        {
            UnlockProfessionCard(Profession.Berserker1);
        }
    }
}
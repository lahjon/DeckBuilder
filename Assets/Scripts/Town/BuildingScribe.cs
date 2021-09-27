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
    public List<CardDisplay> allSideCards = new List<CardDisplay>();
    public List<CardDisplay> allDeckCards = new List<CardDisplay>();
    public List<CardData> extraCards = new List<CardData>();
    public Transform deckParent, sideParent, upgradeParent;
    public TMP_Text sideboardAmountText;
    public int maxSideboardAmount;
    public int lockedSideboardAmount;

    void Start()
    {
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
        //ResetDeck();
        List<CardDisplay> cards = new List<CardDisplay>();
        List<CardDisplay> cardsOptional = allSideCards.Concat(allDeckCards).Where(x => x.rarity != Rarity.Starting).ToList();
        List<CardDisplay> cardLocked = allSideCards.Concat(allDeckCards).Where(x => x.rarity == Rarity.Starting).ToList();
        cardsOptional.ForEach(x => {
            x.OnClick = () => MoveCard(x);
            x.selectable = true;
        });
        cardLocked.ForEach(x => {
            x.OnClick = null;
            x.selectable = false;
        });

        foreach (CardDisplay c in allSideCards)
        {
            foreach (CardWrapper cw in deckCards)
            {
                if (cw.idx == c.idx && cw.cardId == c.cardId)
                {
                    cards.Add(c);
                }
            }
        }
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
        display.name = data.cardName;
        display.cardData = data;
        display.BindCardData();
        display.BindCardVisualData();
        display.timesUpgraded = cw.timesUpgraded;

        for (int i = 0; i < display.timesUpgraded; i++)
            display.AddModifierToCard(data.upgrades[i]);

        display.idx = cw.idx;
    }

    void UpgradeCard(CardDisplay card)
    {   
        if (unlockedCards.FirstOrDefault(x => x.idx == card.idx && x.cardId == card.cardId) is CardWrapper cw)
        {
            int upgradeCost = int.Parse(card.cardData.upgradeCostShards) * (cw.timesUpgraded + 1);
            if (cw.timesUpgraded >= card.cardData.upgrades.Count)
            {
                WorldSystem.instance.uiManager.UIWarningController.CreateWarning("Card is fully upgraded!");
                return;
            }
            if (WorldSystem.instance.characterManager.shard >= upgradeCost)
            {
                CardFunctionalityData cardModifierData = card.cardData.upgrades[cw.timesUpgraded];
                cw.cardModifiersId.Add(cardModifierData.id);
                cw.timesUpgraded++;
                card.timesUpgraded++;
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
                upgradedCard.OnClick = Callback;
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
        int amount = allDeckCards.Count;

        for (int i = 0; i < amount; i++)
        {
            sorted[i].transform.SetSiblingIndex(i);
        }
    
        // sort side
        sorted = allSideCards.OrderBy(x => x.cardName).ToList();
        amount = allSideCards.Count;

        for (int i = 0; i < amount; i++)
        {
            sorted[i].transform.SetSiblingIndex(i);
        }
    }

    void UpdateCounter()
    {
        sideboardAmountText.text = string.Format("{0} / {1}", allDeckCards.Count, maxSideboardAmount + lockedSideboardAmount);
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
        //SaveDataManager.LoadJsonData(GetComponents<ISaveableCharacter>(), (int)WorldSystem.instance.characterManager.selectedCharacterClassType);
        ResetDeck();
        List<CardWrapper> uCards = new List<CardWrapper>();

        for (int i = 0; i < unlockedCards.Count; i++)
        {
            if (DatabaseSystem.instance.GetCardByID(unlockedCards[i].cardId) is CardData data)
            {
                if ((int)data.cardClass == (int)WorldSystem.instance.characterManager.selectedCharacterClassType || data.cardClass == CardClassType.Colorless)
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
            CreateCardManage(allSideCards[i], uCards[i]);
        }
    }

    public void UpdateUpgradeManagement()
    {
        foreach (CardDisplay display in allSideCards.Concat(allDeckCards).ToList())
        {
            display.OnClick = () => UpgradeCard(display);
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

    // protected override void StepBack()
    // {
    //     base.StepBack();
    // }

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
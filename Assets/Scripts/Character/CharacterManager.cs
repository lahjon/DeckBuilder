using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class CharacterManager : Manager, ISaveableWorld, ISaveableTemp
{
    int _shard, _gold, _fullEmber, _ember, _armorShard ;
    
    [HideInInspector] public int maxCardReward;
    [HideInInspector] public int defaultDrawCardAmount;

    public CharacterVariablesUI characterVariablesUI;
    public Character character;
    public List<CardVisual> deck = new List<CardVisual>();
    public CharacterClassType selectedCharacterClassType;
    public List<PlayableCharacterData> allCharacterData = new List<PlayableCharacterData>();
    public List<CharacterClassType> unlockedCharacters = new List<CharacterClassType>();
    public List<Profession> unlockedProfessions = new List<Profession>();
    public CharacterSheet characterSheet;
    public CharacterStats characterStats;
    public int currentHealth;
    public Transform cardParent;
    public GameObject cardPrefab;


    protected override void Awake()
    {
        base.Awake();
        world.characterManager = this;
        maxCardReward = 3;
        defaultDrawCardAmount = 5;
    }

    protected override void Start()
    {
        base.Start(); 
        

        SetupCharacterData();

        if (currentHealth <= 0)
        {
            currentHealth = characterStats.GetStat(StatType.Health);
            characterVariablesUI.UpdateCharacterHUD();
        }

        if (unlockedCharacters.Count == 0)
        {
            unlockedCharacters = allCharacterData.Where(x => x.unlocked == true).Select(x => x.classType).ToList();
        }

        if (unlockedProfessions.Count == 0)
        {
            unlockedProfessions.Add(Profession.Berserker1);
            unlockedProfessions.Add(Profession.Splicer1);
            unlockedProfessions.Add(Profession.Rogue1);
            unlockedProfessions.Add(Profession.Beastmaster1);
        }

        world.menuManager.menuCharacter.Init();
    }
    public int fullEmber 
    {
        get
        {
            return _fullEmber;
        }
        set
        {
            _fullEmber = value;
            EventManager.CurrencyChanged(CurrencyType.FullEmber, value);
            characterVariablesUI.UpdateCharacterHUD();
        }
    }
    public int shard 
    {
        get
        {
            return _shard;
        }
        set
        {
            _shard = value;
            EventManager.CurrencyChanged(CurrencyType.Shard, value);
            characterVariablesUI.UpdateCharacterHUD();
        }
    }
    public int gold 
    {
        get
        {
            return _gold;
        }
        set
        {
            _gold = value;
            EventManager.CurrencyChanged(CurrencyType.Gold, value);
            characterVariablesUI?.UpdateCharacterHUD();
        }
    }
    public int armorShard 
    {
        get
        {
            return _armorShard;
        }
        set
        {
            _armorShard = value;
            EventManager.CurrencyChanged(CurrencyType.ArmorShard, value);
            characterVariablesUI?.UpdateCharacterHUD();
        }
    }
    public int ember 
    {
        get
        {
            return _ember;
        }
        set
        {
            _ember = value;
            EventManager.CurrencyChanged(CurrencyType.Ember, value);
            characterVariablesUI?.UpdateCharacterHUD();
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= Mathf.Abs(amount);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            KillCharacter();
        }

        characterVariablesUI.UpdateCharacterHUD();
    }

    public void ResetDeck()
    {
        for (int i = 0; i < cardParent.childCount; i++)
        {
            Destroy(cardParent.GetChild(i).gameObject);
        }
        world.townManager.scribe.UpdateScribe();
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        Debug.Log("healing for: " + amount);

        if (currentHealth > characterStats.GetStat(StatType.Health))
        {
            currentHealth = characterStats.GetStat(StatType.Health);
        }

        characterVariablesUI.UpdateCharacterHUD();
    }
    public void SetupCharacterData(bool fromTown = false)
    {
        character.SetCharacterData((int)selectedCharacterClassType);
        character.name = character.characterData.classType.ToString();

        if (fromTown) currentHealth = characterStats.GetStat(StatType.Health);

        characterVariablesUI.UpdateCharacterHUD();
    }

    public void ClearDeck()
    {
        while (deck.Any())
            RemoveCard(deck[0]);

        for (int i = 0; i < cardParent.childCount; i++)
        {
            Destroy(cardParent.GetChild(i).gameObject);
        }
    }

    public void AddCardToDeck(CardVisual source)
    {
        AddCardToDeck(source.cardData, source.cardModifiers);
    }
    public void AddCardToDeck(CardData data, List<CardFunctionalityData> appliedUpgrades = null)
    {
        CardVisual card = CardVisual.Factory(data, cardParent, appliedUpgrades);

        deck.Add(card);
        WorldSystem.instance.deckDisplayManager.Add(card);
    }

    public void RemoveCard(CardVisual card)
    {
        WorldSystem.instance.deckDisplayManager.Remove(card);
        deck.Remove(card);
    }

    public void AddCardToDeck(CardWrapper cw)
    {
        CardData data = DatabaseSystem.instance.GetCardByID(cw.cardId);
        List<CardFunctionalityData> modifiers = new List<CardFunctionalityData>();
        for (int i = 0; i < cw.timesUpgraded; i++)
            modifiers.Add(data.upgrades[i]);

        CardVisual card = CardVisual.Factory(data, cardParent, modifiers);

        deck.Add(card);
        WorldSystem.instance.deckDisplayManager.Add(card);
    }

    public void KillCharacter()
    {
        WorldStateSystem.SetInDeathScreen();
    }

    public void PopulateSaveDataWorld(SaveDataWorld a_SaveData)
    {
        a_SaveData.shard = _shard;
        a_SaveData.unlockedCharacters = unlockedCharacters;
        a_SaveData.unlockedProfessions = unlockedProfessions;
    }

    public void LoadFromSaveDataWorld(SaveDataWorld a_SaveData)
    {
        _shard = a_SaveData.shard;
        unlockedCharacters = a_SaveData.unlockedCharacters;
        unlockedProfessions = a_SaveData.unlockedProfessions;
    }

    public void PopulateSaveDataTemp(SaveDataTemp a_SaveData)
    {
        List<CardWrapper> cwList = new List<CardWrapper>();
        deck.ForEach(x => cwList.Add(new CardWrapper(x.cardId, x.idx, x.cardModifiers.Select(x => x.id).ToList(), x.timesUpgraded)));
        a_SaveData.playerCards = cwList;
        a_SaveData.selectedCharacterClassType = selectedCharacterClassType;
        a_SaveData.gold = gold;

        a_SaveData.currentHealth = currentHealth;
        a_SaveData.addedHealth = characterStats.GetStat(StatType.Health) - character.characterData.stats.Where(x => x.type == StatType.Health).FirstOrDefault().value;
    
    }

    public void LoadFromSaveDataTemp(SaveDataTemp a_SaveData)
    {
        if (a_SaveData.playerCards != null && a_SaveData.playerCards.Count > 0 && SceneManager.GetActiveScene().buildIndex != 0)
            a_SaveData.playerCards.ForEach(x => AddCardToDeck(x));

        if (a_SaveData.selectedCharacterClassType != CharacterClassType.None)
            selectedCharacterClassType = a_SaveData.selectedCharacterClassType;
        else
            selectedCharacterClassType = CharacterClassType.Berserker;

        _gold = a_SaveData.gold;

        currentHealth = a_SaveData.currentHealth - a_SaveData.addedHealth;
    }
}


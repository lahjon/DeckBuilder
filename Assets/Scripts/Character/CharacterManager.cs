using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class CharacterManager : Manager, ISaveableWorld, ISaveableTemp
{
    int _shard;
    int _gold;
    
    [HideInInspector] public int maxCardReward;
    [HideInInspector] public int defaultDrawCardAmount;

    public CharacterVariablesUI characterVariablesUI;
    public Character character;
    public List<CardData> playerCardsData = new List<CardData>();
    public CharacterClassType selectedCharacterClassType;
    public List<PlayableCharacterData> allCharacterData = new List<PlayableCharacterData>();
    public List<CharacterClassType> unlockedCharacters = new List<CharacterClassType>();
    public List<Profession> unlockedProfessions = new List<Profession>();
    public CharacterSheet characterSheet;
    public CharacterStats characterStats;
    public int currentHealth;


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
        characterStats = character.GetComponent<CharacterStats>();

        if (selectedCharacterClassType == CharacterClassType.None)
        {
            selectedCharacterClassType = CharacterClassType.Berserker;
            //character.profession = Profession.Berserker1;
        }

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

        // NOTE: uncomment when professions are in DB

        if (unlockedProfessions.Count == 0)
        {
            unlockedProfessions.Add(Profession.Berserker1);
            unlockedProfessions.Add(Profession.Splicer1);
            unlockedProfessions.Add(Profession.Rogue1);
            unlockedProfessions.Add(Profession.Beastmaster1);
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
            characterVariablesUI?.UpdateCharacterHUD();
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= Mathf.Abs(amount);
        Debug.Log(currentHealth);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            KillCharacter();
        }

        characterVariablesUI.UpdateCharacterHUD();
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

        if (playerCardsData == null || playerCardsData.Count == 0) 
            playerCardsData.AddRange(DatabaseSystem.instance.GetStartingDeck(selectedCharacterClassType, character.profession));

        if (fromTown)
        {
            playerCardsData.Clear();
            playerCardsData.AddRange(DatabaseSystem.instance.GetStartingDeck(selectedCharacterClassType, character.profession));
            currentHealth = characterStats.GetStat(StatType.Health);
        }
        characterVariablesUI.UpdateCharacterHUD();
    }

    public void AddCardDataToDeck(CardData newCardData)
    {
        playerCardsData.Add(newCardData); 
    }

    public void RemoveCardDataFromDeck(string aCardName)
    {
        if(playerCardsData.Count > 1)
        {
            CardData cardData = playerCardsData.FirstOrDefault(x => x.cardName == aCardName);
            if (cardData != null)
            {
                playerCardsData.RemoveAt(playerCardsData.IndexOf(cardData));
            }
        }
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
        List<string> tempList = new List<string>();
        playerCardsData.ForEach(x => tempList.Add(x.name));
        a_SaveData.playerCardsDataNames = tempList;
        a_SaveData.selectedCharacterClassType = selectedCharacterClassType;
        a_SaveData.gold = gold;
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            a_SaveData.currentHealth = currentHealth;
            a_SaveData.addedHealth = characterStats.GetStat(StatType.Health) - character.characterData.stats.Where(x => x.type == StatType.Health).FirstOrDefault().value;
        }
    }

    public void LoadFromSaveDataTemp(SaveDataTemp a_SaveData)
    {
        if (a_SaveData.playerCardsDataNames != null && a_SaveData.playerCardsDataNames.Count > 0 && SceneManager.GetActiveScene().buildIndex != 0)
            playerCardsData = DatabaseSystem.instance.GetCardsByName(a_SaveData.playerCardsDataNames);


        if (a_SaveData.selectedCharacterClassType != CharacterClassType.None)
            selectedCharacterClassType = a_SaveData.selectedCharacterClassType;

        _gold = a_SaveData.gold;

        currentHealth = a_SaveData.currentHealth - a_SaveData.addedHealth;
    }
}


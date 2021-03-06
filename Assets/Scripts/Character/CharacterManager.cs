using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class CharacterManager : Manager, ISaveableWorld, ISaveableTemp, ISaveableStart
{
    // should match all items from CharacterVariables Enum
    private int _gold;
    private int _shard;

    public int startingGold = 99;
    public int currentHealth;
    public int maxHealth;
    public int maxCardReward = 3;
    public int energy;
    public int startDrawAmount;
    public CharacterVariablesUI characterVariablesUI;
    public Character character;
    public GameObject characterPrefab;
    public List<CardData> playerCardsData;
    public List<string> playerCardsDataNames;
    public CharacterClassType characterClassType;
    public List<string> selectedTokens = new List<string>();

    protected override void Awake()
    {
        base.Awake();
        world.characterManager = this;
    }

    protected override void Start()
    {
        base.Start(); 
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            SetupCharacterData();
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
            characterVariablesUI.UpdateUI();
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
            characterVariablesUI.UpdateUI();
        }
    }

    public void Reset()
    {
        SetupCharacterData();
    }

    private void SetupCharacterData()
    {
        if (character == null)
        {
            character = Instantiate(characterPrefab).GetComponent<Character>();
        }
        characterClassType = character.classType;
        gold = startingGold;
        maxHealth = character.maxHp;
        startDrawAmount = character.characterData.drawCardsAmount;
        energy = character.characterData.energy;
        characterClassType = character.characterData.classType;

        if (playerCardsDataNames == null || playerCardsDataNames.Count == 0)
        {
            character.characterData.startingDeck.allCards.ForEach(x => playerCardsData.Add(x));
            playerCardsData.ForEach(x => playerCardsDataNames.Add(x.name));
        }
        else
        {
            playerCardsData = DatabaseSystem.instance.GetCardsByName(playerCardsDataNames);
        }
    }
    
    public void AddToCharacter(EncounterOutcomeType type, int value)
    {
        if(type.ToString().ToLower() == "gold")
        {
            _gold += value;
            if(_gold < 0)
            {
                _gold = 0;
            }
            characterVariablesUI.UpdateUI();
        }
        else if(type.ToString().ToLower() == "health")
        {
            currentHealth += value;
            if(currentHealth < 1)
            {
                currentHealth = 0;
                KillCharacter();
            }
            characterVariablesUI.UpdateUI();
        }
    }

    public void AddCardDataToDeck(CardData newCardData)
    {
        playerCardsData.Add(newCardData); 
        WorldSystem.instance.deckDisplayManager.UpdateAllCards();
    }
    public void RemoveCardDataFromDeck(int index)
    {
        if(playerCardsDataNames.Count > 1)
        {
            playerCardsDataNames.RemoveAt(index);
            WorldSystem.instance.deckDisplayManager.RemoveCardAtIndex(index);
        }
        else
        {
            Debug.Log("No more cards to remove!");
        }
    }

    public void KillCharacter()
    {
        Debug.Log("You are dead!");
    }

    public void PopulateSaveDataWorld(SaveDataWorld a_SaveData)
    {
        a_SaveData.shard = _shard;
        Debug.Log("Saving Data");
    }

    public void LoadFromSaveDataWorld(SaveDataWorld a_SaveData)
    {
        _shard = a_SaveData.shard;
        Debug.Log("Loading Data");
    }

    public void PopulateSaveDataTemp(SaveDataTemp a_SaveData)
    {
        a_SaveData.playerCardsDataNames = playerCardsDataNames;
        a_SaveData.characterClassType = characterClassType;
    }

    public void LoadFromSaveDataTemp(SaveDataTemp a_SaveData)
    {
        playerCardsDataNames = a_SaveData.playerCardsDataNames;
        characterClassType = a_SaveData.characterClassType;
    }

    public void PopulateSaveDataStart(SaveDataStart a_SaveData)
    {
        a_SaveData.characterClassType = characterClassType;
    }

    public void LoadFromSaveDataStart(SaveDataStart a_SaveData)
    {
        characterClassType = a_SaveData.characterClassType;
    }
}


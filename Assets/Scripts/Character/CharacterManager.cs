using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class CharacterManager : Manager, ISaveableWorld, ISaveableTemp, ISaveableStart
{
    private int _shard;
    private int _gold;

    public CharacterVariablesUI characterVariablesUI;
    public Character character;
    public GameObject characterPrefab;
    public List<CardData> playerCardsData;
    public List<string> playerCardsDataNames;
    public CharacterClassType selectedCharacterClassType = CharacterClassType.Brute;
    public List<PlayableCharacterData> allCharacterData;
    public List<CharacterClassType> unlockedCharacters = new List<CharacterClassType>();
    public CharacterSheet characterSheet;
    int _currentHealth;

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
            if (selectedCharacterClassType == CharacterClassType.None)
            {
                selectedCharacterClassType = CharacterClassType.Brute;
            }
            SetupCharacterData();
            if (!unlockedCharacters.Contains(selectedCharacterClassType))
            {
                unlockedCharacters.Add(selectedCharacterClassType);
            }
            world.SaveProgression();
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

    public void Reset()
    {
        SetupCharacterData();
    }

    void SetupCharacterData()
    {
        if (character == null)
        {
            character = Instantiate(characterPrefab).GetComponent<Character>();
            character.SetCharacterData();

            if (character.level == 0)
            {
                character.CreateStartingCharacter(character.characterData);
            }

            character.name = character.characterData.classType.ToString();
        }
        selectedCharacterClassType = character.classType;
        selectedCharacterClassType = character.characterData.classType;

        if (playerCardsDataNames == null || playerCardsDataNames.Count == 0)
        {
            character.characterData.startingDeck.allCards.ForEach(x => playerCardsData.Add(x));
            playerCardsData.ForEach(x => playerCardsDataNames.Add(x.name));
        }
        else
        {
            playerCardsData = DatabaseSystem.instance.GetCardsByName(playerCardsDataNames);
        }

        if (_currentHealth <= 0)
        {
            character.currentHealth = character.maxHealth;
        }
        else
        {
            character.currentHealth = _currentHealth;
        }

        characterVariablesUI.UpdateCharacterHUD();
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
        a_SaveData.unlockedCharacters = unlockedCharacters;
        Debug.Log("Saving Data");
    }

    public void LoadFromSaveDataWorld(SaveDataWorld a_SaveData)
    {
        _shard = a_SaveData.shard;
        unlockedCharacters = a_SaveData.unlockedCharacters;
        Debug.Log("Loading Data");
    }

    public void PopulateSaveDataTemp(SaveDataTemp a_SaveData)
    {
        a_SaveData.playerCardsDataNames = playerCardsDataNames;
        a_SaveData.selectedCharacterClassType = selectedCharacterClassType;
        a_SaveData.gold = gold;
        a_SaveData.currentHealth = _currentHealth;
    }

    public void LoadFromSaveDataTemp(SaveDataTemp a_SaveData)
    {
        playerCardsDataNames = a_SaveData.playerCardsDataNames;
        selectedCharacterClassType = a_SaveData.selectedCharacterClassType;
        gold = a_SaveData.gold;
        _currentHealth = a_SaveData.currentHealth;
    }

    public void PopulateSaveDataStart(SaveDataStart a_SaveData)
    {
        a_SaveData.characterClassType = selectedCharacterClassType;
    }

    public void LoadFromSaveDataStart(SaveDataStart a_SaveData)
    {
        selectedCharacterClassType = a_SaveData.characterClassType;
    }
}


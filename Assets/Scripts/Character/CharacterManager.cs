using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class CharacterManager : Manager, ISaveableWorld, ISaveableTemp, IEffectAdder
{
    [HideInInspector] public int maxCardReward;
    [HideInInspector] public int defaultDrawCardAmount;
    public List<CardVisual> deck = new List<CardVisual>();
    public CharacterClassType selectedCharacterClassType;
    public List<PlayableCharacterData> allCharacterData { get => DatabaseSystem.instance.allCharacterDatas; }
    public List<CharacterClassType> unlockedCharacters = new List<CharacterClassType>();
    public List<ProfessionType> unlockedProfessions = new List<ProfessionType>();
    public ProfessionType professionType;
    public PlayableCharacterData characterData;
    public CharacterStats characterStats;
    public CharacterCurrency characterCurrency;
    public List<Profession> professions = new List<Profession>();
    public string characterName = "Character";
    public int Experience => world.levelManager.CurrentExperience;
    public int Level => world.levelManager.CurrentLevel;
    [SerializeField]int _currentHealth;

    public Transform cardParent;
    public GameObject cardPrefab;
    public int CurrentHealth
    {
        get => _currentHealth;
        set
        {
            int previousCurrent = _currentHealth;
            _currentHealth = value;
            EventManager.HealthChanged(value - previousCurrent);
        }
    }


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

        if (CurrentHealth <= 0)
        {
            CurrentHealth = CharacterStats.Health;
        }

        if (unlockedCharacters.Count == 0)
        {
            unlockedCharacters = allCharacterData.Where(x => x.unlocked == true).Select(x => x.classType).ToList();
        }

            world.menuManager.menuInventory.Init();
    }

    public void UnlockCharacter(CharacterClassType aCharacterClassType)
    {
        if (!unlockedCharacters.Contains(aCharacterClassType))
        {
            unlockedCharacters.Add(aCharacterClassType);    
        }
    }

    public void UnlockProfession(ProfessionType profession, bool swapTo = false)
    {
        if (!unlockedProfessions.Contains(profession))
        {
            unlockedProfessions.Add(profession);    
            if (swapTo) SwapProfession(profession);
            //WorldSystem.instance.SaveProgression();
        }
    }

    void RemoveProfession()
    {
        if (professionType != ProfessionType.Base && DatabaseSystem.instance.professionDatas.FirstOrDefault(x => x.professionType == professionType) is ProfessionData data)
        {
            professions.ForEach(x => x.RemoveEffects());
            professions.Clear();
        } 
    }

    public void SwapProfession(ProfessionType profession)
    {
        if (unlockedProfessions.Contains(profession) && DatabaseSystem.instance.professionDatas.FirstOrDefault(x => x.professionType == profession) is ProfessionData data)
        {
            RemoveProfession();
            AddProfession(data);
        }
    }

    void AddProfession(ProfessionData professionData)
    {
        professions.Add(Profession.AddProfession(professionData));
        professionType = professionData.professionType;
    }

    public void LooseLife(int amount)
    {
        CurrentHealth -= Mathf.Abs(amount);

        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            KillCharacter();
        }
    }

    public void ResetDeck()
    {
        for (int i = 0; i < cardParent.childCount; i++)
        {
            Destroy(cardParent.GetChild(i).gameObject);
        }
        world.townManager.scribe.UpdateScribe();
    }

    public void Heal(int amount) => CurrentHealth = Mathf.Clamp(CurrentHealth+amount,0,CharacterStats.Health);

    public void SetupCharacterData(bool fromTown = false)
    {
        characterData = WorldSystem.instance.characterManager.allCharacterData[(int)selectedCharacterClassType];
        characterStats.Init();
        SwapProfession(professionType);

        if (fromTown) CurrentHealth = CharacterStats.Health;
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
    public string GetName() => characterName;

    public void NotifyDeregister() { }

    public void NotifyRegister()
    {
    }
    public void NotifyUsed() { }

    public void PopulateSaveDataWorld(SaveDataWorld a_SaveData)
    {
        a_SaveData.unlockedCharacters = unlockedCharacters;
        a_SaveData.unlockedProfessions = unlockedProfessions;
    }

    public void LoadFromSaveDataWorld(SaveDataWorld a_SaveData)
    {
        unlockedCharacters = a_SaveData.unlockedCharacters;
        unlockedProfessions = a_SaveData.unlockedProfessions;
    }

    public void PopulateSaveDataTemp(SaveDataTemp a_SaveData)
    {
        List<CardWrapper> cwList = new List<CardWrapper>();
        deck.ForEach(x => cwList.Add(new CardWrapper(x.cardId, x.idx, x.cardModifiers.Select(x => x.id).ToList(), x.timesUpgraded)));
        a_SaveData.playerCards = cwList;
        a_SaveData.selectedCharacterClassType = selectedCharacterClassType;
        a_SaveData.currentProfessionType = professionType;
        a_SaveData.damageTaken = CurrentHealth - CharacterStats.Health;
    }

    public void LoadFromSaveDataTemp(SaveDataTemp a_SaveData)
    {
        if (a_SaveData.playerCards != null && a_SaveData.playerCards.Count > 0 && SceneManager.GetActiveScene().buildIndex != 0)
            a_SaveData.playerCards.ForEach(x => AddCardToDeck(x));

        if (a_SaveData.selectedCharacterClassType != CharacterClassType.None)
            selectedCharacterClassType = a_SaveData.selectedCharacterClassType;
        else
            selectedCharacterClassType = CharacterClassType.Berserker;

        CurrentHealth = a_SaveData.damageTaken;
        professionType = a_SaveData.currentProfessionType;
        //Debug.Log(CurrentHealth);
    }
}


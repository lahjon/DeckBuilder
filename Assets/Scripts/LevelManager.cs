using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LevelManager : Manager
{
    int _currentLevel;
    int _currentExperience;
    int _unusedLevelPoints;
    int _maxLevel = 20;
    public int currentExperience => _currentExperience;
    public int currentLevel => _currentLevel;
    public int unusedLevelPoints => _unusedLevelPoints;
    public List<int> requiredExperience = new List<int>();
    
    public List<GameObject> bruteLevelReward = new List<GameObject>();
    public List<LevelReward> bruteLevelRewardData = new List<LevelReward>();
    public List<GameObject> rogueLevelReward = new List<GameObject>();
    public List<GameObject> splicerLevelReward = new List<GameObject>();
    public List<GameObject> beastmasterLevelReward = new List<GameObject>();

    protected override void Awake()
    {
        base.Awake();
        world.levelManager = this;
    }

    protected override void Start()
    {
        base.Start();
        _currentLevel = world.characterManager.character.level;
        world.characterManager.characterSheet.Init();
    }

    void SetMaxLevel()
    {

    }

    public void AddExperience(int exp)
    {
        if (_currentLevel >= _maxLevel)
        {
            return;
        }
        _currentExperience += exp;
        if (_currentExperience >= requiredExperience[_currentLevel])
        {
            _currentExperience -= requiredExperience[_currentLevel];
            LevelUp();
        }
        world.characterManager.character.experience = _currentExperience;
    }

    public GameObject GetLevelReward(int level)
    {
        int index = level - 2 - _unusedLevelPoints;

        switch (world.characterManager.character.classType)
        {
            case CharacterClassType.Brute:
                return bruteLevelReward[index];
            case CharacterClassType.Rogue:
                return rogueLevelReward[index];
            case CharacterClassType.Splicer:
                return splicerLevelReward[index];
            case CharacterClassType.Beastmaster:
                return beastmasterLevelReward[index]; 
            default:
                return null;
        }
    }

    public GameObject SpendLevelPoint()
    {
        if (_unusedLevelPoints > 0)
        {
            _unusedLevelPoints--;
            world.characterManager.characterVariablesUI.DisableLevelUp();
            return GetLevelReward(_currentLevel);
        }
        else
        {
            return null;
        }
    }

    public void AddLevel()
    {
        AddExperience(requiredExperience[_currentLevel]);
    }

    public void LevelUp()
    {
        _currentLevel++;
        _unusedLevelPoints++;
        world.characterManager.character.level = _currentLevel;
        world.characterManager.characterSheet.OnLevelUp();
        world.characterManager.characterVariablesUI.ActivateLevelUp();
        EventManager.LevelUp();
        if (_currentLevel >= _maxLevel)
        {
            SetMaxLevel();
        }
    }
}

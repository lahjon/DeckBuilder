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
    
    public List<LevelReward> bruteLevelReward = new List<LevelReward>();
    public List<LevelReward> rogueLevelReward = new List<LevelReward>();
    public List<LevelReward> splicerLevelReward = new List<LevelReward>();
    public List<LevelReward> beastmasterLevelReward = new List<LevelReward>();

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

    public LevelReward GetLevelReward(int level)
    {
        int index = level - 2 - _unusedLevelPoints;

        switch (world.characterManager.character.classType)
        {
            case CharacterClassType.Berserker:
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

    public LevelReward SpendLevelPoint()
    {
        if (_unusedLevelPoints > 0)
        {
            _unusedLevelPoints--;
            world.characterManager.characterVariablesUI.DisableLevelUp();
            LevelReward levelReward = GetLevelReward(_currentLevel);
            return levelReward;
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
        EventManager.LevelUp(world.characterManager.character.classType, _currentLevel);
        if (_currentLevel >= _maxLevel)
        {
            SetMaxLevel();
        }
    }
}

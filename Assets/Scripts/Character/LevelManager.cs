using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LevelManager : Manager
{
    [SerializeField] int _currentLevel;
    public int CurrentLevel => _currentLevel;
    [SerializeField] int _currentExperience;
    public int CurrentExperience => _currentExperience;
    public int requiredExperience;
    int MaxLevel => requiredExperiences.Count;
    [SerializeField] int _unusedLevelPoints;
    [SerializeField] int promptLevel;
    public List<int> requiredExperiences = new List<int>();

    protected override void Awake()
    {
        base.Awake();
        world.levelManager = this;
    }

    protected override void Start()
    {
        base.Start();
        requiredExperience = requiredExperiences[0];
        _currentLevel = 1;
        promptLevel = 1;
        AddExperience(0);
    }

    public void AddExperience(int exp)
    {
        if (_currentLevel >= MaxLevel) return;

        _currentExperience += exp;
        EventManager.ExperiencedChanged(exp);

        if (_currentExperience >= requiredExperiences[_currentLevel - 1] && _currentLevel >= promptLevel)
        {
            promptLevel++;
            PromptLevelUp();
        }
        
    }

    void PromptLevelUp()
    {
        _unusedLevelPoints++;
        world.hudManager.experienceBar.Flash();
    }

    public void GetLevelReward(int level)
    {
        int index = level - 2 - _unusedLevelPoints;
    }

    public void AddLevel()
    {
        AddExperience(requiredExperiences[_currentLevel]);
    }

    public void LevelUp()
    {
        if (_unusedLevelPoints > 0)
        {
            _unusedLevelPoints--;
            _currentLevel++;
            _currentExperience -= requiredExperience;
            requiredExperience = requiredExperiences[_currentLevel - 1];
            AddExperience(0);
            EventManager.LevelUp(_currentLevel);
            if (_unusedLevelPoints <= 0)
                world.hudManager.experienceBar.StopFlash();
        }
    }
}

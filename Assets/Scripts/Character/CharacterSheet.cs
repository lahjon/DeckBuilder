using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CharacterSheet : MonoBehaviour
{

    public TMP_Text strength, endurance, wit, energy, health, characterClass;
    public GameObject canvas;
    public TMP_Text level, experience;
    public Slider slider;
    public Button levelUpButton;
    public Transform levelUpPanel;
    CharacterManager characterManager;
    LevelManager levelManager;
    public List<GameObject> levelUpSlots = new List<GameObject>();
    public GameObject levelRewardPrefab;
    

    public void UpdateCharacterSheet()
    {
        

        strength.text = characterManager.characterStats.GetStat(StatType.Strength).ToString();
        endurance.text = characterManager.characterStats.GetStat(StatType.Endurance).ToString();
        wit.text = characterManager.characterStats.GetStat(StatType.Wit).ToString();
        energy.text = characterManager.characterStats.GetStat(StatType.Energy).ToString();
        health.text = characterManager.currentHealth.ToString() + "/" + characterManager.characterStats.GetStat(StatType.Health).ToString();
        characterClass.text = characterManager.character.classType.ToString();

        float curExp = WorldSystem.instance.levelManager.currentExperience;
        float reqExp = WorldSystem.instance.levelManager.requiredExperience[WorldSystem.instance.levelManager.currentLevel];
        experience.text = string.Format("{0}/{1}", curExp, reqExp);
        float value = curExp/reqExp;
        slider.value = value;
        level.text = WorldSystem.instance.levelManager.currentLevel.ToString();
    }

    public void Init()
    {
        characterManager = WorldSystem.instance.characterManager;
        levelManager = WorldSystem.instance.levelManager;
        for (int i = 1; i < levelManager.currentLevel; i++)
        {
            //Debug.Log(i);
            GameObject reward = levelUpPanel.GetChild(i).gameObject;
            LevelReward data = levelManager.GetLevelReward(i + 1);
            reward.GetComponent<Image>().sprite = data.artwork;
            Effect.GetEffect(reward, data.name, true);
            EnableReward(reward);
        }
    }

    public void OnLevelUp()
    {
        levelUpButton.interactable = true;
    }

    public void ButtonLevelUp()
    {
        LevelReward rewardData = levelManager.SpendLevelPoint();
        //Debug.Log(rewardData);
        GameObject reward = Instantiate(levelRewardPrefab);
        reward.GetComponent<Image>().enabled = true;
        reward.GetComponent<Image>().sprite = rewardData.artwork;
        
        int index = levelManager.currentLevel - levelManager.unusedLevelPoints - 2;
        if (index < 0)
        {
            return;
        }

        GameObject newReward = levelUpPanel.GetChild(index).gameObject;
        Effect.GetEffect(newReward, rewardData.name, true);
        newReward.GetComponent<Image>().sprite = rewardData.artwork;
        reward.transform.SetParent(levelUpPanel);
        reward.transform.localPosition = Vector3.zero;
        reward.transform.localScale = Vector3.zero;
        
        Vector3 newPos = levelUpPanel.GetChild(index).position;
        LeanTween.move(reward, newPos, 0.5f).setEaseInCubic();
        LeanTween.scale(reward, Vector3.one, 0.51f).setEaseInCubic().setOnComplete(() => EnableReward(newReward)).setDestroyOnComplete(true);
        if (levelManager.unusedLevelPoints < 1)
        {
            levelUpButton.interactable = false;
        }
    }

    void EnableReward(GameObject reward)
    {
        reward.GetComponent<Image>().enabled = true;
    }

    public void OpenCharacterSheet()
    {   
        canvas.SetActive(true);
        UpdateCharacterSheet();
    }

    public void CloseCharacterSheet()
    {
        canvas.SetActive(false);
    }

    public void ButtonCloseCharacterSheet()
    {
        WorldStateSystem.SetInCharacterSheet();
    }
}

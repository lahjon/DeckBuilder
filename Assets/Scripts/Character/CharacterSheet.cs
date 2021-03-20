using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CharacterSheet : MonoBehaviour
{

    //public TMP_Text strength, wisdom, endurance, cunning, speed, drawsize, health, handsize, damage, block, charClass, energy;
    public GameObject canvas;
    public TMP_Text level, experience;
    public Slider slider;
    public Button levelUpButton;
    public Transform levelUpPanel;
    CharacterManager characterManager;
    LevelManager levelManager;
    

    public void UpdateCharacterSheet()
    {
        

        // strength.text = characterManager.stats[CharacterStat.Strength].ToString();
        // wisdom.text = characterManager.stats[CharacterStat.Wisdom].ToString();
        // endurance.text = characterManager.stats[CharacterStat.Endurance].ToString();
        // cunning.text = characterManager.stats[CharacterStat.Cunning].ToString();
        // speed.text = characterManager.stats[CharacterStat.Speed].ToString();
        // drawsize.text = characterManager.cardDrawAmount.ToString();
        // health.text = characterManager.currentHealth.ToString() + "/" + characterManager.maxHealth.ToString();
        // handsize.text = characterManager.handSize.ToString();
        // damage.text = characterManager.damageModifier.ToString();
        // block.text = characterManager.blockModifier.ToString();
        //charClass.text = characterManager.selectedCharacterClassType.ToString();
        //energy.text = characterManager.character.energy.ToString();
        float curExp = WorldSystem.instance.levelManager.currentExperience;
        float reqExp = WorldSystem.instance.levelManager.requiredExperience[WorldSystem.instance.levelManager.currentLevel];
        experience.text = string.Format("{0}/{1}", curExp, reqExp);
        float value = curExp/reqExp;
        Debug.Log(curExp);
        Debug.Log(reqExp);
        slider.value = value;
        //slider.maxValue = 100;
        level.text = WorldSystem.instance.levelManager.currentLevel.ToString();
    }

    public void Init()
    {
        characterManager = WorldSystem.instance.characterManager;
        levelManager = WorldSystem.instance.levelManager;
        for (int i = 1; i < levelManager.currentLevel; i++)
        {
            Debug.Log(i);
            GameObject reward = levelUpPanel.GetChild(i).gameObject;
            GameObject data = levelManager.GetLevelReward(i + 1);
            reward.GetComponent<Image>().sprite = data.GetComponent<Image>().sprite;
            reward.SetActive(true);
        }
    }

    public void OnLevelUp()
    {
        levelUpButton.interactable = true;
    }

    public void ButtonLevelUp()
    {

        GameObject reward = Instantiate(levelManager.SpendLevelPoint());
        GameObject newReward = levelUpPanel.GetChild(levelManager.currentLevel - 2).gameObject;
        newReward.GetComponent<Image>().sprite = reward.GetComponent<Image>().sprite;
        reward.transform.SetParent(levelUpPanel);
        reward.transform.localPosition = Vector3.zero;
        reward.transform.localScale = Vector3.zero;
        levelUpButton.interactable = false;
        Vector3 newPos = levelUpPanel.GetChild(levelManager.currentLevel - 2).position;
        LeanTween.move(reward, newPos, 0.5f).setEaseInCubic();
        LeanTween.scale(reward, Vector3.one, 0.51f).setEaseInCubic().setOnComplete(() => newReward.SetActive(true)).setDestroyOnComplete(true);
    }

    IEnumerator LevelUpAnimation()
    {
        yield return null;
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
}

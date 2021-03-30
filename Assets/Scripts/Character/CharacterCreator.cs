using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class CharacterCreator : MonoBehaviour
{
    public List<PlayableCharacterData> allCharacterData;
    public Image artwork;
    public List<GameObject> characterButtons;
    public TMP_Text decriptionText;
    private int selectionIndex = 0;
    PlayableCharacterData selectedCharacterData;
    GameObject currentButton;
    Color selectedColor = new Color(1.0f, 1.0f, 1.0f);
    Color unselectedColor = new Color(0.3f, 0.3f, 0.3f);
    Color disabledColor = new Color(0.0f, 0.0f, 0.0f);
    public Character selectedCharacter;
    public List<Character> allCharacters;
    public StatsController statsController;  
    public GameObject warningPanel;
    WorldSystem world;
    public CameraShake cameraShake;
    bool shake;

    void Start()
    {
        world = WorldSystem.instance;
        //characterClassType = FileManager.GetClassType();

        for (int i = 0; i < allCharacterData.Count; i++)
        {
            characterButtons[i].GetComponent<Image>().sprite = allCharacterData[i].artwork;
        }

        LoadCharacters();
        SelectCharacter(selectionIndex);
        shake = true;
    }


    public void LoadCharacters()
    {
        for (int i = 0; i < allCharacters.Count; i++)
        {
            if (allCharacterData[i].unlocked && !world.characterManager.unlockedCharacters.Contains(allCharacterData[i].classType))
            {
                Debug.Log("Create New");
                allCharacters[i].CreateStartingCharacter(allCharacterData[i]);
                allCharacters[i].unlocked = true;
            }
            else if (world.characterManager.unlockedCharacters.Contains(allCharacterData[i].classType))
            {
                SaveDataManager.LoadJsonData(allCharacters[i].GetComponents<ISaveableCharacter>(), i + 1);
                allCharacters[i].unlocked = true;
            }

            if (!allCharacters[i].unlocked)
            {
                characterButtons[i].GetComponent<Button>().enabled = false;
                characterButtons[i].GetComponent<Image>().color = disabledColor;
            }
        }
    }

    public void SelectCharacter(int index)
    {
        if (shake && index != this.selectionIndex)
        {
            cameraShake.ShakeCamera();
        }
        currentButton = characterButtons[index];
        selectedCharacterData = allCharacterData[index];
        decriptionText.text = selectedCharacterData.description;
        selectedCharacter = allCharacters[index];
        world.characterManager.character = selectedCharacter;
        selectedCharacter.SetCharacterData(index, selectedCharacterData);
        selectionIndex = index;
        statsController.UpdateStats();
        UpdateButtons();
    }

    void UpdateButtons()
    {
        for (int i = 0; i < allCharacters.Count; i++)
        {
            if (allCharacters[i].unlocked)
            {
                characterButtons[i].GetComponent<Image>().color = unselectedColor;
            }
        }
        currentButton.GetComponent<Image>().color = selectedColor;
    }
    public void StartGame(bool resetData)
    {
        if (resetData)
        {
            FileManager.ResetTempData();
        }
        world.characterManager.selectedCharacterClassType = selectedCharacter.classType;
        world.characterManager.character = selectedCharacter;
        world.SaveProgression();
        LevelLoader.instance.LoadNewLevel();
    }

    public void Confirm()
    {
        if (world.characterManager.selectedCharacterClassType == selectedCharacter.classType || world.characterManager.selectedCharacterClassType == CharacterClassType.None)
        {
            StartGame(false);
        }
        else
        {
            warningPanel.SetActive(true);
        }
    }
}

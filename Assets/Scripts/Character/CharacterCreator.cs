using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class CharacterCreator : MonoBehaviour
{
    public Image artwork;
    public List<GameObject> characterButtons;
    public TMP_Text decriptionText;
    private int selectionIndex = 0;
    public PlayableCharacterData selectedCharacterData;
    public CharacterClassType selectedClassType;
    GameObject currentButton;
    Color selectedColor = new Color(1.0f, 1.0f, 1.0f);
    Color unselectedColor = new Color(0.3f, 0.3f, 0.3f);
    Color disabledColor = new Color(0.0f, 0.0f, 0.0f);
    public StatsController statsController;  
    public GameObject warningPanel;
    WorldSystem world;
    public CameraShake cameraShake;
    bool shake;

    void Start()
    {
        world = WorldSystem.instance;

        for (int i = 0; i < world.characterManager.allCharacterData.Count; i++)
        {
            characterButtons[i].GetComponent<Image>().sprite = world.characterManager.allCharacterData[i].artwork;
        }

        LoadCharacters();
        SelectCharacter(selectionIndex);
        shake = true;
    }


    public void LoadCharacters()
    {
        for (int i = 0; i < world.characterManager.allCharacterData.Count; i++)
        {
            if (world.characterManager.allCharacterData[i].unlocked && !world.characterManager.unlockedCharacters.Contains(world.characterManager.allCharacterData[i].classType))
            {
                //Debug.Log("Create New");
                //Character.CreateStartingCharacter(world.characterManager.allCharacterData[i]);
                world.characterManager.allCharacterData[i].unlocked = true;
            }
            else if (world.characterManager.unlockedCharacters.Contains(world.characterManager.allCharacterData[i].classType))
            {
                //SaveDataManager.LoadJsonData(world.characterManager.character.GetComponents<ISaveableCharacter>(), i + 1);
                world.characterManager.allCharacterData[i].unlocked = true;
            }

            if (!world.characterManager.allCharacterData[i].unlocked)
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
        selectedCharacterData = world.characterManager.allCharacterData[index];
        decriptionText.text = selectedCharacterData.description;
        selectedClassType = (CharacterClassType)index + 1;
        selectionIndex = index;
        statsController.UpdateStats();
        UpdateButtons();
    }

    void UpdateButtons()
    {
        for (int i = 0; i < world.characterManager.allCharacterData.Count; i++)
        {
            if (world.characterManager.allCharacterData[i].unlocked)
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
        world.characterManager.selectedCharacterClassType = selectedClassType;
        LevelLoader.instance.LoadNewLevel();
    }

    public void Confirm()
    {
        if (world.characterManager.selectedCharacterClassType == selectedClassType || world.characterManager.selectedCharacterClassType == CharacterClassType.None)
        {
            StartGame(false);
        }
        else
        {
            warningPanel.SetActive(true);
        }
    }
}

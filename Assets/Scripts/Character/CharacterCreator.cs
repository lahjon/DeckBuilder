using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class CharacterCreator : MonoBehaviour
{
    public List<CharacterData> allCharacterData;
    public Image artwork;
    public List<GameObject> characterButtons;
    public TMP_Text decriptionText;
    private int index = 0;
    CharacterData selectedCharacterData;
    GameObject currentButton;
    Color selectedColor = new Color(1.0f, 1.0f, 1.0f);
    Color unselectedColor = new Color(0.3f, 0.3f, 0.3f);
    Color disabledColor = new Color(0.0f, 0.0f, 0.0f);
    public Character selectedCharacter;
    public List<Character> allCharacters;
    public StatsController statsController;  
    public LevelLoader levelLoader;  
    public GameObject warningPanel;
    WorldSystem world;

    void Update()
    {
        //DEBUG
        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveCharacter();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadCharacters();
        }
    }

    void Start()
    {
        world = WorldSystem.instance;
        //characterClassType = FileManager.GetClassType();

        for (int i = 0; i < allCharacterData.Count; i++)
        {
            characterButtons[i].GetComponent<Image>().sprite = allCharacterData[i].artwork;
        }

        LoadCharacters();
        SelectCharacter(index);
    }


    public void LoadCharacters()
    {
        for (int i = 0; i < allCharacters.Count; i++)
        {
            if (allCharacterData[i].unlocked && !StatsTrackerSystem.unlockedCharacters.Contains(allCharacterData[i].classType))
            {
                allCharacters[i].CreateStartingCharacter(allCharacterData[i]);
                StatsTrackerSystem.unlockedCharacters.Add(allCharacters[i].classType);
                allCharacters[i].unlocked = true;
            }
            else if (StatsTrackerSystem.unlockedCharacters.Contains(allCharacterData[i].classType))
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

    public void SaveCharacter()
    {
        SaveDataManager.SaveJsonData(selectedCharacter.GetComponents<ISaveableCharacter>(), (int)selectedCharacter.classType);
        world.SaveProgression();
    }

    public void SelectCharacter(int index)
    {
        currentButton = characterButtons[index];
        selectedCharacterData = allCharacterData[index];
        decriptionText.text = selectedCharacterData.description;
        selectedCharacter = allCharacters[index];
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

    // void SetCharacter()
    // {
    //     foreach (Character item in allCharacters)
    //     {
    //         if (item != selectedCharacter)
    //         {
    //             DestroyImmediate(item.gameObject);
    //         }
    //     }
    // }

    public void StartGame(bool resetData)
    {
        if (resetData)
        {
            FileManager.ResetTempData();
        }
        world.character = selectedCharacter;
        world.characterClassType = selectedCharacter.classType;
        levelLoader.LoadNewLevel();
    }

    public void Confirm()
    {
        if (world.characterClassType == selectedCharacter.classType || world.characterClassType == CharacterClassType.None)
        {
            Debug.Log("Same");
            StartGame(false);
        }
        else
        {
            Debug.Log("NotSame");
            warningPanel.SetActive(true);
        }
    }
}

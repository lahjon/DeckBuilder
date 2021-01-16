using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCreator : MonoBehaviour
{
    public List<CharacterData> characterData;
    public CanvasController aCanvasController;
    public StatsController statsController;
    public CharacterTypesUI characterTypesUI;
    public GameObject characterPrefab;
    private Dropdown[] myDropdownList;
    public CharacterData selectedChar;
    private CharacterData previousSelectedChar;

    public Image artwork;
    
    private void SetDropDown()
    {
        List<string> myDropdownOptions = new List<string>{};
        myDropdownList = GetComponentsInChildren<Dropdown>();

        foreach(string name in System.Enum.GetNames(typeof(CharacterType)))  
        {  
            myDropdownOptions.Add(name);
        }  

        myDropdownList[0].ClearOptions();
        myDropdownList[0].AddOptions(myDropdownOptions);
    }

    public void SelectCharacterDropDown()
    {
        foreach(CharacterData character in characterData)
        {
            if(character.name == myDropdownList[0].options[myDropdownList[0].value].text)
            {
                previousSelectedChar = selectedChar;
                selectedChar = character;
            }
        }

        SetData(selectedChar); 
    }
    public void SelectCharacter(CharacterData character)
    {
        previousSelectedChar = selectedChar;
        selectedChar = character;
        if(ResetData())
            SetData(selectedChar);
    }

    private void SetData(CharacterData character)
    {
        artwork.sprite = selectedChar.artwork;
    }

    private bool ResetData()
    {
        if(previousSelectedChar != selectedChar)
        {
            statsController.ResetPoints();
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ConfirmCreation()
    {
        if(statsController.statPoints == 0)
        {
            Dictionary<string, int> myStats = statsController.FetchStats();

            WorldSystem.instance.StoreCharacter(myStats, selectedChar, characterPrefab);
            WorldSystem.instance.CreateCharacter();
            WorldSystem.instance.LoadByIndex(1);

        }
        else
        {
            StartFailed();
        }
    }

    void StartFailed()
    {
        Debug.Log("FAILURE!");
    }

    void Start()
    {
        selectedChar = characterData[0];
        SetData(selectedChar);
    }

}
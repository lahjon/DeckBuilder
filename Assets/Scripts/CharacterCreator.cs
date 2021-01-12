using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCreator : MonoBehaviour
{
    public List<CharacterData> characterData;
    public StatsController statsController;
    public CharacterTypesUI characterTypesUI;
    public GameObject characterPrefab;
    private Dropdown[] myDropdownList;
    private CharacterData selectedChar;
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

    public void CreateCharacter()
    {
        if(statsController.statPoints == 0)
        {
            Dictionary<string, int> myStats = statsController.FetchStats();

            ConfirmCharacter(myStats, selectedChar);

        }
        else
        {
            ConfirmFailed();
        }
    }

    void ConfirmCharacter(Dictionary<string, int> myStats, CharacterData aCharacter)
    {
        foreach (KeyValuePair<string, int> item in myStats)
            {
                
                Debug.Log(item.Key + "," + item.Value);
            }

            Debug.Log(aCharacter);
            GameObject newCharacterPrefab = Instantiate(characterPrefab, new Vector3(500, 300, -60), Quaternion.identity);
            
            Character newCharacter = newCharacterPrefab.GetComponent<Character>();

            //newCharacter.strength = 
            
            WorldManager.instance.aCharacter = newCharacter;
    }

    void ConfirmFailed()
    {
        Debug.Log("FAILURE!");
    }

    void Start()
    {
        selectedChar = characterData[0];
        SetData(selectedChar);
    }

}

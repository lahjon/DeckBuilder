using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCreator : MonoBehaviour
{
    public List<CharacterData> characterData;
    private Dropdown[] myDropdownList;
    private CharacterData selectedChar;

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

    public void SelectCharacter()
    {
        foreach(CharacterData character in characterData)
        {
            if(character.name == myDropdownList[0].options[myDropdownList[0].value].text)
            {
                selectedChar = character;
                Debug.Log(character.strength);   
            }

        }

        artwork.sprite = selectedChar.artwork;

    }

    void Start()
    {
        SetDropDown();
        SelectCharacter();
        

    }

}

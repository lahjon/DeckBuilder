using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectableCharacter : MonoBehaviour
{
    bool _unlocked;
    public CharacterClassType characterClassType;
    public bool unlocked
    {
        get => _unlocked;
        set 
        {
            _unlocked = value;
            if (_unlocked) GetComponent<Button>().interactable = true;
            else GetComponent<Button>().interactable = false;
        }
    }


    public void ButtonSelectCharacter()
    {
        Debug.Log("Selected Character");
    }
}

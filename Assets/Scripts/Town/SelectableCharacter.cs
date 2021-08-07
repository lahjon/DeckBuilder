using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectableCharacter : MonoBehaviour
{
    bool _unlocked;
    public CharacterClassType characterClassType;
    Image image;
    static Color colorDeselected = new Color(.5f, .5f, .5f, 1f);
    static Color colorSelected = new Color(1f, 1f, 1f, 1f);
    public static BuildingBarracks buildingBarracks;
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

    void Awake()
    {
        image = GetComponent<Image>();
    }

    public void SelectCharacter()
    {
        image.color = colorSelected;
    }

    public void DeselectCharacter()
    {
        image.color = colorDeselected;
    }


    public void ButtonSelectCharacter()
    {
        buildingBarracks.SelectCharacter(characterClassType);
        Debug.Log("Selected Character: " + characterClassType);
    }
}

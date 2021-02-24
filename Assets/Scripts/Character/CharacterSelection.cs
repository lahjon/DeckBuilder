using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour
{
    public List<CharacterData> characterData;
    public CharacterData selectedChar;
    public Image artwork;

    private int index;
    public GameObject leftArrow;
    public GameObject rightArrow;

    void Start()
    {
        artwork.sprite = characterData[0].artwork; 
        selectedChar = characterData[0];
    }

    public void Next()
    {
        if(index < characterData.Count)
        {
            index++;
            UpdateSelection();
        }
    }

    public void Previous()
    {
        if(index > characterData.Count)
        {
            index--;
            UpdateSelection();
        }
    }

    void UpdateSelection()
    {
        selectedChar = characterData[index];
    }

}

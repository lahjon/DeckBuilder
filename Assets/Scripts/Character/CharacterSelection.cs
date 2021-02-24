using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour
{
    public List<CharacterData> characterData;
    public CharacterData selectedChar;
    public Image artwork;

    int count;

    private int index;
    public Button leftArrow;
    public Button rightArrow;

    void Start()
    {
        index = 0;
        artwork.sprite = characterData[0].artwork; 
        selectedChar = characterData[0];
        count = characterData.Count - 1;
        leftArrow.interactable = false;
    }

    public void Next()
    {

        index++;
        UpdateSelection();
        if (index == count)
        {
            Debug.Log("Index Max");
            rightArrow.interactable = false;
        }
        if(!leftArrow.IsInteractable())
        {
            leftArrow.interactable = true;
        }
    }

    public void Previous()
    {
        index--;
        UpdateSelection();
        if (index == 0)
        {
            Debug.Log("Index Min");
            leftArrow.interactable = false;
        }
        if(!rightArrow.IsInteractable())
        {
            rightArrow.interactable = true;
        }
    }

    void UpdateSelection()
    {
        selectedChar = characterData[index];
        artwork.sprite = selectedChar.artwork;
    }

}

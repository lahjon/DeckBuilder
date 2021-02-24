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
    public GameObject leftArrow;
    public GameObject rightArrow;

    void Start()
    {
        artwork.sprite = characterData[0].artwork; 
        selectedChar = characterData[0];
        count = characterData.Count - 1;
    }

    public void Next()
    {
        if(index < count)
        {
            index++;
            UpdateSelection();
            if (index == count)
            {
                Debug.Log("Index Max");
            }
        }
    }

    public void Previous()
    {
        if(index > count)
        {
            index--;
            UpdateSelection();
            if (index == 0)
            {
                Debug.Log("Index Min");
            }
        }
    }

    void UpdateSelection()
    {
        selectedChar = characterData[index];
        artwork.sprite = selectedChar.artwork;
    }

}

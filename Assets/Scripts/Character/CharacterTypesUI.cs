using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class CharacterTypesUI : MonoBehaviour
{
    public EventSystem eventSystem;
    private GameObject lastSelectedObject;     
    public List<TMP_Text> allTexts;

    public void HightlightButton(TMP_Text aText)
    {
        allTexts.ForEach(x => {if(x.fontStyle == FontStyles.Bold) x.fontStyle = FontStyles.Normal;});
        aText.fontStyle = FontStyles.Bold;
    }

    public void ButtonHover(GameObject aButton)
    {
        Debug.Log(aButton.name);
    }
}

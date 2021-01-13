using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Encounter : MonoBehaviour
{
    public EncounterData encounterData;
    public SpriteRenderer spriteRenderer; 
    public Material matHighlight;
    public Material matNormal;
    
    private bool highlighted;
    private EncounterTypes encounterType;

    void OnMouseOver()
    {
        if(!highlighted)
            SetHighlightedMat();
        highlighted = true;
    }

    void Awake()
    {
        encounterType = encounterData.type;
        UpdateIcon();
    }
    void OnMouseExit()
    {
        highlighted = false;
        SetNormalMat();
    }

    void OnMouseDown()
    {
        Debug.Log("Mouse is pressed on GameObject.");
    }

    void SetHighlightedMat()
    {
        GetComponent<Renderer>().material = matHighlight;
    }
    void SetNormalMat()
    {
        GetComponent<Renderer>().material = matNormal;
    }

    void UpdateIcon()
    {
        List<Sprite> allIcons = DatabaseSystem.instance.iconDatabase.allIcons;
        Sprite icon = allIcons.Where(x => x.name == encounterType.ToString()).FirstOrDefault();
        spriteRenderer.sprite = icon;
    }
}

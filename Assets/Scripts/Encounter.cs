using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Encounter : MonoBehaviour
{
    public EncounterData encounterData;
    public Material matHighlight;
    public Material matNormal;
    private bool highlighted;

    void OnMouseOver()
    {
        if(!highlighted)
            SetHighlightedMat();
        highlighted = true;
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
}

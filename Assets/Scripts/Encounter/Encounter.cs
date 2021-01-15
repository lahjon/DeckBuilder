using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Encounter : MonoBehaviour
{
    public EncounterData encounterData;
    public EncounterManager encounterManager;
    public SpriteRenderer spriteRenderer; 
    public Material matHighlight;
    public Material matNormal;
    public Material matCleared;
    public List<Encounter> neighbourEncounters;
    [HideInInspector]
    public EncounterUI encounterUI;
    //[HideInInspector]
    public GameObject newEncounterUIPrefab;
    private bool highlighted;
    private EncounterType encounterType;
    private bool isCleared;
    private bool isClicked;

    void Awake()
    {
        if(gameObject.GetComponent<Encounter>() == encounterManager.allEncounters[0])
            SetIsCleared();
        encounterType = encounterData.type;
        encounterUI = encounterData.encounterUI;

        isCleared = encounterData.isCleared;
    }

    void Start()
    {
        UpdateIcon();
    }
    void OnMouseOver()
    {
        
        if(!isCleared)
        {
            if(!highlighted)
                SetHighlightedMat();
            highlighted = true;
        }

    }

    bool CheckViablePath(Encounter anEncounter)
    {
        foreach (Encounter x in encounterManager.currentEncounter.neighbourEncounters)
        {
            if(x == anEncounter)
                return true;
        }
        return false;

    }

    void OnMouseExit()
    {
        if(!isCleared)
        {
            highlighted = false;
            SetNormalMat();
        }
    }

    void OnMouseDown()
    {
        //WorldSystem.instance.character.MoveToLocation(gameObject.transform.position, this);
        //Debug.Log("Mouse is pressed on GameObject.");
        if(!isCleared && CheckViablePath(this) && !isClicked)
        {
            isClicked = true;
            CreateUI();
        }
    }

    void CreateUI()
    {
        if(encounterUI.gameObject != null)
        {
            newEncounterUIPrefab = Instantiate(encounterUI.gameObject, new Vector3(0, 0, 0), Quaternion.identity);
            
            encounterUI = newEncounterUIPrefab.GetComponent<EncounterUI>();
            encounterUI.encounterData = encounterData;
            encounterUI.encounter = this;
            WorldSystem.instance.worldState = WorldState.Menu;
            encounterUI.UpdateUI();
            Debug.Log(newEncounterUIPrefab);
        }
    }

    public void DestroyUI()
    {
        Destroy(newEncounterUIPrefab, 0.2f);
        isCleared = true;
        encounterUI = encounterData.encounterUI;
    }

    void SetHighlightedMat()
    {
        GetComponent<Renderer>().material = matHighlight;
    }
    void SetNormalMat()
    {
        GetComponent<Renderer>().material = matNormal;
    }
    public void SetIsCleared()
    {
        DestroyUI();
        GetComponent<Renderer>().material = matCleared;
        encounterManager.currentEncounter = this;
    }

    public void UpdateIcon()
    {
        List<Sprite> allIcons = DatabaseSystem.instance.iconDatabase.allIcons;
        Sprite icon = allIcons.Where(x => x.name == encounterType.ToString()).FirstOrDefault();
        spriteRenderer.sprite = icon;
    }
}

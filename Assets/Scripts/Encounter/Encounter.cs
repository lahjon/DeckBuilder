using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class Encounter : MonoBehaviour
{
    public EncounterData encounterData;
    public SpriteRenderer spriteRenderer; 
    public Material matHighlight;
    public Material matNormal;
    public Material matVisited;
    public List<Encounter> neighbourEncounters;
    
    [HideInInspector]
    public EncounterUI encounterUI;
    //[HideInInspector]
    public GameObject newEncounterUIPrefab;
    protected bool highlighted;
    [HideInInspector]
    public EncounterType encounterType;
    protected bool isVisited;
    protected bool isClicked;

    public void UpdateEncounters()
    {
        if(gameObject.GetComponent<Encounter>() == WorldSystem.instance.encounterManager.allEncounters[0])
        {
            SetIsVisited(false);
        }
        encounterType = encounterData.type;
        encounterUI = encounterData.encounterUI;

        isVisited = encounterData.isVisited;
        UpdateIcon();
    }

    void Start()
    {
        //DEBUG
        if(WorldSystem.instance.previousState != WorldState.MainMenu)
            UpdateEncounters();
    }
    protected abstract void OnMouseOver();

    protected abstract bool CheckViablePath(Encounter anEncounter);

    void OnMouseExit()
    {
        if(!isVisited)
        {
            highlighted = false;
            SetNormalMat();
        }
    }

    protected abstract void OnMouseDown();

    protected void CreateUI()
    {
        if(encounterData.encounterUI != null)
        {
            newEncounterUIPrefab = Instantiate(encounterUI.gameObject, new Vector3(0, 0, 0), Quaternion.identity);
            
            encounterUI = newEncounterUIPrefab.GetComponent<EncounterUI>();
            encounterUI.encounterData = encounterData;
            encounterUI.encounter = this;
            WorldSystem.instance.SwapState(WorldState.Menu);
            encounterUI.UpdateUI();
        }
    }
    public void DestroyUI()
    {
        WorldSystem.instance.SwapStatePrevious();
        Destroy(newEncounterUIPrefab, 0.2f);
        encounterUI = encounterData.encounterUI;
    }

    protected void SetHighlightedMat()
    {
        GetComponent<Renderer>().material = matHighlight;
    }
    void SetNormalMat()
    {
        GetComponent<Renderer>().material = matNormal;
    }
    public void SetIsVisited(bool destroyUI)
    {
        isVisited = true;
        if(destroyUI)
            DestroyUI();
        GetComponent<Renderer>().material = matVisited;
        WorldSystem.instance.encounterManager.currentEncounter = this;
    }

    public void UpdateIcon()
    {
        List<Sprite> allIcons = DatabaseSystem.instance.iconDatabase.allIcons;
        Sprite icon = allIcons.Where(x => x.name == encounterType.ToString()).FirstOrDefault();
        spriteRenderer.sprite = icon;
    }
}

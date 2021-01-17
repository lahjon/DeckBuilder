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

    public void UpdateEncounters()
    {
        if(gameObject.GetComponent<Encounter>() == WorldSystem.instance.encounterManager.allEncounters[0])
        {
            SetIsCleared(false);
        }
        encounterType = encounterData.type;
        encounterUI = encounterData.encounterUI;

        isCleared = encounterData.isCleared;
        UpdateIcon();
    }

    void Start()
    {
        //DEBUG
        if(WorldSystem.instance.previousState != WorldState.MainMenu)
            UpdateEncounters();
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
        foreach (Encounter x in WorldSystem.instance.encounterManager.currentEncounter.neighbourEncounters)
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
        if(!isCleared && CheckViablePath(this) && !isClicked)
        {
            switch (this.encounterType)
            {
                case EncounterType.NormalCombat:
                    Debug.Log("Enter Combat!");
                    SetIsCleared(false);
                    break;
                
                case EncounterType.EliteCombat:
                    Debug.Log("Enter Elite Combat!");
                    SetIsCleared(false);
                    break;
                
                case EncounterType.BossCombat:
                    Debug.Log("Enter Boss Combat!");
                    SetIsCleared(false);
                    break;

                case EncounterType.Shop:
                    WorldSystem.instance.shopManager.shop.gameObject.SetActive(true);
                    WorldSystem.instance.shopManager.shop.RestockShop();
                    SetIsCleared(false);
                    WorldSystem.instance.SwapState(WorldState.Shop);
                    Debug.Log("Enter Shop!");
                    break;
                
                default:
                    isClicked = true;
                    CreateUI();
                    break;
            }
        }
    }

    void CreateUI()
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
        WorldSystem.instance.SwapState();
        Destroy(newEncounterUIPrefab, 0.2f);
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
    public void SetIsCleared(bool destroyUI)
    {
        isCleared = true;
        if(destroyUI)
            DestroyUI();
        GetComponent<Renderer>().material = matCleared;
        WorldSystem.instance.encounterManager.currentEncounter = this;
    }

    public void UpdateIcon()
    {
        List<Sprite> allIcons = DatabaseSystem.instance.iconDatabase.allIcons;
        Sprite icon = allIcons.Where(x => x.name == encounterType.ToString()).FirstOrDefault();
        spriteRenderer.sprite = icon;
    }
}

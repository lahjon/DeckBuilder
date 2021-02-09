using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public abstract class Encounter : MonoBehaviour
{
    public EncounterData encounterData;
    public Sprite sprite; 
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

    public List<GameObject> roads = new List<GameObject>();

    public abstract void UpdateEncounter();

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

    // Åååååååh Fredrik

    // protected void CreateUI()
    // {
    //     if(encounterData.encounterUI != null)
    //     {
    //         newEncounterUIPrefab = Instantiate(encounterUI.gameObject, new Vector3(0, 0, 0), Quaternion.identity);
            
    //         encounterUI = newEncounterUIPrefab.GetComponent<EncounterUI>();
    //         encounterUI.encounterData = encounterData;
    //         encounterUI.encounter = this;
    //         WorldSystem.instance.SwapState(WorldState.Menu);
    //         encounterUI.UpdateUI();
    //     }
    // }
    // public void DestroyUI()
    // {
    //     WorldSystem.instance.SwapStatePrevious();
    //     Destroy(newEncounterUIPrefab, 0.2f);
    //     encounterUI = encounterData.encounterUI;
    // }

    protected void SetHighlightedMat()
    {
        //GetComponent<Renderer>().material = matHighlight;
    }
    protected void SetNormalMat()
    {
        //GetComponent<Renderer>().material = matNormal;
    }
    public void SetIsVisited(bool destroyUI)
    {
        isVisited = true;
        // if(destroyUI)
        //     DestroyUI();
        //GetComponent<Renderer>().material = matVisited;
        Button button = this.GetComponent<Button>();
        ColorBlock color = button.colors;

        color.normalColor = new Color (1f, 0.5f, 0.5f);
        color.disabledColor = new Color (1f, 0.5f, 0.5f);
        color.selectedColor = new Color (1f, 0.5f, 0.5f);
        button.colors = color;


        if(this.roads.Count > 0)
        {
            
            // for (int i = 0; i < this.roads.transform.childCount; i++)
            // {
            //     this.roads.transform.GetChild(i).GetComponent<Image>().color = new Color (0.5f, 0.5f, 0.5f);
            // }

        }

        WorldSystem.instance.encounterManager.currentEncounter = this;


    }

    public void UpdateIcon()
    {
        List<Sprite> allIcons = DatabaseSystem.instance.iconDatabase.allIcons;
        Sprite icon = allIcons.Where(x => x.name == encounterType.ToString()).FirstOrDefault();
        this.GetComponent<Image>().sprite = icon;
    }
}

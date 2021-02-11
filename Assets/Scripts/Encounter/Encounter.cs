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

    //public Dictionary<Encounter, GameObject> roads = new Dictionary<Encounter, GameObject>();
    public Dictionary<GameObject, List<Encounter>> roads = new Dictionary<GameObject, List<Encounter>>();

    public abstract void UpdateEncounter();

    protected abstract void OnMouseOver();

    protected abstract bool CheckViablePath(Encounter anEncounter);


    protected delegate void VisitAction();

    void OnMouseExit()
    {
        if(!isVisited)
        {
            highlighted = false;
            SetNormalMat();
        }
    }
    public abstract void ButtonPress();

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
    public void SetIsVisited(Encounter enc = null)
    {
        isVisited = true;

        Button button = this.GetComponent<Button>();
        ColorBlock color = button.colors;

        color.normalColor = new Color (1.0f, 0.5f, 0.5f);
        color.disabledColor = new Color (1.0f, 0.5f, 0.5f);
        color.selectedColor = new Color (1.0f, 0.5f, 0.5f);
        button.colors = color;
        

        if(enc != null && roads.Count > 0)
        {
            foreach(KeyValuePair<GameObject, List<Encounter>> p in roads)
            {
                if(p.Value[0] == WorldSystem.instance.encounterManager.currentEncounter && p.Value[1] == this)
                {
                    for (int i = 0; i < p.Key.transform.childCount; i++)
                    {
                        p.Key.transform.GetChild(i).GetComponent<Image>().color = new Color (1.0f, 0.5f, 0.5f);
                    }
                }
                break;
            }
        }

        WorldSystem.instance.encounterManager.currentEncounter = this;

    }

    protected IEnumerator SetVisited(List<System.Action> visitActions, Encounter enc = null)
    {
        isVisited = true;

        if(enc != null && roads.Count > 0)
        {
            foreach(KeyValuePair<GameObject, List<Encounter>> p in roads)
            {
                
                if(p.Value[0] == WorldSystem.instance.encounterManager.currentEncounter && p.Value[1] == this)
                {
                    int counter = 0;
                    int countMax = p.Key.transform.childCount;
                    while (counter < countMax)
                    {
                        p.Key.transform.GetChild(counter).GetComponent<Image>().color = new Color (1.0f, 0.5f, 0.5f);
                        counter++;
                        yield return new WaitForSeconds(0.1f);
                    }
                    break;
                }
            }
        }   

        Button button = this.GetComponent<Button>();
        ColorBlock color = button.colors;

        color.normalColor = new Color (1.0f, 0.5f, 0.5f);
        color.disabledColor = new Color (1.0f, 0.5f, 0.5f);
        color.selectedColor = new Color (1.0f, 0.5f, 0.5f);
        button.colors = color;

        WorldSystem.instance.encounterManager.currentEncounter = this;
        Debug.Log("Doing Action");
        visitActions.ForEach(x => x.Invoke());
    }

    public void UpdateIcon()
    {
        List<Sprite> allIcons = DatabaseSystem.instance.iconDatabase.allIcons;
        Sprite icon = allIcons.Where(x => x.name == encounterType.ToString()).FirstOrDefault();
        this.GetComponent<Image>().sprite = icon;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public abstract class Encounter : MonoBehaviour
{
    [HideInInspector]
    public List<Encounter> neighbourEncounters;
    public EncounterData encounterData;
    public EncounterType encounterType;
    protected bool isVisited;
    protected bool isClicked;
    public Dictionary<GameObject, List<Encounter>> roads = new Dictionary<GameObject, List<Encounter>>();
    protected delegate void VisitAction();

    public abstract void UpdateEncounter();

    protected virtual bool CheckViablePath(Encounter anEncounter)
    {
        return false;
    }

    public abstract void ButtonPress();
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

    protected IEnumerator SetVisited(System.Action VisitAction, Encounter enc = null)
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
        VisitAction();
    }

    public void UpdateIcon()
    {
        List<Sprite> allIcons = DatabaseSystem.instance.iconDatabase.allIcons;
        Sprite icon = allIcons.Where(x => x.name == encounterType.ToString()).FirstOrDefault();
        this.GetComponent<Image>().sprite = icon;
    }
}

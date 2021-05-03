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
    public bool selectable = false;
    public Dictionary<GameObject, List<Encounter>> roads = new Dictionary<GameObject, List<Encounter>>();
    protected delegate void VisitAction();

    public abstract void UpdateEncounter();

    protected virtual bool CheckViablePath(Encounter anEncounter)
    {
        return false;
    }

    public abstract void ButtonPress();

    public IEnumerator SetVisited(System.Action VisitAction, Encounter enc = null)
    {
        selectable = false;

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
                        p.Key.transform.GetChild(counter++).GetComponent<Image>().color = new Color (1.0f, 0.5f, 0.5f);
                        yield return new WaitForSeconds(0.1f);
                    }
                    break;
                }
            }
        }

        WorldSystem.instance.encounterManager.currentEncounter?.SetLeaving(this);

        foreach (Encounter e in neighbourEncounters)
            e.selectable = true;

        Button button = GetComponent<Button>();
        ColorBlock color = button.colors;

        color.normalColor = new Color (1.0f, 0.5f, 0.5f);
        color.disabledColor = new Color (1.0f, 0.5f, 0.5f);
        color.selectedColor = new Color (1.0f, 0.5f, 0.5f);
        button.colors = color;

        WorldSystem.instance.encounterManager.currentEncounter = this;
        Debug.Log("Doing Action");
        VisitAction();
    }

    public void SetLeaving(Encounter nextEnc)
    {
        foreach (Encounter e in neighbourEncounters)
            e.selectable = (e == nextEnc);
    }

    public void UpdateIcon()
    {
        List<Sprite> allIcons = DatabaseSystem.instance.iconDatabase.allIcons;
        Sprite icon = allIcons.Where(x => x.name == encounterType.ToString()).FirstOrDefault();
        GetComponent<Image>().sprite = icon;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class EncounterOverworld: Encounter
{
    public override IEnumerator Entering(System.Action VisitAction, Encounter enc = null)
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
        VisitAction();
    }

    public override void SetLeaving(Encounter nextEnc)
    {
        foreach (Encounter e in neighbourEncounters)
            e.selectable = (e == nextEnc);
    }

}

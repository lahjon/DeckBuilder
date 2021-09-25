using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MenuProgression : MonoBehaviour
{
    public Transform objParent, objCompletedParent, objCurrentParent;
    public Transform missionParent;
    public GameObject progressionItemPrefab;

    void OnEnable()
    {
        UpdateMenu();
    }

    public void UpdateMenu()
    {
        List<Objective> allObjs = WorldSystem.instance.objectiveManager.GetAllObjectives();
        // while (objParent.childCount > 0)
        // {
        //     Destroy(objParent.GetChild(0));
        // }
        for (int i = 0; i < allObjs.Count; i++)
        {
            MenuItemProgression item = Instantiate(progressionItemPrefab, objParent).GetComponent<MenuItemProgression>();
            item.SetItem(allObjs[i]);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Building : MonoBehaviour
{
    public BuildingType buildingType;
    public Canvas canvas;
    public List<GameObject> currentSelection = new List<GameObject>();
    public virtual void CloseBuilding()
    {
        canvas.gameObject.SetActive(false);
        currentSelection.Clear();
        WorldSystem.instance.SaveProgression();
        WorldStateSystem.SetInBuilding(false);
    }
    public virtual void EnterBuilding()
    {
        WorldStateSystem.SetInBuilding(true);
        canvas.gameObject.SetActive(true);
        EventManager.EnterBuilding(this.buildingType);
    }

    protected virtual void StepInto(GameObject room)
    {
        if (currentSelection.Count > 1)
            currentSelection[currentSelection.Count - 1].SetActive(false);

        room.SetActive(true);
        currentSelection.Add(room);
    }

    public void ButtonStepBack()
    {
        StepBack();
    }

    protected virtual void StepBack()
    {
        if (currentSelection.Count > 1)
        {
            currentSelection[currentSelection.Count - 1].SetActive(false);
            currentSelection.RemoveAt(currentSelection.Count - 1);
            currentSelection[currentSelection.Count - 1].SetActive(true);
        }
        else
        {
            CloseBuilding();
        }
    }
}

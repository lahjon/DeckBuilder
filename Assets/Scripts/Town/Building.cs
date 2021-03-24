using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Building : MonoBehaviour
{
    public BuildingType buildingType;
    public Canvas canvas;
    public virtual void CloseBuilding()
    {
        canvas.gameObject.SetActive(false);
        WorldSystem.instance.SaveProgression();
        WorldStateSystem.SetInBuilding(false);
    }
    public virtual void EnterBuilding()
    {
        WorldStateSystem.SetInBuilding(true);
        canvas.gameObject.SetActive(true);
        EventManager.EnterBuilding(this.buildingType);
    }
}

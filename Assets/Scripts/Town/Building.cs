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
        WorldSystem.instance.worldStateManager.RemoveState(true);
    }
    public virtual void EnterBuilding()
    {
        WorldSystem.instance.worldStateManager.AddState(WorldState.Building, true);
        canvas.gameObject.SetActive(true);
        EventManager.EnterBuilding(this.buildingType);
    }
}

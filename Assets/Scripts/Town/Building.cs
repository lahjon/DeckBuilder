using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Building : MonoBehaviour
{
    public virtual void CloseBuilding()
    {
        gameObject.SetActive(false);
        WorldSystem.instance.SaveProgression();
        WorldSystem.instance.worldStateManager.RemoveState(true);
    }
    public virtual void EnterBuilding()
    {
        WorldSystem.instance.worldStateManager.AddState(WorldState.Town, true);
        gameObject.SetActive(true);
    }
}

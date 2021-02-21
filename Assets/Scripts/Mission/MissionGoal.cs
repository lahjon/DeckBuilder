using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public abstract class MissionGoal
{
    public bool completed = false;
    public int currentAmount;
    public int requiredAmount;
    public virtual void Init()
    {

    }

    public virtual void End()
    {
        WorldSystem.instance.missionManager.CheckProgression();
    }
    public void Evaluate()
    {
        if (currentAmount >= requiredAmount)
        {
            Complete();
        }
    }

    public void Complete()
    {
        Debug.Log("Completed mission goal");
        completed = true;
        End();
    }

}
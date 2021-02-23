using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public abstract class MissionGoal
{
    public string goalName;
    public bool completed = false;
    public int currentAmount;
    public int requiredAmount;
    public virtual void Init()
    {

    }

    public virtual void End()
    {
        WorldSystem.instance.missionManager.CheckProgression();
        Debug.Log(string.Format("Goal {0} ended.", goalName));
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
        Debug.Log(string.Format("Goal {0} completed.", goalName));
        completed = true;
        End();
    }

}
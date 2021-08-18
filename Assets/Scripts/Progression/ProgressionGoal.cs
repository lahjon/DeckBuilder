using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public abstract class ProgressionGoal : IEventSubscriber
{
    public string goalName;
    public bool completed = false;
    public int currentAmount;
    public int requiredAmount;
    public Progression progression;
    public abstract void End();
    public void Evaluate()
    {
        Debug.Log("CurrentAmount is: " + currentAmount);
        progression.UpdateGoals(this, currentAmount);

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

    public virtual void Subscribe()
    {
        Debug.Log("Subscribe: " + this);
    }

    public virtual void Unsubscribe()
    {
        Debug.Log("Unsubscribe: " + this);
    }
}
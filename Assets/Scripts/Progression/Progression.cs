using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class Progression : MonoBehaviour
{
    public string progressName;
    public List<ProgressionGoal> goals = new List<ProgressionGoal>();
    public List<int> goalsTrackAmount = new List<int>();
    public bool completed = false;
    public virtual void CheckGoals()
    {
        Debug.Log("checking goals");
        completed = goals.All(g => g.completed);
        WorldSystem.instance.missionManager.missionUI.UpdateUI(false);

        if(completed)
        {
            Complete();
        }
    }

    public void UpdateGoals(ProgressionGoal aGoal, int newAmount)
    {
        goalsTrackAmount[goals.IndexOf(aGoal)] = newAmount;
    }

    protected virtual void AddGoal(ProgressionGoal progressionGoal)
    {
        goals.Add(progressionGoal);
        goalsTrackAmount.Add(0);
    }

    protected abstract void Complete();

    protected virtual void TriggerEvent()
    {
        
    }

}
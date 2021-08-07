using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class Progression : MonoBehaviour
{
    public string id;
    public string aName;
    public List<ProgressionGoal> goals = new List<ProgressionGoal>();
    public List<int> goalsTrackAmount = new List<int>();
    public bool completed = false;
    public virtual void CheckGoals()
    {
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

    public void CreateGoals(ProgressionData data)
    {
        // enter building
        data?.goalEnterBuilding.ForEach(x => AddGoal(new EnterBuildingGoal(this, x.buildingType, x.requiredAmount)));

        // kill enemy
        data?.goalKillEnemy.ForEach(x => AddGoal(new KillEnemyGoal(this, x.enemyId, x.requiredAmount)));

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
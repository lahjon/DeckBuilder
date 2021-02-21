using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Mission : MonoBehaviour
{
    public string missionName;
    public bool completed;
    public string description;
    public string startEvent;
    public string nextMission;
    public List<MissionGoal> goals = new List<MissionGoal>();

    protected virtual void Start()
    {
        WorldSystem.instance.gameEventManager.StartEvent(startEvent);
    }

    public void CheckGoals()
    {
        Debug.Log("checking goals");
        completed = goals.All(g => g.completed);

        if(completed)
        {
            Complete();
        }
    }

    void Complete()
    {
        Debug.Log("Mission Done");
        Debug.Log(nextMission);
        if (nextMission != null && nextMission != "")
        {
            WorldSystem.instance.missionManager.NewMission(nextMission);
            Debug.Log("Starting new mission");
            //DestroyImmediate(this);
        }
        else
        {
            Debug.Log("No new mission to start in this chain");
        }
    }

}
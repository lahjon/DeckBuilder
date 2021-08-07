using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Objective : Progression
{
    [TextArea(5,5)]
    public string objectiveName;
    public string description;
    public string endEvent;
    public void StartObjetive(ObjectiveData data)
    {
        goals.Clear();
        goalsTrackAmount.Clear();

        objectiveName = data.aName;
        id = data.id;
        description = data.description;
        endEvent = data.endEvent;

        CreateGoals(data);

        WorldSystem.instance.uiManager.UIWarningController.CreateWarning("Starting Objective: " + objectiveName);
    }

    protected override void Complete()
    {
        Debug.Log("Progression Done");
        WorldSystem.instance.gameEventManager.StartEvent(endEvent);
        WorldSystem.instance.uiManager.UIWarningController.CreateWarning(description);
        WorldSystem.instance.progressionManager.AddCompleteObjective(this);
        Destroy(this);
    }

}
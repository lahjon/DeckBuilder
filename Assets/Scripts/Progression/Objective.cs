using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Objective : Progression
{
    [TextArea(5,5)]
    public string objectiveName;
    public string description;
    public ObjectiveData data;
    public ObjectiveData nextObjective;
    public string endEvent;
    public void StartObjetive(ObjectiveData aData)
    {
        data = aData;

        nextObjective = data.nextObjective;

        objectiveName = data.aName;
        id = data.id;
        description = data.description;
        endEvent = data.endEvent;

        CreateGoals(data);

        WorldSystem.instance.uiManager.UIWarningController.CreateWarning("Starting Objective: " + objectiveName);
    }

    protected override void Complete()
    {
        base.Complete();
        WorldSystem.instance.objectiveManager.StartObjective(nextObjective);

        WorldSystem.instance.gameEventManager.StartEvent(endEvent);
        WorldSystem.instance.uiManager.UIWarningController.CreateWarning(description);
        WorldSystem.instance.objectiveManager.AddCompleteObjective(this);
        Destroy(gameObject);
    }

}
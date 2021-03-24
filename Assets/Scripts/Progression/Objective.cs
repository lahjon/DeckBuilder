using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Objective : Progression
{
    [TextArea(5,5)]
    public string objectiveId;
    public string description;
    public string completeEvent;
    public AchivementData achivementData; 

    public virtual void Init()
    {
        
    }

    protected override void TriggerEvent()
    {
        WorldSystem.instance.gameEventManager.StartEvent(completeEvent);
    }

    protected override void Complete()
    {
        Debug.Log("progression Done");
        WorldSystem.instance.uiManager.UIWarningController.CreateWarning(description);
        WorldSystem.instance.progressionManager.AddCompleteObjective(this);
        TriggerEvent();
    }

}
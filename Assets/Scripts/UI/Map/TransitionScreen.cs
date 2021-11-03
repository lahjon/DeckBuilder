using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TransitionScreen : MonoBehaviour
{
    public Canvas canvas;
    public TMP_Text worldEncounter;
    public TMP_Text worldEncounterNameText;
    public Animation transition;
    public System.Action midCallback;
    void Awake()
    {
        WorldStateSystem.instance.transitionScreen = this;
        transition = this.GetComponent<Animation>();
    }

    public void EnterActTransitionStart()
    {
        string actName = WorldSystem.instance.worldMapManager.currentWorldScenario != null ? WorldSystem.instance.worldMapManager.currentWorldScenario.worldScenarioData.ScenarioName : "Default Encounter";

        WorldStateSystem.SetInTransition(true);
        worldEncounterNameText.text = string.Format("{0}", actName);
        transition.Play("EnterActTransition");
    }
    public void EnterActTransitionMidSwap()
    {
        // Triggers when enter act transition is done
        if (midCallback != null)
        {
            midCallback.Invoke();
            midCallback = null;
        }
    }
    public void NormalTransitionStart() 
    {
        WorldStateSystem.SetInTransition(true);
        transition.Play("NormalTransition");
    }

    public void EnterTownTransition() 
    {
        WorldStateSystem.SetInTransition(true);
        transition.Play("TownTransition");
    }

    void TriggerCallback()
    {
        // called from animator when transition completes
        WorldStateSystem.TriggerClear();
    }

    void EnterTownCallback()
    {
        // called from animator when transition completes

        WorldStateSystem.TriggerClear();
        WorldSystem.instance.dialogueManager.StartDialogue();
        
    }
}

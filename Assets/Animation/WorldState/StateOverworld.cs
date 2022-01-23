using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateOverworld : WorldStateAnimator
{
    ScenarioCameraController hexMapController;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Init(TransitionType.Normal, WorldState.Overworld);
        if(hexMapController == null) hexMapController = world.scenarioMapManager.hexMapController;
        world.scenarioManager.content.SetActive(true);
        world.hudManager.ToggleScenarioHUD();
        ScenarioManager.ControlsEnabled = true;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        world.scenarioManager.content.SetActive(false);
        ScenarioManager.ControlsEnabled = false;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        hexMapController.HandleMovementInput();
    }

}

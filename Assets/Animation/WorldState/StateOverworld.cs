using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateOverworld : WorldStateAnimator
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Init(TransitionType.Normal, WorldState.Overworld);
        world.encounterManager.OpenOverworldMap();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        world.encounterManager.CloseOverworldMap();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
    }

}

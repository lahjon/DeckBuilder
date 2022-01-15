using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateWorldMap : WorldStateAnimator
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Init(TransitionType.Normal, WorldState.WorldMap);
        //world.worldMapManager.OpenMap();
        //world.hudManager.DisableHUD();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //world.worldMapManager.CloseMap();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
    }

}

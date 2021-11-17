using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateBuilding : WorldStateAnimator
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Init(TransitionType.Normal, StateSwitch());
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
    }

    WorldState StateSwitch()
    {
        Debug.Log(world.townManager.currentBuilding);
        switch (world.townManager.currentBuilding)
        {
            case BuildingType.Scribe:
                return WorldState.BuildingScribe;
            case BuildingType.TownHall:
                return WorldState.BuildingTownHall;
            case BuildingType.Barracks:
                return WorldState.BuildingBarracks;
            case BuildingType.Tavern:
                return WorldState.BuildingTavern;
            case BuildingType.Jeweler:
                return WorldState.BuildingJeweler;
            default:
                return WorldState.Building;
        }
    }

}

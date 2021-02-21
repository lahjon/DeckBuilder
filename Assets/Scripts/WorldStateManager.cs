using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldStateManager : Manager
{
    [SerializeField]
    private List<WorldState> stateStack = new List<WorldState>();

    public WorldState currentState;

    public void RemoveState(bool transition = true)
    {
        if (stateStack.Count > 0)
        {
            stateStack.RemoveAt(stateStack.Count - 1);
            currentState = stateStack[stateStack.Count - 1];
            WorldSystem.instance.characterManager.characterVariablesUI.UpdateUI();
        }
        else
        {
            stateStack.Add(WorldState.Overworld);
            currentState = WorldState.Overworld;
            Debug.Log("This should never happen! Make sure you are using the states correctly!");
        }
        WorldSystem.instance.worldState = currentState;
        StateAction(currentState);
        if (transition && currentState != WorldState.Transition)
        {
            WorldSystem.instance.uiManager.screenTransition.ToggleActive();
        }
    }

    public void AddState(WorldState aState, bool transition = true)
    {
        stateStack.Add(aState);
        currentState = aState;
        WorldSystem.instance.characterManager.characterVariablesUI.UpdateUI();
        WorldSystem.instance.worldState = currentState;
        StateAction(currentState);
        if (transition && currentState != WorldState.Transition)
        {
            WorldSystem.instance.uiManager.screenTransition.ToggleActive();
        }
    }

    private void StateAction(WorldState aState)
    {
        switch (aState)
        {
            case WorldState.Overworld:
                WorldSystem.instance.townManager.ExitTown();
                WorldSystem.instance.encounterManager.OpenOverworldMap();
                break;
            
            case WorldState.Transition:
                break;

            case WorldState.Town:
                WorldSystem.instance.townManager.UpdateTown();
                break;

            default:
                WorldSystem.instance.encounterManager.CloseOverworldMap();
                break;
        }
    }
}
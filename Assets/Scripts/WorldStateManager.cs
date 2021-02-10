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
        if (aState == WorldState.Overworld)
        {
            WorldSystem.instance.encounterManager.OpenOverworldMap();
        }
        else if (aState == WorldState.Transition)
        {
            return;
        }
        else
        {
            WorldSystem.instance.encounterManager.CloseOverworldMap();
        }
    }
}

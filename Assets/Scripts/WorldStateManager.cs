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
            world.characterManager.characterVariablesUI.UpdateUI();
            world.cameraManager.CameraGoto(currentState, transition); 
        }
        else
        {
            stateStack.Add(WorldState.Overworld);
            currentState = WorldState.Overworld;
            Debug.Log("This should never happen! Make sure you are using the states correctly!");
        }
        world.worldState = currentState;
        StateAction(currentState);
    }

    public void AddState(WorldState aState, bool transition = true)
    {
        stateStack.Add(aState);
        currentState = aState;
        world.characterManager.characterVariablesUI.UpdateUI();
        world.cameraManager.CameraGoto(aState, transition);
        world.worldState = currentState;
        StateAction(currentState);
    }

    private void StateAction(WorldState aState)
    {
        if (aState == WorldState.Overworld)
        {
            world.encounterManager.OpenOverworldMap();
        }
        else
        {
            world.encounterManager.CloseOverworldMap();
        }
    }
}

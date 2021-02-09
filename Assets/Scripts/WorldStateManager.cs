using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldStateManager : Manager
{
    private List<WorldState> worldsStates = new List<WorldState>();

    public WorldState currentState;

    public void RemoveState(bool transition = true)
    {
        worldsStates.RemoveAt(worldsStates.Count - 1);
        currentState = worldsStates[worldsStates.Count - 1];
        world.characterManager.characterVariablesUI.UpdateUI();
        world.cameraManager.CameraGoto(currentState, transition);
    }

    public void AddState(WorldState aState, bool transition = true)
    {
        worldsStates.Add(aState);
        currentState = aState;
        world.characterManager.characterVariablesUI.UpdateUI();
        world.cameraManager.CameraGoto(aState, transition);
    }
}

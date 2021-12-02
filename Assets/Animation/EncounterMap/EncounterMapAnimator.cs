using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterMapAnimator : StateMachineBehaviour
{
    protected static ScenarioMapManager gridManager;
    protected HexTile tile;
    protected virtual void Init()
    {
        if (gridManager == null)
        {
            gridManager = WorldSystem.instance.scenarioMapManager;
        }
        tile = gridManager.currentTile;
    }
}

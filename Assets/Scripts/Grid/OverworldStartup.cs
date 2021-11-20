using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;


public class OverworldStartup : MonoBehaviour
{
    void Start()
    {
        // if (WorldSystem.instance.missionManager != null && WorldSystem.instance.missionManager.mission == null)
        // {
        //     WorldSystem.instance.missionManager.NewMission("Mission001", false);
        // }

        // DEBUG! fix
        if (WorldSystem.instance.characterManager.characterCurrency.gold == 0)
        {
            WorldSystem.instance.characterManager.characterCurrency.gold = 50;
        }

        if (WorldSystem.instance.characterManager.characterCurrency.shard == 0)
        {
            WorldSystem.instance.characterManager.characterCurrency.shard = 5;
        }
        if (WorldSystem.instance.characterManager.characterCurrency.fullEmber == 0)
        {
            WorldSystem.instance.characterManager.characterCurrency.fullEmber = 2;
        }
        //WorldSystem.instance.worldMapManager.UpdateMap();
        WorldSystem.instance.missionManager.StartMission(0);

        WorldSystem.instance.worldMapManager.UnlockScenario(0);
    }
}

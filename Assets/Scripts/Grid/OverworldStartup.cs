using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class OverworldStartup : MonoBehaviour
{
    void Start()
    {
        // if (WorldSystem.instance.missionManager != null && WorldSystem.instance.missionManager.mission == null)
        // {
        //     WorldSystem.instance.missionManager.NewMission("Mission001", false);
        // }

        // DEBUG! fix
        if (WorldSystem.instance.characterManager.gold == 0)
        {
            WorldSystem.instance.characterManager.gold = 50;
        }

        if (WorldSystem.instance.characterManager.shard == 0)
        {
            WorldSystem.instance.characterManager.shard = 5;
        }
        WorldSystem.instance.worldMapManager.UpdateMap();
    }
}

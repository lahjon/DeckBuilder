using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class DebugUI : MonoBehaviour
{
    public GameObject canvas;
    public int strength;
    public GameObject tokenReward;
    public GameObject artifactReward;
    public TMP_Text worldState;
    public TMP_Text overlayState;
    public TMP_Text worldTier;
    public List<EnemyData> enemyData = new List<EnemyData>();
    WorldSystem world;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.D))
        {
            ToggleDebugMenu();
        }
    }

    void Start()
    {
        world = WorldSystem.instance;
    }

    public void UpdateCharacterDebugHUD()
    {
        worldState.text = WorldStateSystem.instance.currentWorldState.ToString();
        overlayState.text = WorldStateSystem.instance.currentOverlayState.ToString();
        worldTier.text = "Act " + world.act.ToString();
    }

    public void DebugCreateWarning()
    {
        world.uiManager.UIWarningController.CreateWarning("This is a debug warning!");
    }
    public void DebugTakeDamage(int amount)
    {
        world.characterManager.TakeDamage(amount);
    }
    public void DebugHeal(int amount)
    {
        world.characterManager.Heal(amount);
    }

    public void DebugDraftCards(int amount)
    {
        world.rewardManager.draftAmount = amount;
        WorldStateSystem.SetInReward(true);
    }

    public void DebugAddStat(int amount)
    {
        world.characterManager.characterStats.ModifyHealth(amount);
    }

    public void DebugRemoveStat(int amount)
    {
        world.characterManager.characterStats.ModifyHealth(-amount);
    }

    public void DebugAddSpecficArtifact()
    {
        world.artifactManager.AddArifact(artifactReward);
    }
    public void DebugAddExperience(int amount)
    {
        world.levelManager.AddExperience(amount);
    }
    public void DebugAddLevel()
    {
        world.levelManager.AddLevel();
    }

    public void DebugStartCombat()
    {
        world.combatManager.combatController.enemyDatas = enemyData;
        WorldStateSystem.SetInOverworld(true);
        WorldStateSystem.SetInTown(false);
        WorldStateSystem.SetInCombat(true);
    }
    public void DebugRemoveSpecificArtifact()
    {
        world.artifactManager.RemoveArtifact(artifactReward);
    }
    public void DebugAddRandomArtifact()
    {
        world.artifactManager.AddArifact(world.artifactManager.GetRandomAvailableArtifact());
    }

    public void DebugRemoveRandomArtifact()
    {
        world.artifactManager.RemoveArtifact(world.artifactManager.GetRandomActiveArtifact());
    }
    public void DebugAddTokenPoints()
    {
        world.tokenManager.AddTokenPoint();
    }
    public void DebugAddTokenReward()
    {
        world.tokenManager.UnlockNewToken(tokenReward);
    }

    public void DebugGetShards()
    {
        world.characterManager.shard += 5;
    }

    public void DebugResetAllData()
    {
        Directory.GetFiles(Application.persistentDataPath);
        DirectoryInfo dirInfo = new DirectoryInfo(Application.persistentDataPath);
        foreach (FileInfo file in dirInfo.GetFiles())
        {
            Debug.Log(file);
            file.Delete(); 
        }
    }

    public void DebugTriggerDeathScreen()
    {
        WorldStateSystem.SetInDeathScreen(true);
    }

    public void DebugGenerateWorld()
    {
        world.encounterManager.GenerateMap();
    }

    public void DebugWinCombat()
    {
        if(WorldStateSystem.instance.currentWorldState == WorldState.Combat)
            world.combatManager.combatController.WinCombat();
    }
    public void ToggleDebugMenu()
    {
        if(canvas.activeSelf)
        {
            canvas.SetActive(false);
        }
        else
        {
            canvas.SetActive(true);
        }
    }

    public void DebugLoadGame()
    {
        world.LoadProgression();
    }
    public void DebugSaveGmae()
    {
        world.SaveProgression();
    }

}

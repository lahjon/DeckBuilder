using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugUI : MonoBehaviour, ISaveableWorld
{
    public GameObject canvas;
    public int strength;
    public GameObject tokenReward;
    public GameObject artifactReward;
    public TMP_Text worldState;
    public TMP_Text overlayState;
    public TMP_Text worldTier;
    public List<EnemyData> enemyData = new List<EnemyData>();

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.D))
        {
            ToggleDebugMenu();
        }
    }

    public void UpdateCharacterDebugHUD()
    {
        worldState.text = WorldStateSystem.instance.currentWorldState.ToString();
        overlayState.text = WorldStateSystem.instance.currentOverlayState.ToString();
        worldTier.text = "Act " + WorldSystem.instance.act.ToString();
    }

    public void DebugCreateWarning()
    {
        WorldSystem.instance.uiManager.UIWarningController.CreateWarning("This is a debug warning!");
    }

    public void DebugDraftCards(int amount)
    {
        WorldSystem.instance.rewardManager.draftAmount = amount;
        WorldStateSystem.SetInReward(true);
    }

    public void DebugAddSpecficArtifact()
    {
        WorldSystem.instance.artifactManager.AddArifact(artifactReward);
    }
    public void DebugAddExperience(int amount)
    {
        WorldSystem.instance.levelManager.AddExperience(amount);
    }
    public void DebugAddLevel()
    {
        WorldSystem.instance.levelManager.AddLevel();
    }

    public void DebugStartCombat()
    {
        WorldSystem.instance.combatManager.combatController.enemyDatas = enemyData;
        WorldStateSystem.SetInOverworld(true);
        WorldStateSystem.SetInTown(false);
        WorldStateSystem.SetInCombat(true);
    }
    public void DebugRemoveSpecificArtifact()
    {
        WorldSystem.instance.artifactManager.RemoveArtifact(artifactReward);
    }
    public void DebugAddRandomArtifact()
    {
        WorldSystem.instance.artifactManager.AddArifact(WorldSystem.instance.artifactManager.GetRandomAvailableArtifact());
    }

    public void DebugRemoveRandomArtifact()
    {
        WorldSystem.instance.artifactManager.RemoveArtifact(WorldSystem.instance.artifactManager.GetRandomActiveArtifact());
    }
    public void DebugAddTokenPoints()
    {
        WorldSystem.instance.tokenManager.AddTokenPoint();
    }
    public void DebugAddTokenReward()
    {
        WorldSystem.instance.tokenManager.UnlockNewToken(tokenReward);
    }

    public void DebugGetShards()
    {
        WorldSystem.instance.characterManager.shard += 5;
    }

    public void DebugResetAllData()
    {
        FileManager.WriteToFile(SaveDataManager.saveFileName, "");
    }

    public void DebugTriggerDeathScreen()
    {
        WorldSystem.instance.uiManager.deathScreen.TriggerDeathscreen();
    }

    public void DebugGenerateWorld()
    {
        WorldSystem.instance.encounterManager.GenerateMap();
    }

    public void DebugWinCombat()
    {
        if(WorldStateSystem.instance.currentWorldState == WorldState.Combat)
            WorldSystem.instance.combatManager.combatController.WinCombat();
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
    public void PopulateSaveDataWorld(SaveDataWorld a_SaveData)
    {
        a_SaveData.strength = strength;
    }

    public void LoadFromSaveDataWorld(SaveDataWorld a_SaveData)
    {
        strength = a_SaveData.strength;
    }

    public void DebugPermanentMaxHealth()
    {
        strength++;
        Debug.Log(strength);
    }

    public void DebugLoadGame()
    {
        SaveDataManager.LoadJsonData((Helpers.FindInterfacesOfType<ISaveableWorld>()));
    }
    public void DebugSaveGmae()
    {
        SaveDataManager.SaveJsonData((Helpers.FindInterfacesOfType<ISaveableWorld>()));
    }

}

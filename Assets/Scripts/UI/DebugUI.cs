using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugUI : MonoBehaviour, ISaveable
{
    public GameObject canvas;
    public int strength;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.D))
        {
            ToggleDebugMenu();
        }
    }

    public void DebugCreateWarning()
    {
        WorldSystem.instance.uiManager.UIWarningController.CreateWarning("This is a debug warning!");
    }

    public void DebugTriggerDeathScreen()
    {
        WorldSystem.instance.uiManager.deathScreen.TriggerDeathscreen();
    }

    public void DebugAddStrength()
    {
        WorldSystem.instance.characterManager.AddStat(CharacterStat.Strength, 1);
        WorldSystem.instance.uiManager.characterSheet.UpdateCharacterSheet();
    }
    public void DebugWinCombat()
    {
        if(WorldSystem.instance.worldState == WorldState.Combat)
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
    public void PopulateSaveData(SaveData a_SaveData)
    {
        a_SaveData.strength = strength;
    }

    public void LoadFromSaveData(SaveData a_SaveData)
    {
        strength = a_SaveData.strength;
        Debug.Log(strength);
    }

    public void DebugPermanentMaxHealth()
    {
        strength++;
        Debug.Log(strength);
    }

    public void DebugLoadGame()
    {
        SaveDataManager.LoadJsonData((Helpers.FindInterfacesOfType<ISaveable>()));
        Debug.Log(strength);
    }
    public void DebugSaveGmae()
    {
        SaveDataManager.SaveJsonData((Helpers.FindInterfacesOfType<ISaveable>()));
        Debug.Log(strength);
    }

}

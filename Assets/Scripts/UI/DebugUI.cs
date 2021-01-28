using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugUI : MonoBehaviour
{
    public GameObject canvas;

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

}

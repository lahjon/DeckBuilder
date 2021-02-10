using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownManager : Manager
{
    public List<Encounter> townEncounters;
    public Canvas canvas;
    void Start()
    {
        canvas.gameObject.SetActive(true);
        townEncounters.ForEach(x => x.UpdateEncounter());
    }
    public void EnterTavern()
    {

    }

    public void EnterPray()
    {
        
    }

    public void LeaveTown()
    {

        canvas.gameObject.SetActive(false);
        WorldSystem.instance.encounterManager.canvas.gameObject.SetActive(true);
        WorldSystem.instance.encounterManager.GenerateMap();
        WorldSystem.instance.encounterManager.canvas.gameObject.SetActive(false);
        WorldSystem.instance.characterManager.characterVariablesUI.UpdateUI();
        WorldSystem.instance.encounterManager.currentEncounter = WorldSystem.instance.encounterManager.overworldEncounters[0];
        WorldSystem.instance.worldStateManager.AddState(WorldState.Overworld, true);
    }

    public void EnterShop()
    {
        
    }
}

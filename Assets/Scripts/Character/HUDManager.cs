using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : Manager
{
    public GameObject townHUD, scenarioHUD, combatHUD, abilityHUD, debugHUD;
    void ToggleBar(WorldState worldState)
    {
        if (worldState == WorldState.Overworld)
            ToggleScenarioHUD();
        else if (worldState == WorldState.Town)
            ToggleTownHUD();
        else if (worldState == WorldState.Combat)
            ToggleCombatHUD();
    }

    protected override void Awake()
    {
        base.Awake();
        world.hudManager = this;
    }

    protected override void Start()
    {
        base.Start(); 
        if (world.debugMode) debugHUD.SetActive(true);
        EventManager.OnNewWorldStateEvent += ToggleBar;
    }

    void ToggleTownHUD()
    {
        townHUD.SetActive(true);
        scenarioHUD.SetActive(false);
        abilityHUD.SetActive(false);
        combatHUD.SetActive(false);
    }
    void ToggleScenarioHUD()
    {
        scenarioHUD.SetActive(true);
        abilityHUD.SetActive(true);
        townHUD.SetActive(false);
        combatHUD.SetActive(false);
    }
    void ToggleCombatHUD()
    {
        combatHUD.SetActive(true);
        abilityHUD.SetActive(true);
        townHUD.SetActive(false);
        scenarioHUD.SetActive(false);
    }
    public void ButtonDisplayDeck()
    {
        if (WorldStateSystem.instance.currentOverlayState != OverlayState.Display)
        {
            WorldSystem.instance.deckDisplayManager.OpenDisplay();
        }
        else
        {
            WorldSystem.instance.deckDisplayManager.CloseDeckDisplay();
        }
    }
}

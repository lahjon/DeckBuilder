using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : Manager
{
    public GameObject townHUD, scenarioHUD, combatHUD, abilityHUD, debugHUD;

    protected override void Awake()
    {
        base.Awake();
        world.hudManager = this;
    }

    protected override void Start()
    {
        base.Start(); 
        if (world.debugMode) debugHUD.SetActive(true);
    }

    public void ToggleTownHUD()
    {
        townHUD.SetActive(true);
        scenarioHUD.SetActive(false);
        abilityHUD.SetActive(false);
        combatHUD.SetActive(false);
    }
    public void ToggleScenarioHUD()
    {
        scenarioHUD.SetActive(true);
        abilityHUD.SetActive(true);
        townHUD.SetActive(false);
        combatHUD.SetActive(false);
    }
    public void ToggleCombatHUD()
    {
        combatHUD.SetActive(true);
        abilityHUD.SetActive(true);
        townHUD.SetActive(false);
        scenarioHUD.SetActive(false);
    }

    public void DisableHUD()
    {
        combatHUD.SetActive(false);
        abilityHUD.SetActive(false);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Manager
{
    public int planeDistance = 10;
    public GameObject ui;
    public ScreenTransition screenTransition;
    public RewardScreen rewardScreen;
    public UIWarningController UIWarningController;
    public CharacterSheet characterSheet;
    public DeathScreen deathScreen;
    public EncounterUI encounterUI;
    protected override void Awake()
    {
        base.Awake(); 
        world.uiManager = this;
    }

}

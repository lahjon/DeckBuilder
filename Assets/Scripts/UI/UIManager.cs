using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Manager
{
    public int planeDistance = 10;
    public UIWarningController UIWarningController;
    public DeathScreen deathScreen;
    public EncounterUI encounterUI;
    public DebugUI debugUI;
    public EscapeMenu escapeMenu;
    
    protected override void Awake()
    {
        base.Awake(); 
        world.uiManager = this;
    }

}

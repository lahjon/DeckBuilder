using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioSelectionManager : Manager
{
    public ScenarioSelectWindow windowA, windowB, windowC;
    public GameObject canvas;
    ScenarioSelectWindow currentSelection;
    protected override void Start()
    {
        base.Start();
        world.scenarioSelectionManager = this;
    }

    protected override void Awake()
    {
        base.Awake();
    }

    public void Open()
    {
        canvas.SetActive(true);
    }
    
    public void ButtonClose()
    {
        canvas.SetActive(false);
    }
}

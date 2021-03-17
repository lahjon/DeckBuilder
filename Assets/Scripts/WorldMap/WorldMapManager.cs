using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapManager : Manager
{
    public Canvas canvas;
    public GameObject content;
    public TransitionScreen newActTransition;
    public int actIndex;
    public List<string> actNames = new List<string>();
    protected override void Awake()
    {
        base.Awake();
        world.worldMapManager = this;

        actNames.Add("Town of Nielgard");
        actNames.Add("Barren Woods");
        actNames.Add("Act2");
        actNames.Add("Act3");
        actNames.Add("Act4");

    }
    public void OpenMap()
    {
        UpdateMap();
        content.SetActive(true);
    }

    public void CloseMap()
    {
        content.SetActive(false);
    }
    void UpdateMap()
    {
        // if new act is unlocked, update all the maps
    }
}

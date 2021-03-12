using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapManager : Manager
{
    public Canvas canvas;
    public GameObject content;
    public NewActTransition newActTransition;
    protected override void Awake()
    {
        base.Awake();
        world.worldMapManager = this;
    }
    public void OpenMap()
    {
        UpdateMap();
        content.SetActive(true);
    }
    void UpdateMap()
    {

    }
}

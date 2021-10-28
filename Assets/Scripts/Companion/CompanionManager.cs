using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionManager : Manager
{
    public CompanionData currentCompanionData;
    public List<int> unlockedCompanions;
    protected override void Awake()
    {
        base.Awake();
        world.companionManager = this;
    }

    protected override void Start()
    {
        base.Start();
    }
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BuildingTavern : Building
{
    public GameObject roomMain;
    public override void CloseBuilding()
    {
        base.CloseBuilding();
    }
    
    public override void EnterBuilding()
    {
        base.EnterBuilding();
        StepInto(roomMain);
    }
}
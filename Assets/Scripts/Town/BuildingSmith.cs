using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BuildingSmith : Building
{
    public GameObject roomMain, roomToken, roomEquipment;
    public void ButtonEnterToken()
    {
        StepInto(roomToken);
    }
    public void ButtonEnterEquipment()
    {
        StepInto(roomEquipment);
    }
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BuildingScribe : Building
{
    public GameObject scribe, deckManagement, cardUpgrade; // rooms
    public override void CloseBuilding()
    {
        base.CloseBuilding();
    }
    public override void EnterBuilding()
    {
        base.EnterBuilding();
        StepInto(scribe);
    }
    public void ButtonEnterDeckManagement()
    {
        StepInto(deckManagement);
    }
    public void ButtonEnterCardUpgrade()
    {
        StepInto(cardUpgrade);
    }
}

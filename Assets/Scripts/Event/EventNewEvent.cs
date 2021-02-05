using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventNewEvent : EventMain
{
    public override bool TriggerEvent()
    {
        EncounterUITest ui = WorldSystem.instance.uiManager.encounterUITest;
        GameObject oldUI = Instantiate(ui.background, ui.background.transform.position, Quaternion.Euler(0, 0, 0), ui.panel.transform) as GameObject;
        ui.encounterData = ui.newEncounterData;
        ui.BindEncounterData();
        ui.background.GetComponent<CanvasGroup>().alpha = 0.0f;

        ui.StartFade(oldUI, ui.background);
        return false;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioSelectWindow : MonoBehaviour
{
    public void ButtonSelectScenario()
    {
        WorldStateSystem.instance.overrideTransitionType = TransitionType.EnterMap;
        WorldStateSystem.SetInOverworld();
        Helpers.DelayForSeconds(1f, () => WorldSystem.instance.scenarioManager.GenerateMap());
        WorldSystem.instance.scenarioSelectionManager.ButtonClose();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugUI : MonoBehaviour
{
    public void DebugCreateWarning()
    {
        WorldSystem.instance.uiManager.UIWarningController.CreateWarning("This is a debug warning!");
    }
}

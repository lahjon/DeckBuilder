using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterIcon : MonoBehaviour, IToolTipable
{
    [TextArea(5, 5)]
    public string description;
    static float width = 50;
    public virtual (List<string> tips, Vector3 worldPosition, float offset) GetTipInfo()
    {
        Vector3 pos = WorldSystem.instance.cameraManager.currentCamera.WorldToScreenPoint(transform.position);
        return (new List<string>{description} , pos, width);
    }
}

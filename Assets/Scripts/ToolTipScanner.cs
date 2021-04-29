using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ToolTipScanner : MonoBehaviour
{ 
    private static LTDescr delayAction;
    public float timeDelay = 0.5f;

    /*
    /* Verkar inte behövas fattar fan noll???
    public void OnPointerEnter(PointerEventData eventData)
    {
        delayAction = LeanTween.delayedCall(timeDelay, () =>
        {
            Debug.Log("TooltipScanner sending");
            (List<string> tips, Vector3 worldPos) tipInfo = GetComponent<IToolTipable>().GetTipInfo();
            WorldSystem.instance.toolTipManager.Tips(tipInfo.tips, WorldSystem.instance.cameraManager.mainCamera.WorldToScreenPoint(tipInfo.worldPos));
        }
        );
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LeanTween.cancel(delayAction.uniqueId);
        WorldSystem.instance.toolTipManager.DisableTips();
    }
    */

    public void OnMouseEnter()
    {
        delayAction = LeanTween.delayedCall(timeDelay, () =>
        {
            Debug.Log("TooltipScanner sending");
            (List<string> tips, Vector3 worldPos) tipInfo = GetComponent<IToolTipable>().GetTipInfo();
            WorldSystem.instance.toolTipManager.Tips(tipInfo.tips, WorldSystem.instance.cameraManager.mainCamera.WorldToScreenPoint(tipInfo.worldPos), this);
        }
        );
    }

    public void OnMouseExit()
    {
        LeanTween.cancel(delayAction.uniqueId);
        WorldSystem.instance.toolTipManager.DisableTips(this);
    }

    /* Behövs ej längre? On destroy verkar trigga att mouse exit
    private void OnDestroy()
    {
        LeanTween.cancel(delayAction.uniqueId);
        WorldSystem.instance.toolTipManager.DisableTips(this);
    }
    */
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ToolTipScanner : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{ 
    private static LTDescr delayAction;
    public float timeDelay = 0.5f;
    public Tween delayTween;
    private float dummy; 

    Coroutine _delayAction;

    // BRYT UT TILL COLLISION VS UI

    public void EnterAction()
    {
        
        (delayTween = DOTween.To(() => 0, x => { }, 0, timeDelay)).OnComplete(() => {
            (List<string> tips, Vector3 worldPos) tipInfo = GetComponent<IToolTipable>().GetTipInfo();
            WorldSystem.instance.toolTipManager.Tips(tipInfo.tips, tipInfo.worldPos, this);
        }); 
    }

    public void ExitAction()
    {
        delayTween.Kill();
        WorldSystem.instance.toolTipManager.DisableTips(this);
        
    }
    public void OnMouseEnter()
    {
        EnterAction();
    }

    public void OnMouseExit()
    {
        ExitAction();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        EnterAction();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        ExitAction();
    }

    IEnumerator DelayAction()
    {
        yield return new WaitForSeconds(timeDelay);
        (List<string> tips, Vector3 worldPos) tipInfo = GetComponent<IToolTipable>().GetTipInfo();
        WorldSystem.instance.toolTipManager.Tips(tipInfo.tips, WorldSystem.instance.cameraManager.currentCamera.WorldToScreenPoint(tipInfo.worldPos), this);
    }
}

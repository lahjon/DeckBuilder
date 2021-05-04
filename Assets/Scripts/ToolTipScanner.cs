using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ToolTipScanner : MonoBehaviour
{ 
    private static LTDescr delayAction;
    public float timeDelay = 0.5f;
    private Tween _delayTween;
    private float dummy; 

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

    Coroutine _delayAction;

    public void OnMouseEnter()
    {
        /*
        delayAction = LeanTween.delayedCall(timeDelay, () => {
            (List<string> tips, Vector3 worldPos) tipInfo = GetComponent<IToolTipable>().GetTipInfo();
            WorldSystem.instance.toolTipManager.Tips(tipInfo.tips, WorldSystem.instance.cameraManager.mainCamera.WorldToScreenPoint(tipInfo.worldPos), this);
        }
        );
        */
        //_delayAction = StartCoroutine(DelayAction());

        (_delayTween = DOTween.To(() => 0, x => { }, 0, timeDelay)).OnComplete(() => {
            (List<string> tips, Vector3 worldPos) tipInfo = GetComponent<IToolTipable>().GetTipInfo();
            WorldSystem.instance.toolTipManager.Tips(tipInfo.tips, WorldSystem.instance.cameraManager.mainCamera.WorldToScreenPoint(tipInfo.worldPos), this);
        }); 
    }

    public void OnMouseExit()
    {
        //StopCoroutine(_delayAction);
        //LeanTween.cancel(delayAction.uniqueId);
        _delayTween.Kill();
        WorldSystem.instance.toolTipManager.DisableTips(this);

    }

    /* Behövs ej längre? On destroy verkar trigga att mouse exit
    private void OnDestroy()
    {
        LeanTween.cancel(delayAction.uniqueId);
        WorldSystem.instance.toolTipManager.DisableTips(this);
    }
    */

    IEnumerator DelayAction()
    {
        yield return new WaitForSeconds(timeDelay);
        (List<string> tips, Vector3 worldPos) tipInfo = GetComponent<IToolTipable>().GetTipInfo();
        WorldSystem.instance.toolTipManager.Tips(tipInfo.tips, WorldSystem.instance.cameraManager.mainCamera.WorldToScreenPoint(tipInfo.worldPos), this);
    }
}

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
    public float timeDelay = 0.5f;
    public Tween delayTween;

    public void EnterAction()
    {
        WorldSystem.instance.toolTipManager.currentScanner = this;
        (delayTween = DOTween.To(() => 0, x => { }, 0, timeDelay)).OnComplete(() => {
            (List<string> tips, Vector3 worldPos, float offset) tipInfo = GetComponent<IToolTipable>().GetTipInfo();
            WorldSystem.instance.toolTipManager.Tips(tipInfo.tips, tipInfo.worldPos, tipInfo.offset, this);
        }); 
    }

    public void ExitAction()
    {
        delayTween.Kill(false);
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

}

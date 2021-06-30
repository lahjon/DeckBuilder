// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using DG.Tweening;

// public class IntentHover : MonoBehaviour, IToolTipable
// {
//     [HideInInspector] public string tooltipDescription;
//     public (List<string> tips, Vector3 worldPosition) GetTipInfo()
//     {
//         Vector3 pos = WorldSystem.instance.cameraManager.currentCamera.WorldToScreenPoint(transform.position);
//         return (new List<string>() { tooltipDescription}, pos);
//     }

//     public void OnMouseEnter()
//     {
//         Debug.Log("Dick?");
//     }
// }

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using TMPro;
using UnityEngine.UI;


public class ToolTipManager : Manager
{
    Canvas canvas;

    public GameObject templateTooltip;
    public RectTransform TipLocation; 
    public List<TMP_Text> txt_tips = new List<TMP_Text>();

    VerticalLayoutGroup vlg;

    protected override void Awake()
    {
        base.Awake();
        canvas = GetComponent<Canvas>();
        vlg = GetComponentInChildren<VerticalLayoutGroup>();
        world.toolTipManager = this;
    }

    protected override void Start()
    {
        canvas.worldCamera = WorldSystem.instance.cameraManager.mainCamera;
        canvas.planeDistance = WorldSystem.instance.uiManager.planeDistance;
    }


    public void Tips(List<string> tips, Vector3 screenPoint)
    {
        TipLocation.position = screenPoint;
        //TipLocation.position = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, screenPoint);
        for (int i = 0; i < tips.Count; i++) {
            if (i == txt_tips.Count)
            {
                GameObject go = Instantiate (templateTooltip,TipLocation);
                TMP_Text text = go.GetComponentInChildren<TMP_Text>();
                txt_tips.Add(text);
            }
            vlg.enabled = false;
            vlg.enabled = true;
            txt_tips[i].transform.parent.gameObject.SetActive(true);
            txt_tips[i].text = tips[i];
            txt_tips[i].transform.parent.GetComponentInChildren<VerticalLayoutGroup>().enabled = false;
            txt_tips[i].transform.parent.GetComponentInChildren<VerticalLayoutGroup>().enabled = true;
        }

        vlg.enabled = false;
        vlg.enabled = true;
        Canvas.ForceUpdateCanvases();
        TipLocation.position = screenPoint;
    }


    public void DisableTips()
    {
        foreach (TMP_Text text in txt_tips)
            text.transform.parent.gameObject.SetActive(false);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using TMPro;
using UnityEngine.UI;


public class ToolTipManager : Manager
{
    public Canvas canvas;

    public GameObject templateTooltip;
    public RectTransform TipLocation; 
    public List<TMP_Text> txt_tips = new List<TMP_Text>();

    public ToolTipScanner currentScanner = null;

    private bool _canShow = true;
    public bool canShow { get { return _canShow; } 
        set {
            if (value == false)
                DisableTips(currentScanner);
            //Debug.Log("tooltip Can show  set to:" + value);
            _canShow = value;
        } }

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

    public void Tips(List<string> tips, Vector3 screenPoint, ToolTipScanner scanner)
    {
        if (!canShow || tips.Count == 0) return;
        currentScanner = scanner;

        TipLocation.anchoredPosition = screenPoint;
        for (int i = 0; i < tips.Count; i++) {
            if (i == txt_tips.Count)
            {
                GameObject go = Instantiate (templateTooltip,TipLocation);
                txt_tips.Add(go.GetComponentInChildren<TMP_Text>());
            }
            //vlg.enabled = false;
            //vlg.enabled = true;
            txt_tips[i].transform.parent.gameObject.SetActive(true);
            txt_tips[i].text = tips[i];
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(TipLocation);
    }

    public void DisableTips(ToolTipScanner scanner)
    {
        if (currentScanner is null || scanner == currentScanner)
            foreach (TMP_Text text in txt_tips)
                text.transform.parent.gameObject.SetActive(false);
            currentScanner = null;
    }

    public void DisableTips()
    {
        currentScanner?.ExitAction();
        // foreach (TMP_Text text in txt_tips)
        //     text.transform.parent.gameObject.SetActive(false);
    }

}

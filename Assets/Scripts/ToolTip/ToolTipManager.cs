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
    [SerializeField] float toolTipWidth;
    float screenWidth;
    float screenHeight;
    float margin = 10;
    public List<TMP_Text> txt_tips = new List<TMP_Text>();

    public ToolTipScanner currentScanner = null;

    private bool _canShow = true;
    public bool canShow { get { return _canShow; } 
        set {
            if (value == false)
                DisableTips(currentScanner);
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
        screenWidth = (float)Screen.currentResolution.width;
        screenHeight = (float)Screen.currentResolution.height;
    }

    public void Tips(List<string> tips, Vector3 screenPoint, float xOffset, ToolTipScanner scanner)
    {
        if (!canShow || tips.Count == 0) return;
        currentScanner = scanner;
        for (int i = 0; i < tips.Count; i++) {
            if (i == txt_tips.Count)
            {
                GameObject go = Instantiate(templateTooltip,TipLocation);
                txt_tips.Add(go.GetComponentInChildren<TMP_Text>());
            }
            txt_tips[i].transform.parent.gameObject.SetActive(true);
            txt_tips[i].text = tips[i];
        }
        Vector3 tooltipPosition;
        TextAnchor textAnchor;
        LayoutRebuilder.ForceRebuildLayoutImmediate(TipLocation);
        (tooltipPosition, textAnchor) = ValidatePosition(screenPoint, xOffset);
        vlg.childAlignment = textAnchor;
        TipLocation.anchoredPosition = tooltipPosition;
    }

    public void DisableTips(ToolTipScanner scanner)
    {
        if (currentScanner is null || scanner == currentScanner)
            foreach (TMP_Text text in txt_tips)
                text.transform.parent.gameObject.SetActive(false);
            currentScanner = null;
    }

    public (Vector3 pos, TextAnchor textAnchor) ValidatePosition(Vector3 aPos, float offset)
    {
        float xOffset = offset;
        float yOffset = 0;
        TextAnchor anAnchor = TextAnchor.UpperLeft;
        if (toolTipWidth + offset + aPos.x > screenWidth - margin)
        {
            anAnchor = TextAnchor.UpperRight;
            xOffset *= -1;
        }

        float height = 0;
        for (int i = 0; i < TipLocation.transform.childCount; i++)
        {
            if (TipLocation.transform.GetChild(i).gameObject.activeSelf)
                height += TipLocation.transform.GetChild(i).GetComponent<RectTransform>().sizeDelta.y;
        }
        if (aPos.y - height < 0 + margin)
            yOffset = height + margin;

        return (new Vector3(aPos.x + xOffset, aPos.y + yOffset, aPos.z), anAnchor);
    }

    public void DisableTips()
    {
        currentScanner?.ExitAction();
        // foreach (TMP_Text text in txt_tips)
        //     text.transform.parent.gameObject.SetActive(false);
    }

}

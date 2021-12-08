using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class Artifact : Item, IToolTipable
{
    public int id;
    protected Image image;
    public Button button;
    public Transform tooltipAnchor;
    bool initialized;
    public ArtifactData artifactData;
    Tween tween;
    static float width = 40;

    public ConditionCounting condition;

    protected void Awake()
    {
        Initialize();
    }

    protected void Initialize()
    {
        if (!initialized)
        {
            image = GetComponent<Image>();
            button = GetComponent<Button>();
            initialized = true;   
        }
    }

    protected void Start()
    {
        button.onClick.AddListener(OnClick);
    }

    public virtual (List<string> tips, Vector3 worldPosition, float offset) GetTipInfo()
    {
        Vector3 pos = WorldSystem.instance.cameraManager.currentCamera.WorldToScreenPoint(tooltipAnchor.transform.position);
        string desc = string.Format("<b>{0}</b>\n{1}", artifactData.itemName, artifactData.description);
        return (new List<string>{desc} , pos, width);
    }
    public void OnClick()
    {

    }
    public virtual void BindData(ArtifactData anArtifactData)
    {   
        Initialize();
        if (anArtifactData != null)
        {
            artifactData = anArtifactData;
            image.sprite = artifactData.artwork;
            id = artifactData.itemId;
            itemName = anArtifactData.itemName;
            itemEffect = ItemEffect.Factory(artifactData.itemEffectStruct, this);
            condition = new ConditionCounting(artifactData.conditionCounting, null, itemEffect.ApplyEffect, artifactData.conditionCountingOnTrueType, artifactData.conditionResetEvent);
            condition.Subscribe();
        }
    }

    public override void NotifyUsed()
    {
        if (tween != null) tween.Kill();
        tween = transform.DOScale(1.2f * Vector3.one, .3f).SetLoops(2, LoopType.Yoyo).OnKill(() => transform.localScale = Vector3.one);
    }
}


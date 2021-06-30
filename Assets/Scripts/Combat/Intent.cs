using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Intent : MonoBehaviour, IToolTipable
{
    public Image image;
    public string tooltipDescription;
    public Transform toolTipAnchor;
    RectTransform rect;
    Tween myTween;
    static float offset;
    static float startOffset;

    void Awake()
    {
        rect = image.GetComponent<RectTransform>();
        offset = 20;
        startOffset = 0;
    }

    void OnEnable()
    {
        myTween = DOTween.To(() => rect.anchoredPosition, x => rect.anchoredPosition = x, new Vector2(0, startOffset + offset), 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }

    private void OnDisable()
    {
        myTween?.Kill();
        rect.anchoredPosition = new Vector2(0, startOffset);
    }
    public (List<string> tips, Vector3 worldPosition) GetTipInfo()
    {
        Vector3 pos = WorldSystem.instance.cameraManager.currentCamera.WorldToScreenPoint(toolTipAnchor.position);
        return (new List<string>() {tooltipDescription}, pos);
    }
}

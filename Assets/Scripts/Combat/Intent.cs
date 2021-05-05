using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Intent : MonoBehaviour
{
    public Image image;
    RectTransform rect;
    Tween myTween;
    static float offset;
    static float startOffset;

    void Awake()
    {
        //Debug.Log("Awake---------------------------");
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
        //Debug.Log("Disable-----------------------------");
        myTween?.Kill();
        rect.anchoredPosition = new Vector2(0, startOffset);
    }
    public void DisableTween()
    {
        /*
        Debug.Log("Disable-----------------------------");
        myTween?.Kill();
        myTween = null;
        rect.anchoredPosition = new Vector2(0, startOffset);
        */
    }
}

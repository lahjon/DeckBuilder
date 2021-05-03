using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class EffectDisplay : MonoBehaviour, IToolTipable
{
    public TMP_Text effectLabel;
    public Image image;
    Tween myTween;
    public EffectType backingType;

    private void OnEnable()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(1, 0.2f);
    }

    public void SetLabel(string stackNr)
    {
        effectLabel.text = stackNr;
        CancelAnimation();
        myTween = effectLabel.transform.DOScale(1.3f, 0.2f).SetEase(Ease.InOutSine).SetLoops(1, LoopType.Yoyo);
    }

    public void CancelAnimation()
    {
        if(myTween != null)
        {
            myTween.Kill();
            myTween = null;
            effectLabel.transform.localScale = Vector3.one;
        }
    }

    public void SetSprite(Sprite sprite)
    {
        if(sprite != null) image.sprite = sprite;
    }

    public (List<string> tips, Vector3 worldPosition) GetTipInfo()
    {
        return (new List<string>() { backingType.GetDescription()}, transform.position);
    }
}

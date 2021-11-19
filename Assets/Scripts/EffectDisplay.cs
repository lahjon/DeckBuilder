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
    public CardEffect backingEffect;

    private void OnEnable()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(1, 0.2f);
    }

    public void SetBackingEffect(CardEffect effect)
    {
        backingEffect = effect;
        image.sprite = WorldSystem.instance.uiManager.GetSpriteByName(effect.type.ToString());
        image.color = effect.isBuff ? Color.green : Color.red; // REMOVE WHEN FINAL ICON;
    }

    public void RefreshLabel()
    {
        effectLabel.text = backingEffect.strStacked();
        CancelAnimation();
        if(effectLabel != null)
            myTween = effectLabel.transform.DOScale(1.3f, 0.2f).SetEase(Ease.InOutSine).SetLoops(1, LoopType.Yoyo);
    }

    public void CancelAnimation()
    {
        if(myTween != null)
        {
            myTween.Kill();
            myTween = null;
            if(effectLabel != null)
                effectLabel.transform.localScale = Vector3.one;
        }
    }

    public (List<string> tips, Vector3 worldPosition) GetTipInfo()
    {
        Vector3 pos = WorldSystem.instance.cameraManager.currentCamera.WorldToScreenPoint(transform.position);
        return (new List<string>{ backingEffect.type.toolTip}, pos);
    }
}

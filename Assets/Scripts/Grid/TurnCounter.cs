using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class TurnCounter : MonoBehaviour, IToolTipable
{
    public TMP_Text counterText;
    static float width = 50;
    int _counter;
    [HideInInspector] public int tilesUntilDanger;
    Tween tween;
    public string tilesLeftUntilBoss
    {
        get
        {
            if (_counter <= 0)
                return string.Format("The void is here!");
            else
                return string.Format(counter + " tiles left until the void comes closer!");

        }
    }
    public int counter
    {
        get => _counter;
        set 
        {
            tween?.Kill();
            _counter = value;
            float a = (float)_counter;
            float b = (float)tilesUntilDanger;
            float textScale = 1 - (counter / tilesUntilDanger * .5f);
            if (_counter <= 0)
            {
                tween = transform.DOScale(new Vector3(textScale, textScale, textScale), 0.6f).SetEase(Ease.InOutSine).SetLoops(-1).OnKill(() => transform.localScale = Vector3.one);
                counterText.text = "";
            }
            else if (_counter < tilesUntilDanger)
            {
                tween = transform.DOScale(new Vector3(textScale, textScale, textScale), 0.3f).SetEase(Ease.InOutSine).SetLoops(2).OnKill(() => transform.localScale = Vector3.one);
                counterText.text = (_counter).ToString();
            }
            else 
            {
                counterText.text = (_counter).ToString();
            }
        }
    }
    public (List<string> tips, Vector3 worldPosition, float offset) GetTipInfo()
    {
        Vector3 pos = WorldSystem.instance.cameraManager.currentCamera.WorldToScreenPoint(transform.position);
        return (new List<string>{tilesLeftUntilBoss} , pos, width);
    }
}

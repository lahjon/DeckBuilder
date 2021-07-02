using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class BossCounter : MonoBehaviour, IToolTipable
{
    public TMP_Text bossCounterText;
    int _counter;
    [HideInInspector] public int tilesUntilBoss;
    Tween tween;
    public string tilesLeftUntilBoss
    {
        get
        {
            if (_counter <= 0)
                return string.Format("Boss has appeard!");
            else
                return string.Format(counter + " tiles left until boss appears!");

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
            float b = (float)tilesUntilBoss;
            float textScale = 1 - (counter / tilesUntilBoss * .5f);
            Debug.Log(textScale);
            if (_counter <= 0)
            {
                tween = transform.DOScale(new Vector3(textScale, textScale, textScale), 0.6f).SetEase(Ease.InOutSine).SetLoops(-1);
                bossCounterText.text = "";
                WorldSystem.instance.gridManager.StartBoss();
            }
            else if (_counter < tilesUntilBoss)
            {
                tween = transform.DOScale(new Vector3(textScale, textScale, textScale), 0.3f).SetEase(Ease.InOutSine).SetLoops(2);
                bossCounterText.text = (_counter).ToString();
            }
            else 
            {
                bossCounterText.text = (_counter).ToString();
            }
        }
    }

    public void ResetCounter()
    {
        counter = tilesUntilBoss;
    }

    public (List<string> tips, Vector3 worldPosition) GetTipInfo()
    {
        Vector3 pos = WorldSystem.instance.cameraManager.currentCamera.WorldToScreenPoint(transform.position);
        return (new List<string>{tilesLeftUntilBoss} , pos);
    }
}

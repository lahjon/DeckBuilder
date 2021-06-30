using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class UIWarningController : MonoBehaviour
{
    public GameObject warningTextPrefab;
    public GameObject content;
    private int count;
    private int maxWarnings = 3;

    public void CreateWarning(string text, float time = 2f)
    {
        if (count < maxWarnings)
        {
            TMP_Text newUI = Instantiate(warningTextPrefab, content.transform).GetComponent<TMP_Text>();
            newUI.text = text;
            DOTween.To(() => newUI.color, x => newUI.color = x, new Color(1,1,1,0), time).SetEase(Ease.InSine).OnComplete(() =>{
                count--;
                Destroy(newUI.gameObject);
                });
            count++;
        }
    }
}

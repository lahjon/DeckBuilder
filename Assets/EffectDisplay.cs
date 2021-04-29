using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EffectDisplay : MonoBehaviour, IToolTipable
{
    public TMP_Text effectLabel;
    public Image image;

    float durationPopin = 0.2f;
    public Vector3 popinStartSize = new Vector3(0f, 0f, 0f);
    public Vector3 fullSize = Vector3.one;


    public EffectType backingType;

    IEnumerator popinFunction;

    private void Awake()
    {
        popinFunction = Popin();
    }

    private void OnEnable()
    {
        transform.localScale = popinStartSize;
        StartCoroutine(popinFunction);
    }

    public IEnumerator Popin()
    {
        float time = 0;
        while(time < durationPopin)
        {
            time += Time.deltaTime;
            transform.localScale = Vector3.Lerp(popinStartSize, fullSize, time / durationPopin);
            yield return null;
        }

        transform.localScale = fullSize;
    }

    public void SetLabel(string stackNr)
    {
         effectLabel.text = stackNr;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EffectDisplay : MonoBehaviour
{
    public TMP_Text effectLabel;

    public float durationPopin = 2;
    public Vector3 popinStartSize = new Vector3(0f, 0f, 0f);
    private void OnEnable()
    {
        transform.localScale = popinStartSize;
        
    }

    public IEnumerator Popin()
    {
        float time = 0;
        Vector3 fullSize = new Vector3(1f, 1f, 1f);
        while(time < durationPopin)
        {
            time += Time.deltaTime;
            transform.localScale = Vector3.Lerp(popinStartSize, fullSize, time / durationPopin);
            yield return null;
        }

        transform.localScale = fullSize;
    }

    public void SetLabel(int x)
    {
        effectLabel.text = x.ToString();
    }


}

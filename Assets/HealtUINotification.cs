using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class HealtUINotification : MonoBehaviour
{
    [HideInInspector]
    public HealthEffectsUI healthEffectsUI;

    public TMP_Text label;
    float timePassed;
    float animationDuration = 3f;
    public static float speed = 40f;
    public AnimationCurve decreasingRise;

    public Color32 startColor = new Color32(255,255,255,255);
    public Color32 endColor = new Color32(255, 255, 255, 0);

    public Func<float , Vector3> Kinetor;

    private int signHorizontal = 1;   

    private void OnEnable()
    {
        if (Kinetor is null)
            Kinetor = LinearRise;
    }

    public void ResetLabel(string newLabel, Color32 color, Transform parent, float duration = 2.5f)
    {
        SetColor(color);
        transform.localPosition = Vector3.zero;
        signHorizontal = UnityEngine.Random.Range(0f, 1f) < 0.5 ? -1 : 1; 
        label.text = newLabel;
        animationDuration = duration;
        timePassed = 0;
        gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        timePassed += Time.deltaTime;
        float pct = timePassed / animationDuration;
        label.color = Color32.Lerp(startColor, endColor, pct);
        transform.localPosition = Kinetor(pct);

        if (timePassed > animationDuration)
        {
            gameObject.SetActive(false);
            healthEffectsUI.uINotificators.Enqueue(this);
        }
    }


    public void SetColor(Color32 color)
    {
        startColor = color;
        label.color = color;
        endColor = new Color32(color.r, color.g, color.b, 0);
    }



    public Vector3 LinearRise(float pct)
    {
        return new Vector3(0, pct*speed);
    }

    public Vector3 DecreasingRise(float pct)
    {
        return new Vector3(0, decreasingRise.Evaluate(pct)*speed*2);
    }

    public Vector3 Poly2Rise(float pct)
    {
        float x = speed * pct;
        return new Vector3(signHorizontal*x*4, -Mathf.Pow(x,2)+10*x);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class HealtUINotification : MonoBehaviour
{
    public TMP_Text label;
    public float timePassed;
    public float animationDuration = 3f;
    public static float speed = 40f;

    public Color32 startColor = new Color32(255,255,255,255);
    public Color32 endColor = new Color32(255, 255, 255, 0);

    private Func<float , Vector3> Kinetor;

    private int signHorizontal = 1;   

    private void OnEnable()
    {
        if (Kinetor is null)
            Kinetor = LinearRise;
    }

    public void ResetLabel(string newLabel, float duration = 2.5f)
    {
        transform.localPosition = Vector3.zero;
        signHorizontal = UnityEngine.Random.Range(0f, 1f) < 0.5 ? -1 : 1; 
        label.text = newLabel;
        animationDuration = duration;
        timePassed = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timePassed += Time.deltaTime;
        float pct = timePassed / animationDuration;
        label.color = Color32.Lerp(startColor, endColor, pct);
        transform.localPosition = Kinetor(pct);

        if (timePassed > animationDuration)
            gameObject.SetActive(false);
    }


    public void SetColor(Color32 color)
    {
        startColor = color;
        endColor = new Color32(color.r, color.g, color.b, 0);
    }

    public void SetKineticFun(string strFunc)
    {
        if (strFunc == "LinearRise")
            Kinetor = LinearRise;
        else if (strFunc == "Poly2Rise")
            Kinetor = Poly2Rise;
        else
            Kinetor = LinearRise;
    }

    public Vector3 LinearRise(float pct)
    {
        return new Vector3(0, pct*speed);
    }

    public Vector3 Poly2Rise(float pct)
    {
        float x = speed * pct;
        return new Vector3(signHorizontal*x*4, -Mathf.Pow(x,2)+10*x);
    }

}

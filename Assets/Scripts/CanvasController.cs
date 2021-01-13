using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{

    public bool fadeToBlack = false;

    void Awake()
    {
        FadeToBlack();
    }
    public void DeactiveCanvas()
    {
        gameObject.SetActive(false);
    }
    public void FadeToBlack()
    {
        Color color = gameObject.GetComponent<Image>().color;
        if(fadeToBlack)
            StartCoroutine(FadeTo(0, 2, color));
    }
    IEnumerator FadeTo(float value, float time, Color color)
    {
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / time)
        {
            Color newColor = new Color(color.r, color.g, color.b, Mathf.Lerp(color.a,value,t));
            gameObject.GetComponent<Image>().color = newColor;
            yield return null;
        }
        DeactiveCanvas();
    }
}
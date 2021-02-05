using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenTransition : MonoBehaviour
{

    public AnimationCurve transitionCurve;
    public bool useCurve = false;
    public GameObject canvas;
    public void ToggleActive()
    {
        if(canvas.activeSelf == false)
        {
            canvas.SetActive(true);
            FadeToColor();
        }
        else
        {
            canvas.SetActive(false);
        }
    }

    private void FadeToColor()
    {
        WorldSystem.instance.SwapState(WorldState.Transition);
        Color color = canvas.GetComponent<Image>().color;
        if(useCurve == false)
            StartCoroutine(FadeTo(0, 2, color));
        else
            StartCoroutine(FadeToCurve(0, color));
    }
    IEnumerator FadeTo(float value, float time, Color color)
    {
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / time)
        {
            Color newColor = new Color(color.r, color.g, color.b, Mathf.Lerp(color.a,value,t));
            canvas.GetComponent<Image>().color = newColor;
            yield return null;
        }
        ToggleActive();
        canvas.GetComponent<Image>().color = color;
        WorldSystem.instance.SwapStatePrevious();
    }

    IEnumerator FadeToCurve(float value, Color color)
    {
        float time = transitionCurve.keys[transitionCurve.length -1].time;
        while(time > 0.0f)
        {
                Color newColor = new Color(color.r, color.g, color.b, transitionCurve.Evaluate(time));
                canvas.GetComponent<Image>().color = newColor;
                time -= Time.deltaTime;
                yield return null;
        }
        ToggleActive();
        canvas.GetComponent<Image>().color = color;
        WorldSystem.instance.SwapStatePrevious();
    }
}
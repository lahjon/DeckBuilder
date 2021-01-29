using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeathScreen : MonoBehaviour
{
    public AnimationCurve backgroundCurve;
    public AnimationCurve textCurve;
    public Canvas canvas;
    public TMP_Text text;
    public Button button;


    public void TriggerDeathscreen()
    {
        button.gameObject.SetActive(false);
        text.gameObject.SetActive(false);
        canvas.gameObject.SetActive(true);
        FadeBackgroundColor();
    }

    public void BackToMainMenu()
    {
        canvas.gameObject.SetActive(false);
        WorldSystem.instance.Reset();
    }

    private void FadeBackgroundColor()
    {
        WorldSystem.instance.SwapState(WorldState.Dead);
        Color color = canvas.GetComponent<Image>().color;
            StartCoroutine(FadeToCurveImage(color));
    }
    private void FadeTextColor()
    {
        WorldSystem.instance.SwapState(WorldState.Dead);
        Color color = text.color;
            StartCoroutine(FadeToCurveText(color));
    }
    IEnumerator FadeToCurveImage(Color color)
    {
        float time = backgroundCurve.keys[backgroundCurve.length -1].time;
        while(time > 0.0f)
        {
            Color newColor = new Color(color.r, color.g, color.b, backgroundCurve.Evaluate(time));
            canvas.GetComponent<Image>().color = newColor;
            time -= Time.deltaTime;
            yield return null;
        }
        canvas.GetComponent<Image>().color = color;
        text.gameObject.SetActive(true);
        FadeTextColor();
    }

    IEnumerator FadeToCurveText(Color color)
    {
        float time = backgroundCurve.keys[backgroundCurve.length -1].time;
        while(time > 0.0f)
        {
            Color newColor = new Color(color.r, color.g, color.b, backgroundCurve.Evaluate(time));
            text.color = newColor;
            time -= Time.deltaTime;
            yield return null;
        }
        button.gameObject.SetActive(true);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIWarningController : MonoBehaviour
{
    public GameObject warningTextPrefab;
    public GameObject content;
    public AnimationCurve fadeCurve;
    private int count;
    private int maxWarnings = 3;

    public void CreateWarning(string text)
    {
        if (count < maxWarnings)
        {
            GameObject newUI = Instantiate(warningTextPrefab, Vector3.zero, Quaternion.Euler(0, 0, 0), content.transform) as GameObject;
            newUI.GetComponent<TMP_Text>().text = text;
            FadeText(newUI);
            count++;
        }
    }

    public void FadeText(GameObject text)
    {
        Color color = text.GetComponent<TMP_Text>().color;
        //StartCoroutine(FadeTo(0, 2, color, text));
        StartCoroutine(FadeToCurve(0, color, text));

    }
    IEnumerator FadeTo(float value, float time, Color color, GameObject text)
    {
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / time)
        {
            Color newColor = new Color(color.r, color.g, color.b, Mathf.Lerp(color.a,value,t));
            text.GetComponent<TMP_Text>().color = newColor;
            yield return null;
        }
        DestroyImmediate(text);
        count--;
    }

    IEnumerator FadeToCurve(float value, Color color, GameObject text)
    {
        float time = fadeCurve.keys[fadeCurve.length - 1].time;
        while (time > 0.0f)
        {
            time -= Time.deltaTime;
            Color newColor = new Color(color.r, color.g, color.b, fadeCurve.Evaluate(time));
            text.GetComponent<TMP_Text>().color = newColor;
            yield return null;
        }
        DestroyImmediate(text);
        count--;
    }
}

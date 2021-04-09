using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EffectionNotificationFade : MonoBehaviour
{
    public TMP_Text label;
    public float timePassed;
    public float animationDuration = 3f;
    public static float speed = 40f;

    public Color32 startColor = new Color32(255,255,255,255);
    public Color32 endColor = new Color32(255, 255, 255, 0);

    public void ResetLabel(string newLabel)
    {
        transform.localPosition = Vector3.zero;
        label.text = newLabel;
        timePassed = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timePassed += Time.deltaTime;
        label.color = Color32.Lerp(startColor, endColor, timePassed / animationDuration);

        transform.localPosition += new Vector3(0, Time.deltaTime * speed);

        if (timePassed > animationDuration)
            gameObject.SetActive(false);
    }
}

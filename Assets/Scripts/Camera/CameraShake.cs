using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public GameObject shakeObject;
    public float magnitude;
    public float duration;

    public void ShakeCamera()
    {
        StartCoroutine(StartShake());
    }

    IEnumerator StartShake()
    {
        float elapsed = 0.0f;
        float newMagnitude = magnitude;
        while (elapsed < duration)
        {
            float x = Random.Range(-1.0f, 1.0f);
            float y = Random.Range(-1.0f, 1.0f);
            float z = shakeObject.transform.localPosition.z;
            shakeObject.transform.localPosition = new Vector3(x, y, z) * newMagnitude;
            newMagnitude = Mathf.Lerp(magnitude, 0, elapsed/duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        shakeObject.transform.localPosition = Vector3.zero;
    }
}

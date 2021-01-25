using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour 
{
    public Camera currentCamera;
    public Camera previousCamera;

    public Camera townCamera;
    public GameObject act1CameraPos;
    public GameObject act2CameraPos;
    public ScreenTransition screenTransition;

    public void ToggleCamera(Camera aCamera)
    {
        screenTransition.gameObject.SetActive(true);
        if(currentCamera != aCamera)
        {
            previousCamera = currentCamera;
            currentCamera = aCamera;
            previousCamera.gameObject.SetActive(false);
            currentCamera.gameObject.SetActive(true);
        }
        else
        {
            currentCamera = previousCamera;
            previousCamera = aCamera;
            currentCamera.gameObject.SetActive(true);
            previousCamera.gameObject.SetActive(false);
        }
    }
}
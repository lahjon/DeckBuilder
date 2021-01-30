using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraManager : MonoBehaviour 
{
    public Camera mainCamera;
    public Camera currentCamera;
    public Camera previousCamera;

    public Camera townCamera;
    public List<GameObject> actCameraPos;
    public ScreenTransition screenTransition;

    public void ToggleCamera(Camera aCamera, bool doTransition = true)
    {
        if(doTransition)
            screenTransition.ToggleActive();
            
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
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraManager : MonoBehaviour 
{
    public Camera mainCamera;
    public Transform combatCameraPos;
    public Transform shopCameraPos;
    public List<Transform> actCameraPos;
    public ScreenTransition screenTransition;

    public void CameraGoto(WorldState worldstate, bool doTransition = true)
    {
        switch (worldstate)
        {
            case WorldState.Combat:
                mainCamera.transform.position = combatCameraPos.position;
                break;

            case WorldState.Overworld:
                int act = WorldSystem.instance.act;
                mainCamera.transform.position = actCameraPos[act - 1].position;
                break;

            case WorldState.Shop:
                mainCamera.transform.position = shopCameraPos.position;
                break;   
             
            default:
                break;
        }
    }

    public void UpdateCameras()
    {

    }



    // public void ToggleCamera(Camera aCamera, bool doTransition = true)
    // {
    //     if(doTransition)
    //         screenTransition.ToggleActive();
            
    //     if(currentCamera != aCamera)
    //     {
    //         previousCamera = currentCamera;
    //         currentCamera = aCamera;
    //         previousCamera.gameObject.SetActive(false);
    //         currentCamera.gameObject.SetActive(true);
    //     }
    //     else
    //     {
    //         currentCamera = previousCamera;
    //         previousCamera = aCamera;
    //         currentCamera.gameObject.SetActive(true);
    //         previousCamera.gameObject.SetActive(false);
    //     }
    // }
}
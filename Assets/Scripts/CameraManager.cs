using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour 
{
    public Camera[] cameras;

    private int currentCameraIndex;
    void Start () 
    {
        currentCameraIndex = 0;
    }

    public void SetActiveCamera(int index)
    {
        for (int i = 0; i < cameras.Length; i++) 
        {
            if(i == index)
                cameras[i].gameObject.SetActive(true);
            else
                cameras[i].gameObject.SetActive(false);
        }
    }
}
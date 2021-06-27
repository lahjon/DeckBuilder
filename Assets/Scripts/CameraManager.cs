using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraManager : Manager 
{
    public Camera mainCamera;
    public Camera combatCamera;
    public List<Canvas> swapCanvas = new List<Canvas>();
    public Camera currentCamera;

    protected override void Awake()
    {
        base.Awake(); 
        world.cameraManager = this;
        currentCamera = mainCamera;
    }

    public void SwapToMain()
    {
        foreach (Canvas canvas in swapCanvas)
        {
            canvas.worldCamera = mainCamera;
        }
        world.cameraManager.combatCamera.gameObject.SetActive(false);
        world.cameraManager.mainCamera.gameObject.SetActive(true);
        currentCamera = mainCamera;
    }
    public void SwapToCombat()
    {
        foreach (Canvas canvas in swapCanvas)
        {
            canvas.worldCamera = combatCamera;
        }
        world.cameraManager.combatCamera.gameObject.SetActive(true);
        world.cameraManager.mainCamera.gameObject.SetActive(false);
        currentCamera = combatCamera;
    }
}
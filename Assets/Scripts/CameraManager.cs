using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraManager : Manager 
{
    public Camera mainCamera;
    public Camera combatCamera;
    public List<Canvas> swapCanvas = new List<Canvas>();

    protected override void Awake()
    {
        base.Awake(); 
        world.cameraManager = this;
    }

    public void SwapToMain()
    {
        foreach (Canvas canvas in swapCanvas)
        {
            canvas.worldCamera = mainCamera;
        }
        world.cameraManager.combatCamera.gameObject.SetActive(false);
        world.cameraManager.mainCamera.gameObject.SetActive(true);
    }
    public void SwapToCombat()
    {
        foreach (Canvas canvas in swapCanvas)
        {
            canvas.worldCamera = combatCamera;
        }
        world.cameraManager.combatCamera.gameObject.SetActive(true);
        world.cameraManager.mainCamera.gameObject.SetActive(false);
    }
}
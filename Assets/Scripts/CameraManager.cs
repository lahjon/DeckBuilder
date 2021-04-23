using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraManager : Manager 
{
    public Camera mainCamera;
    public Transform combatCameraPos;
    public Transform shopCameraPos;
    public List<Transform> actCameraPos;

    protected override void Awake()
    {
        base.Awake(); 
        world.cameraManager = this;
    }

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
}
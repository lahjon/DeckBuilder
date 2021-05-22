using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class HexMapController : MonoBehaviour
{
    float zoomIn, zoomOut;
    [SerializeField] Camera cam;
    Vector3 dragStartPosition;
    Vector3 cameraOrigin;
    [SerializeField] Vector3 newPosition;
    Vector3 dragCurrentPosition;
    public float minX, minY, maxX, maxY;
    public float panSpeed;
    public float panSensitivity;
    GridManager gridManager;
    [HideInInspector] float timer;
    public bool disableInput;
    Vector3 cameraPosition;
    public int zoomStep = 1;

    void Update()
    {
        if (!disableInput)
        {
            PanCamera();
            ZoomCamera();  
        }

        if (cam.transform.position != newPosition)
        {
            MoveCamera();
        }
    }


    void Start()
    {
        cameraOrigin = cam.transform.position;
        newPosition = cameraOrigin;
        gridManager = GetComponent<GridManager>();
        zoomIn = 7;
        zoomOut = cam.transform.localPosition.z;
        zoomStep = 1;
    }

    public void FocusTile(HexTile focusTile, ZoomState zoomstate, bool endDisable = false)
    {
        if (focusTile != null)
        {
            if (zoomstate == ZoomState.Inner)
            {
                Zoom(ZoomState.Inner, focusTile.transform.position, endDisable);
            }
            else if (zoomstate == ZoomState.Mid)
            {
                Zoom(ZoomState.Mid, focusTile.transform.position, endDisable);
            }
        }
    }

    public void Zoom(ZoomState zoomState = ZoomState.Mid, Vector3? pos = null, bool endDisable = false)
    {
        disableInput = true;
        timer = 0.5f;
        Vector3 zoomPosition;
        if (pos != null)
        {
            zoomPosition = (Vector3)pos;
        }
        else if (GetMousePosition() is Vector3 mousePosition)
        {
            zoomPosition = mousePosition;
        }
        else
        {
            zoomPosition = cam.transform.position;
        }

        if (zoomState == ZoomState.Inner)
        {
            zoomStep = 0;
            zoomPosition.z = zoomOut + zoomIn;
            cam.transform.DOMove(zoomPosition, 1.0f).SetEase(Ease.InExpo).OnComplete(() => {
                ZoomCallback(endDisable, 3f);
                if (gridManager.currentTile != null && gridManager.currentTile.tileState != TileState.Completed)
                {
                    gridManager.currentTile.StopFadeInOutColor();
                }
            });
        }
        else if (zoomState == ZoomState.Mid)
        {
            zoomStep = 1;
            zoomPosition.z = zoomOut;
            cam.transform.DOMove(zoomPosition, 1.0f).SetEase(Ease.InExpo).OnComplete(() => {
                ZoomCallback(endDisable, 5f);
                if (gridManager.currentTile != null && gridManager.currentTile.tileState != TileState.Completed)
                {
                    gridManager.currentTile.StartFadeInOutColor();
                }
            });
        }
        else if (zoomState == ZoomState.Outer)
        {
            zoomStep = 2;
            cam.transform.DOMoveZ(zoomOut - zoomIn, 1.0f).SetEase(Ease.InExpo).OnComplete(() => {
                ZoomCallback(endDisable, 5f);
            });
        }
    }

    void ZoomCallback(bool endDisable, float panSensitivity)
    {
        disableInput = endDisable;
        panSensitivity = 5f;
        newPosition = cam.transform.position;
        timer = 0;
        gridManager.animator.SetBool("IsPanning", false);
    }

    void ZoomCamera()
    {
        if(Input.mouseScrollDelta.y > 0) // zoom in
        {
            if (zoomStep > 0)
            {
                zoomStep--;
                Zoom((ZoomState)zoomStep);
            }
        }
        else if(Input.mouseScrollDelta.y < 0) // zoom out
        {
            if (zoomStep < 2)
            {
                zoomStep++;
                Zoom((ZoomState)zoomStep);
            }
        }
    }

    Vector3? GetMousePosition()
    {
        Plane plane = new Plane(Vector3.forward, new Vector3(0,0,10));
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        float entry;

        if (plane.Raycast(ray, out entry))
        {
            return ray.GetPoint(entry);
        }
        else
        {
            return null;
        }
    }

    void PanCamera()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3? ray = GetMousePosition();
            dragStartPosition = ray != null ? (Vector3)ray : dragStartPosition;
            timer = 0;

            timer += Time.deltaTime;
        }

        if (Input.GetMouseButtonUp(0))
        {
            timer = 0;
        }

        if (Input.GetMouseButton(0) && timer > 0)
        {
            Vector3? ray = GetMousePosition();
            if (ray != null)
            {
                dragCurrentPosition = (Vector3)ray;
                newPosition = cam.transform.position + (dragStartPosition - dragCurrentPosition) * panSensitivity;
            }

            timer += Time.deltaTime;
        }
    }

    void MoveCamera()
    {
        cameraPosition = Vector3.Lerp(cam.transform.position, newPosition, Time.deltaTime * panSpeed);

        if (timer > .3f || Vector3.Distance(cameraPosition, newPosition) > .1f)
        {
            gridManager.animator.SetBool("IsPanning", true);
        }
        else if (gridManager.animator.GetBool("IsPanning"))
        {
            gridManager.animator.SetBool("IsPanning", false);
        }

        cam.transform.position = new Vector3(Mathf.Clamp(cameraPosition.x, minX, maxX), Mathf.Clamp(cameraPosition.y, minY, maxY), cameraPosition.z);
    }
}

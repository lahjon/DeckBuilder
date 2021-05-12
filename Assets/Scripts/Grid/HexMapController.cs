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
    Vector3 newPosition;
    Vector3 dragCurrentPosition;
    public float minX, minY, maxX, maxY;
    public float panSpeed;
    public float panSensitivity;
    GridManager gridManager;
    [HideInInspector] float timer;
    public bool disableInput;
    Vector3 cameraPosition;
    [SerializeField] int _zoomStep = 1;
    public int zoomStep
    {
        get
        {
            return _zoomStep;
        }
        set
        {
            if (value >= 0 && value <= 2)
                _zoomStep = value; 

            if (_zoomStep == 0)
            {
                ZoomInner();
            }
            else if (_zoomStep == 1)
            {
                ZoomMid();
            }
            else if (_zoomStep == 2)
            {
                ZoomOuter();
            }
        }
    }
    void Update()
    {
        if (!disableInput)
        {
            PanCamera();
            ZoomCamera();  
        }
    }


    void Start()
    {
        cameraOrigin = cam.transform.position;
        newPosition = cameraOrigin;
        gridManager = GetComponent<GridManager>();
        zoomIn = 7;
        zoomOut = cam.transform.localPosition.z;
        _zoomStep = 1;
    }

    public void FocusTile(HexTile focusTile)
    {
        if (focusTile != null)
        {
            _zoomStep = 0;
            ZoomInner(focusTile.transform.position);
        }
    }
    public void FocusOverview(bool disable)
    {
        ZoomOuter(disable);
    }
    void ZoomInner(Vector3? pos = null)
    {
        disableInput = true;
        Vector3 zoomPosition;

        if (pos != null)
        {
            zoomPosition = (Vector3)pos;
        }
        else
        {
            Vector3? mousePosition = GetMousePosition();
            if (mousePosition != null)
            {
                zoomPosition = (Vector3)mousePosition;
            }
            else
            {
                zoomPosition = cam.transform.position;
            }
            
        }


        zoomPosition.z = zoomOut + zoomIn;

        cam.transform.DOMove(zoomPosition, 1.0f).SetEase(Ease.InExpo).OnComplete(() => {
            disableInput = false;
            panSensitivity = 3f;
            newPosition = cam.transform.position;
        });
    }
    void ZoomMid()
    {
        disableInput = true;
        cam.transform.DOMoveZ(zoomOut, 1.0f).SetEase(Ease.InExpo).OnComplete(() => {
            disableInput = false;
            panSensitivity = 5f;

            newPosition = cam.transform.position;
        });
    }
    void ZoomOuter(bool disable = false)
    {
        disableInput = disable;
        cam.transform.DOMoveZ(zoomOut - zoomIn, 1.0f).SetEase(Ease.InExpo).OnComplete(() => {
            disableInput = false;
            panSensitivity = 5f;

            newPosition = cam.transform.position;
        });
    }

    void ZoomCamera()
    {
        if(Input.mouseScrollDelta.y > 0) // zoom in
        {
            //if (gridManager.hoverTile != null)
            zoomStep--;
        }
        else if(Input.mouseScrollDelta.y < 0) // zoom out
        {
            //if (gridManager.hoverTile != null)
            zoomStep++;
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

        cameraPosition = Vector3.Lerp(cam.transform.position, newPosition, Time.deltaTime * panSpeed);

        if (timer > .3f || Vector3.Distance(cameraPosition, newPosition) > .1f)
        {
            gridManager.animator.SetBool("IsPanning", true);
        }
        else
        {
            gridManager.animator.SetBool("IsPanning", false);
        }

        cam.transform.position = new Vector3(Mathf.Clamp(cameraPosition.x, minX, maxX), Mathf.Clamp(cameraPosition.y, minY, maxY), cameraPosition.z);
    }
}

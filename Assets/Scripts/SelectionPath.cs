using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionPath : MonoBehaviour
{
    [SerializeField] Transform[] controlPoints;
    Vector3 gizmosPosition;
    public GameObject pathIcon;
    public GameObject pathIconEnd;
    List<GameObject> pathIcons = new List<GameObject>();
    public bool enableDebug;
    CombatController combatController;

    void Start()
    {
        CreatePath();
        combatController = WorldSystem.instance.combatManager.combatController;
    }

    void OnDrawGizmos()
    {
        if (enableDebug)
        {
            for(float t = 0; t <= 1; t += 0.05f)
            {
                gizmosPosition = Mathf.Pow(1 - t, 3) * controlPoints[0].position + 3 * Mathf.Pow(1 - t, 2) * t * controlPoints[1].position + 3 * (1 - t) * Mathf.Pow(t, 2) * controlPoints[2].position + Mathf.Pow(t, 3) * controlPoints[3].position;

                Gizmos.DrawSphere(gizmosPosition, 0.1f);
            }

            Gizmos.DrawLine(new Vector3(controlPoints[0].position.x, controlPoints[0].position.y, controlPoints[0].position.z), new Vector3(controlPoints[1].position.x, controlPoints[1].position.y, controlPoints[1].position.z));
            Gizmos.DrawLine(new Vector3(controlPoints[2].position.x, controlPoints[2].position.y, controlPoints[2].position.z), new Vector3(controlPoints[3].position.x, controlPoints[3].position.y, controlPoints[3].position.z));
        }
    }

    public void StartFollow()
    {
        foreach (GameObject obj in pathIcons)
        {
            obj.SetActive(true);
        }
    }

    public void StopFollow()
    {
        foreach (GameObject obj in pathIcons)
        {
            obj.SetActive(false);
        }
    }

    public void FollowPath()
    {
        Vector3 targetPos = WorldSystem.instance.cameraManager.mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
        int counter = 0;
        controlPoints[3].position = targetPos;
        for(float t = 0; t <= 1; t += 0.05f)
        {
            Vector3 iconPosition = Mathf.Pow(1 - t, 3) * controlPoints[0].position + 3 * Mathf.Pow(1 - t, 2) * t * controlPoints[1].position + 3 * (1 - t) * Mathf.Pow(t, 2) * controlPoints[2].position + Mathf.Pow(t, 3) * controlPoints[3].position;
            pathIcons[counter].transform.position = iconPosition;
            counter++;
        }
        pathIcons[pathIcons.Count - 1].transform.position = targetPos;
        pathIcons[pathIcons.Count - 1].transform.rotation = Quaternion.LookRotation(pathIcons[pathIcons.Count - 2].transform.position, pathIcons[pathIcons.Count - 1].transform.position);

    }

    void CreatePath()
    {
        GameObject obj;
        for(float t = 0; t <= 1; t += 0.05f)
        {
            obj = Instantiate(pathIcon, Vector3.zero, Quaternion.identity, transform);
            float scale = t.Remap(0, 1, .75f, 1.5f);
            obj.transform.localScale = new Vector3(scale, scale, scale);
            obj.SetActive(false);
            pathIcons.Add(obj);
        }
        obj = Instantiate(pathIconEnd, Vector3.zero , Quaternion.identity, transform);
        obj.transform.localScale = new Vector3(.2f, .2f, .2f);
        pathIcons.Add(obj);
    }

}

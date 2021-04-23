using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationSpot : MonoBehaviour
{
    public float size;
    void Start()
    {
        
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(gameObject.transform.position, size);
    }

}

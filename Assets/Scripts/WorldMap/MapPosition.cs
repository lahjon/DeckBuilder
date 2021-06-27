using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPosition : MonoBehaviour
{
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(transform.position, 50);
    }

}

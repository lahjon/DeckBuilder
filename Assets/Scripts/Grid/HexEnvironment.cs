using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexEnvironment : MonoBehaviour
{
    public GameObject objects;
    public GameObject map;
    public void RemoveColliders()
    {
        HashSet<Collider2D> colliderObjects = new HashSet<Collider2D>();
        for (int i = 0; i < objects.transform.childCount; i++)
        {
            Collider2D obj;
            objects.transform.GetChild(i).TryGetComponent<Collider2D>(out obj);
            if (obj != null)
            {
                colliderObjects.Add(obj);
            }
        }

        HashSet<Collider2D> colliderMaps = new HashSet<Collider2D>();
        for (int i = 0; i < map.transform.childCount; i++)
        {
            Collider2D obj;
            map.transform.GetChild(i).TryGetComponent<Collider2D>(out obj);
            if (obj != null)
            {
                colliderMaps.Add(obj);
            }
        }

        foreach (Collider2D mCol in colliderMaps)
        {
            foreach (Collider2D oCol in colliderObjects)
            {
                if (mCol.IsTouching(oCol))
                {
                    Destroy(oCol.gameObject);
                } 
            }

            Destroy(mCol.GetComponent<Rigidbody2D>());
            Destroy(mCol.GetComponent<Collider2D>());
        }
        foreach (Collider2D oCol in colliderObjects)
        {
            if (oCol != null)
            {
                Destroy(oCol.GetComponent<Collider2D>());
            }
        }
    }

}

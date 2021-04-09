using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTile : MonoBehaviour
{
    public Sprite artwork;
    public Vector3Int coord;
    public bool available;
    void OnMouseUp()
    {
        transform.Rotate(new Vector3(0,0,-60));
        Debug.Log("Mouse Pressed");
    }

    void OnMouseOver()
    {
        //Debug.Log("Mouse Over");
    }

    void OnMouseExit()
    {
        Debug.Log("Mouse Exit");
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using UnityEngine.UI;

public class TileSelector : MonoBehaviour
{
    const float addedHeight = 0.2f;
    public void Show(HexCell cell)
    {
        transform.position = new Vector3(cell.transform.position.x, (HexMetrics.elevationStep * cell.Elevation) + addedHeight, cell.transform.position.z);
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
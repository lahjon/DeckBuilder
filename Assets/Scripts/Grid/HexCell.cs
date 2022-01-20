using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using UnityEngine.UI;

public class HexCell : MonoBehaviour
{
    public HexCoordinates coordinates;
    public MeshCollider meshCollider;
    public Color color;
    int _elevation;
    public int Elevation 
    {
		get => _elevation;
		set => _elevation = value;
	}
    public bool IsUnderwater => Elevation < 1;
	
    public static HexGrid hexGrid;
    public HexCell[] neighbours = new HexCell[6];
    void Awake()
    {
        meshCollider = gameObject.AddComponent<MeshCollider>();
        if (hexGrid == null) hexGrid = HexGrid.instance;
    }
    void OnMouseEnter() 
    {
        hexGrid.tileSelector.Show(this);
    }
    void OnMouseExit() 
    {
        hexGrid.tileSelector.Hide();
    }
    public Vector3 Position
    {
        get => new Vector3(transform.localPosition.x, transform.localPosition.y + (Elevation * HexMetrics.elevationStep), transform.localPosition.z);
    }
    public HexCell GetNeighbor(HexDirection direction) 
    {
		return neighbours[(int)direction];
	}
    public void SetNeighbour(HexDirection direction, HexCell cell) 
    {
		neighbours[(int)direction] = cell;
		cell.neighbours[(int)direction.Opposite()] = this;
	}
    public HexEdgeType GetEdgeType(HexDirection direction) 
	{
		return HexMetrics.GetEdgeType(Elevation, neighbours[(int)direction].Elevation);
	}
    public HexEdgeType GetEdgeType(HexCell otherCell) 
    {
		return HexMetrics.GetEdgeType(Elevation, otherCell.Elevation);
	}
}
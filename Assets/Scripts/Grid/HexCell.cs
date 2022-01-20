using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using UnityEngine.UI;

public class HexCell : MonoBehaviour
{
    public HexCoordinates coordinates;
    public GameObject debug;
    public MeshCollider meshCollider;
    public bool Blocked;
    int distance;
    public int Distance{get; set;}
    public HexCell PathFrom{get; set;}
    public int SearchHeuristic{get; set;}
    public int SearchPriority => distance + SearchHeuristic;
    public int SearchPhase { get; set; }
    public HexCell NextWithSamePriority { get; set; }
    public int TerrainTypeIndex
    {
        get => HexGrid.instance.meshColors.IndexOf(color);
        set => color = HexGrid.instance.meshColors[value];
    }
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
        TerrainTypeIndex = 0;
    }
    void OnMouseEnter() 
    {
        hexGrid.tileSelector.Show(this);
        if (hexGrid.FindPath(hexGrid.currentCell, this)) hexGrid.ShowPath();
    }
    void OnMouseExit() 
    {
        hexGrid.tileSelector.Hide();
        hexGrid.ClearPath();
    }

    void OnMouseUp()
    {
        hexGrid.currentCell = this;
    }

    public void EnableHighlight()
    {
        debug.SetActive(true);
    }
    public void DisableHighlight()
    {
        debug.SetActive(false);
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
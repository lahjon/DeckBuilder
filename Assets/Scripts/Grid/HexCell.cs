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
    public TileType _tileType;
    public TileType TileType
    {
        get => _tileType;
        set
        {
            _tileType = value;
            if (Elevation == 1)
            {
                color = HexGrid.instance.meshColors[(int)_tileType];
            }
        }
    }
    public bool Blocked
    {
        get => Elevation != 1;
    }
    int _distance;
    public int Distance
    {
        get => _distance;
        set => _distance = value;
    }
    public HexCell PathFrom{get; set;}
    public int SearchHeuristic{get; set;}
    public int SearchPriority => _distance + SearchHeuristic;
    public int SearchPhase { get; set; }
    public HexCell NextWithSamePriority { get; set; }
    // public int TerrainTypeIndex
    // {
    //     get => HexGrid.instance.meshColors.IndexOf(color);
    //     set => color = HexGrid.instance.meshColors[value];
    // }
    public Color color;
    int _elevation;
    public int Elevation 
    {
		get => _elevation;
		set
        {
            _elevation = value;
            if (_elevation < 0)
            {
                color = HexGrid.instance.meshColors[1];
            }
            else if (_elevation == 0)
            {
                color = HexGrid.instance.meshColors[2];
            }
            else if (_elevation == 1)
            {
                color = HexGrid.instance.meshColors[3];
            }
            else if (_elevation == 2)
            {
                color = HexGrid.instance.meshColors[4];
            }
        }
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
        if (hexGrid.FindPath(hexGrid.currentCell, this)) hexGrid.ShowPath();
    }
    void OnMouseExit() 
    {
        hexGrid.tileSelector.Hide();
        hexGrid.ClearPath();
    }

    void OnMouseUp()
    {
        if (!Blocked && hexGrid.currentCell != this)
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
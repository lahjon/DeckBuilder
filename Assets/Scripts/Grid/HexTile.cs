using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HexTile : MonoBehaviour
{
    TileState _tileState; 
    public Sprite artwork;
    public Vector3Int coord;
    public List<int> availableDirections = new List<int>();
    public List<SpriteRenderer> exits = new List<SpriteRenderer>();
    GridManager gridManager;
    Vector3 startPosition;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    public TileState tileState
    {
        get
        {
            return _tileState;
        }

        set 
        {
            _tileState = value;

            if (_tileState == TileState.Active)
            {
                gridManager.activeTiles.Add(this);
            }
            else if (_tileState == TileState.Completed)
            {
                gridManager.completedTiles.Add(this);
                spriteRenderer.color = Color.gray;
            }
        }
    }
    public void Init()
    {
        gridManager = WorldSystem.instance.gridManager;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Activate(TileState aTileState = TileState.Active)
    {
        availableDirections = gridManager.AddNeighbours();
        availableDirections.ForEach(x => exits[x].gameObject.SetActive(true));
        spriteRenderer.sprite = gridManager.sprites[0];
        tileState = aTileState;
    }

    void Update()
    {
        // if (selected)
        // {
        //     if (Input.GetMouseButtonDown(1))
        //     {
        //         TestPlacement();
        //     }
        //     CheckPlacement();
        // }
    }
    
    void OnMouseDown()
    {
        switch (tileState)
        {
            case TileState.Inactive:
                //Debug.Log(gridManager.TilePlacementValid(gridManager.activeTile));
                break;

            case TileState.Inventory:
                if (gridManager.activeTile == null)
                {
                    StartPlacement();
                }
                break;

            case TileState.Placement:
                //Debug.Log(gridManager.TilePlacementValid(this));
                transform.Rotate(new Vector3(0,0,-60));
                break;

            case TileState.Active:
                //Debug.Log(gridManager.TilePlacementValid(this));
                break;


            case TileState.Completed:
                //Debug.Log(gridManager.TilePlacementValid(this));
                break;

            default:
                break;
        }
    }

    void OnMouseUp()
    {
        switch (tileState)
        {
            case TileState.Inactive:
                break;

            case TileState.Inventory:
                
                break;

            case TileState.Placement:
                transform.Rotate(new Vector3(0,0,-60));
                break;

            case TileState.Active:
                break;


            case TileState.Completed:
                break;

            default:
                break;
        }
    }
    void StartPlacement()
    {
        startPosition = transform.position;
        spriteRenderer.sortingOrder += 1;
        GetComponent<PolygonCollider2D>().enabled = false;
        gridManager.InPlacement(this);
    }
    public void CheckPlacement()
    {
        GetComponent<PolygonCollider2D>().enabled = true;
        if (gridManager.hoverTilePosition != Vector3.zero && gridManager.hoverTile != null && gridManager.TilePlacementValidStart(this))
        {
            gridManager.oldHoverTile = gridManager.hoverTile;
            coord = gridManager.hoverTile.coord;
            gridManager.InRotation();
        }
        else
        {
            EndPlacement();
        }
    }

    public void EndPlacement()
    {
        transform.position = startPosition;
        
        gridManager.ExitPlacement();
    }

    public void ConfirmTilePlacement()
    {
        gridManager.tiles[gridManager.activeTile.coord] = gridManager.activeTile;
        transform.position = gridManager.CellPosToWorldPos(coord);
        Destroy(gridManager.oldHoverTile.gameObject);
        gridManager.oldHoverTile = null;
        //tileState = TileState.Active;

        // DEBUG
        tileState = TileState.Completed;
    }


    void OnMouseEnter()
    {
        //Debug.Log("Enter");
    }

    void OnMouseOver()
    {
        if (tileState == TileState.Inactive && gridManager.hoverTile != this)
        {
            gridManager.hoverTilePosition = transform.position;
            gridManager.hoverTile = this;;
            if (gridManager.activeTile != null && gridManager.oldHoverTile == null)
            {
                gridManager.activeTile.coord = coord;
            }
        }
    }

    void OnMouseExit()
    {
        //Debug.Log("Yes");
        gridManager.hoverTilePosition = Vector3.zero;
        gridManager.hoverTile = null;
    }


}

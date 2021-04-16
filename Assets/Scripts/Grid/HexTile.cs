using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HexTile : MonoBehaviour
{
    [SerializeField] TileState _tileState; 
    [HideInInspector] public Sprite artwork;
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

    public void Activate(TileState aTileState = TileState.Active, bool activeDebug = true)
    {
        availableDirections = gridManager.AddNeighbours();
        if (activeDebug)
        {
            availableDirections.ForEach(x => exits[x].gameObject.SetActive(true));
        }
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
    


    void FlipDownNewTile()
    {
        spriteRenderer.color = Color.white;
        gridManager.IsComplete();
        tileState = TileState.Animation;
        LeanTween.rotateAround(gameObject, new Vector3(0,1,0), 270.0f, 1.5f).setEaseInCubic().setOnComplete(() => FlipUpNewTile());
    }

    public void FlipUpNewTile()
    {
        Activate(TileState.Active, false);
        List<int> requiredExits = gridManager.GetNewExits(this);

        foreach (int dir in requiredExits)
        {
            if(!availableDirections.Contains(dir))
            {
                availableDirections[requiredExits.IndexOf(dir)] = dir;
            }
        }
        availableDirections.ForEach(x => exits[x].gameObject.SetActive(true));

        LeanTween.rotateAround(gameObject, new Vector3(0,1,0), 90.0f, 0.5f).setEaseOutCubic().setOnComplete(
            () => tileState = TileState.Completed
        );
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

    void OnMouseUp()
    {
        switch (tileState)
        {
            case TileState.Inactive:
                if (gridManager.gridState == GridState.Idle)
                {
                    FlipDownNewTile();
                }
                break;

            case TileState.Inventory:
                if (gridManager.activeTile == null && gridManager.gridState != GridState.Complete)
                {
                    StartPlacement();
                }
                break;

            case TileState.Placement:
                //Debug.Log(gridManager.TilePlacementValid(this));
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


    void OnMouseEnter()
    {
        switch (tileState)
        {
            case TileState.Inactive:
                if (gridManager.gridState == GridState.Idle && _tileState == TileState.Inactive)
                {
                    if (gridManager.activeTile == null && gridManager.CheckFreeSlot(this))
                    {
                        spriteRenderer.color = Color.green;
                    }
                    else
                    {
                        spriteRenderer.color = Color.red;
                    }
                }

                break;

            case TileState.Inventory:
                
                break;

            case TileState.Placement:
                break;

            case TileState.Active:
                break;


            case TileState.Completed:
                break;

            default:
                break;
        }
    }

    void OnMouseOver()
    {
        switch (tileState)
        {
            case TileState.Inactive:
                if (gridManager.hoverTile != this)
                {
                    gridManager.hoverTilePosition = transform.position;
                    gridManager.hoverTile = this;
                    if (gridManager.activeTile != null && gridManager.oldHoverTile == null)
                    {
                        gridManager.activeTile.coord = coord;
                    }
                }

                break;

            case TileState.Inventory:
                
                break;

            case TileState.Placement:
                break;

            case TileState.Active:
                break;


            case TileState.Completed:
                break;

            default:
                break;
        }

    }

    void OnMouseExit()
    {
        if (_tileState == TileState.Inactive)
        {
            spriteRenderer.color = Color.white;
        }
        gridManager.hoverTilePosition = Vector3.zero;
        gridManager.hoverTile = null;
    }


}

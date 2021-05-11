using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class HexTile : MonoBehaviour
{
    [SerializeField] TileState _tileState; 
    [HideInInspector] public Sprite artwork;
    public Vector3Int coord;
    public List<int> availableDirections = new List<int>();
    public List<int> lockedDirections = new List<int>();
    public List<SpriteRenderer> exits = new List<SpriteRenderer>();
    static GridManager gridManager;
    Vector3 startPosition;
    [HideInInspector] public SpriteRenderer spriteRenderer;

    public Transform encounterParent;
    Color highlight = new Color(.5f, .5f, .5f, 1f);
    Color normal = new Color(1f, 1f, 1f, 1f);
    Tween colorTween;

    public static List<Vector3Int> encounterPositions = new List<Vector3Int>  {

                                                                new Vector3Int(1, -1, 0),
                                                                new Vector3Int(1, 0, -1),
                                                                new Vector3Int(0, 1, -1),
                                                                new Vector3Int(-1, 1, 0),
                                                                new Vector3Int(-1, 0, 1),
                                                                new Vector3Int(0, -1, 1),

                                                                new Vector3Int(2, -1, -1),
                                                                new Vector3Int(1, 1, -2),
                                                                new Vector3Int(-1, 2, -1),
                                                                new Vector3Int(-2, 1, 1),
                                                                new Vector3Int(-1, -1, 2),
                                                                new Vector3Int(1, -2, 1),

                                                                new Vector3Int(0, 0, 0)

                                                            };

    public static float encounterRadius = 1.0f;
    public static float encounterNoiseAllowed = 0.1f;
    public static float zRadFactor = Mathf.Sqrt(3/2);

    public List<EncounterHex> encounters;
    public List<EncounterHex> encountersExits;
    

    public TileState tileState
    {
        get
        {
            return _tileState;
        }
        set 
        {
            _tileState = value;
            colorTween?.Kill();

            if (_tileState == TileState.Active)
            {
                gridManager.activeTiles.Add(this);
            }
            else if (_tileState == TileState.Completed)
            {
                gridManager.completedTiles.Add(this);
                spriteRenderer.color = Color.gray;
            }
            else if (_tileState == TileState.Current)
            {
                colorTween = spriteRenderer.DOColor(highlight, 1f).SetEase(Ease.InSine).SetLoops(-1, LoopType.Yoyo).OnKill(() => spriteRenderer.color = normal);
                gridManager.currentTile = this;
            }
        }
    }
    public void Init()
    {
        gridManager = WorldSystem.instance.gridManager;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Activate(bool activeDebug = true)
    {
        availableDirections = gridManager.AddNeighbours();
        gridManager.activeTiles.Add(this);
        if (activeDebug)
        {
            availableDirections.ForEach(x => exits[x].gameObject.SetActive(true));
        }
        spriteRenderer.sprite = gridManager.sprites[0];
    }

    public void CloseExits(List<int> openExists)
    {
        // close all exists not connecting to the placed hex
        List<int> closedDirections = availableDirections.Except(lockedDirections).Except(openExists).ToList();
        availableDirections = availableDirections.Except(closedDirections).ToList();

        // debug to display exists that are no longer available
        closedDirections.ForEach(x => exits[x].GetComponent<SpriteRenderer>().color = Color.red);
    }

    public void LockDirections(List<int> directions = null)
    {
        if (directions == null)
        {
            lockedDirections = availableDirections;
            return;
        }
        lockedDirections = directions;
    }

    public float BeginFlipUpNewTile(bool enterPlacement = false)
    {
        GetComponent<PolygonCollider2D>().enabled = false; //ska bort n�r man ska l�gga ut sj�lv
        spriteRenderer.color = Color.white;
        if (enterPlacement)
        {
            gridManager.animator.SetBool("IsPlacing", true);
        }

        tileState = TileState.Animation;
        LeanTween.rotateAround(gameObject, new Vector3(0,1,0), 270.0f, 0.5f).setEaseInCubic().setOnComplete(() => EndFlipUpNewTile());
        return 1f;
    }

    public void EndFlipUpNewTile()
    {
        Activate(false);
        WorldSystem.instance.encounterManager.GenerateHexEncounters(this);
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
            () => CompleteFlip()
        );
    }

    void CompleteFlip()
    {
        if (gridManager.gridState != GridState.Creating)
        {
            tileState = TileState.Placement;
            StartPlacement();
        }
        else
        {
            tileState = TileState.Active;
        }
        //WorldSystem.instance.encounterManager.GenerateHexEncounters(this);
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
    }
    void Highlight()
    {
        spriteRenderer.color = highlight;
    }
    void UnHighlight()
    {
        spriteRenderer.color = normal;
    }

    void OnMouseUp()
    {
        if(tileState == TileState.Inactive && gridManager.gridState == GridState.Placement && gridManager.TileConnectedToExit(this))
            BeginFlipUpNewTile(true);
        else if(tileState == TileState.Inventory && gridManager.activeTile == null && gridManager.gridState != GridState.Complete )
            StartPlacement();
        else if(gridManager.gridState == GridState.Play && gridManager.hexMapController.zoomStep != 0 && tileState != TileState.Inactive)
        {
            gridManager.hexMapController.FocusTile(this);
        }
    }
    void OnMouseEnter()
    {
        if (gridManager.gridState == GridState.Placement && tileState == TileState.Inactive && gridManager.hexMapController.zoomStep != 2)
        {
            if (gridManager.activeTile == null && gridManager.CheckFreeSlot(this))
                spriteRenderer.color = Color.green;
            else
                spriteRenderer.color = Color.red;
        }
        else if (gridManager.gridState == GridState.Play)
        {
            Highlight();
        }
    }

    void OnMouseOver()
    {
        if (tileState == TileState.Inactive && gridManager.hoverTile != this)
        {
            gridManager.hoverTilePosition = transform.position;
            gridManager.hoverTile = this;
            if (gridManager.activeTile != null && gridManager.oldHoverTile == null)
                gridManager.activeTile.coord = coord;
        }
    }

    void OnMouseExit()
    {
        if (_tileState == TileState.Inactive)
            spriteRenderer.color = Color.white;
        UnHighlight();
        gridManager.hoverTilePosition = Vector3.zero;
        gridManager.hoverTile = null;
    }

    #region Encounters 


    public static Vector3 EncounterPosToLocalCoord(Vector3Int encPos)
    {
        float x = (encPos.x * 0.5f) - (encPos.y * 0.5f);
        float y = zRadFactor*encPos.z * -1;

        return encounterRadius*(new Vector3(x, y));
    }

    public static Vector3Int DirectionToDoorEncounter(Vector3Int dir)
    {
        return encounterPositions[encounterPositions.IndexOf(dir) + 6];
    }

    #endregion
}

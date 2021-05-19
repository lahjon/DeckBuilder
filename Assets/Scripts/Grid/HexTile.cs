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
    public Vector3Int entryPosition;
    Vector3 startPosition;
    [HideInInspector] public SpriteRenderer spriteRenderer;

    public Transform encounterParent;
    Color highlightColor = new Color(.6f, .6f, .6f, 1f);
    Color completedColor = new Color(.4f, .4f, .4f, 1f);
    Color normalColor = new Color(.8f, .8f, .8f, 1f);
    Tween colorTween;
    public bool highlighted;

    public Dictionary<Vector3Int, EncounterHex> posToEncounter = new Dictionary<Vector3Int, EncounterHex>();
    public Dictionary<Vector3Int, EncounterHex> posToEncountersExit = new Dictionary<Vector3Int, EncounterHex>();
    public static List<Vector3Int> positionsExit = new List<Vector3Int>()
    {
        new Vector3Int(1, -2, 1),
        new Vector3Int(2, -1, -1),
        new Vector3Int(1, 1, -2),
        new Vector3Int(-1, 2, -1),
        new Vector3Int(-2, 1, 1),
        new Vector3Int(-1, -1, 2)
    };
    public static List<Vector3Int> positionsInner = new List<Vector3Int>();

    public static float radiusInverse = 0.90f;
    public static float encounterNoiseAllowed = 0.1f;
    public static float zRadFactor = Mathf.Sqrt(3 / 2);
    private int gridWidth = 2;

    public List<EncounterHex> encounters;
    public List<EncounterHex> encountersExits;
    public bool specialTile;


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
                spriteRenderer.color = completedColor;
            }
            else if (_tileState == TileState.InactiveHighlight)
            {
                spriteRenderer.color = normalColor;
                StartFadeInOutColor();
            }
            else if (_tileState == TileState.Special)
            {
                spriteRenderer.sprite = gridManager.sprites[2];
            }
            else if (_tileState == TileState.Current)
            {
                spriteRenderer.color = normalColor;
                StartFadeInOutColor();
                gridManager.currentTile = this;
            }
        }
    }

    public void StartFadeInOutColor()
    {
        Debug.Log("START");
        colorTween?.Kill();
        spriteRenderer.color = normalColor;
        colorTween = spriteRenderer.DOColor(highlightColor, 1f).SetEase(Ease.InSine).SetLoops(-1, LoopType.Yoyo).OnKill(() => spriteRenderer.color = normalColor);
    }
    public void StopFadeInOutColor()
    {
        Debug.Log("STOP");
        colorTween?.Kill();
        if (tileState == TileState.Completed)
        {
            spriteRenderer.color = completedColor;
        }
        else
        {
            spriteRenderer.color = normalColor;
        }
    }
    public void Init()
    {
        gridManager = WorldSystem.instance.gridManager;
        spriteRenderer = GetComponent<SpriteRenderer>();
        EncountersInitializePositions(posToEncounter, gridWidth);
    }

    public void Activate(bool activeDebug = true)
    {
        availableDirections = gridManager.AddNeighbours();
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
        if (enterPlacement)
        {
            Activate(false);
            entryPosition = gridManager.currentTile.coord;
            //gridManager.currentTile = null;
            gridManager.animator.SetBool("IsPlacing", true);
            gridManager.hexMapController.FocusTile(this, ZoomState.Mid, true);
        }
        else
        {
            spriteRenderer.sprite = gridManager.sprites[2];
        }



        tileState = TileState.Animation;
        LeanTween.rotateAround(gameObject, new Vector3(0, 1, 0), 270.0f, 0.5f).setEaseInCubic().setOnComplete(() => EndFlipUpNewTile());
        return 1f;
    }

    public void EndFlipUpNewTile()
    {
        List<int> requiredExits = gridManager.GetNewExits(this);

        foreach (int dir in requiredExits)
        {
            if (!availableDirections.Contains(dir))
            {
                availableDirections[requiredExits.IndexOf(dir)] = dir;
            }
        }
        availableDirections.ForEach(x => exits[x].gameObject.SetActive(true));

        LeanTween.rotateAround(gameObject, new Vector3(0, 1, 0), 90.0f, 0.5f).setEaseOutCubic().setOnComplete(
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
        WorldSystem.instance.encounterManager.GenerateHexEncounters(this);
    }


    void StartPlacement()
    {
        startPosition = transform.position;
        spriteRenderer.sortingOrder += 1;
        //GetComponent<PolygonCollider2D>().enabled = false;
        gridManager.activeTile = this;
        //coord = gridManager.hoverTile.coord;
        //gridManager.InPlacement(this);
    }
    public void CheckPlacement()
    {
        // GetComponent<PolygonCollider2D>().enabled = true;
        // if (gridManager.TileConnectedToExit(this))
        // {
        //     EndPlacement();
        // }
        // if (gridManager.TilePlacementValidStart(this))
        // {
        //     gridManager.oldHoverTile = gridManager.hoverTile;
        //     coord = gridManager.hoverTile.coord;
        //     gridManager.InRotation();
        // }
        // else
        // {

        // }
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
        if (!highlighted)
        {
            Debug.Log("Highlight");
            spriteRenderer.color += new Color(0.2f, 0.2f, 0.2f, 1);
            highlighted = true;
        }
    }
    void UnHighlight()
    {
        if (highlighted)
        {
            Debug.Log("Unhighlight");
            spriteRenderer.color -= new Color(0.2f, 0.2f, 0.2f, 1);
            highlighted = false;
        }
    }

    void OnMouseUp()
    {
        if (tileState == TileState.InactiveHighlight && gridManager.gridState == GridState.Placement)
            BeginFlipUpNewTile(true);
        // else if(gridManager.activeTile == null && gridManager.gridState != GridState.Complete )
        //     StartPlacement();
        else if (gridManager.gridState == GridState.Play && gridManager.hexMapController.zoomStep != 0 && tileState != TileState.Inactive)
        {
            gridManager.hexMapController.FocusTile(this, ZoomState.Mid);
        }
    }
    // void OnMouseOver()
    // {
    //     if (tileState == TileState.Inactive && gridManager.hoverTile != this)
    //     {
    //         gridManager.hoverTilePosition = transform.position;
    //         gridManager.hoverTile = this;
    //     }
    // }
    void OnMouseEnter()
    {
        if (tileState == TileState.Current || tileState == TileState.InactiveHighlight)
        {
            spriteRenderer.color = normalColor;
            Highlight();
            StartFadeInOutColor();
        }
        else
        {
            Highlight();
        }
    }


    void OnMouseExit()
    {
        if (tileState == TileState.Current || tileState == TileState.InactiveHighlight)
        {
            spriteRenderer.color = normalColor;
            UnHighlight();
            StartFadeInOutColor();
        }
        else
        {
            UnHighlight();
        }
        gridManager.hoverTilePosition = Vector3.zero;
        gridManager.hoverTile = null;

    }

    public static void EncountersInitializePositions(Dictionary<Vector3Int, EncounterHex> encPos, int gridWidth)
    {
        if (positionsInner.Count != 0) return;
        for (int q = -gridWidth; q <= gridWidth; q++)
        {
            int r1 = Mathf.Max(-gridWidth, -q - gridWidth);
            int r2 = Mathf.Min(gridWidth, -q + gridWidth);

            for (int r = r1; r <= r2; r++)
            {
                Vector3Int coords = new Vector3Int(q, r, -q - r);
                if (!positionsExit.Contains(coords))
                    positionsInner.Add(coords);
            }
        }

    }


    #region Encounters 

    public void SetCurrentEncounter(Vector3Int pos)
    {
        EncounterHex selected = encounters.Where(x => x.coordinates == pos).FirstOrDefault();
        if(selected == null)
            Debug.LogError("Tried to set entry node that doesnt exist!!" + pos);
        else
        {
            StartCoroutine(selected.Entering(() => { }));
            gridManager.hexMapController.FocusTile(this, ZoomState.Inner);
        }

        GetComponent<PolygonCollider2D>().enabled = false;
    }
    

    public static Vector3 EncounterPosToLocalCoord(Vector3Int encPos)
    {
        float x = (encPos.x * 0.5f) - (encPos.y * 0.5f);
        float y = zRadFactor * encPos.z * -1;

        return radiusInverse * (new Vector3(x, y));
    }

    public static Vector3Int DirectionToDoorEncounter(int dir)
    {
        return positionsExit[dir];
    }

    public void AddEncounter(Vector3Int pos, EncounterHex enc, bool exit = false)
    {
        if (exit)
        {
            posToEncountersExit[pos] = enc;
            encountersExits.Add(enc);
        }

        posToEncounter[pos] = enc;
        encounters.Add(enc);

    }
    #endregion
}
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
    static HexMapController hexMapController;
    public Vector3Int entryPosition;
    Vector3 startPosition;
    [HideInInspector] public SpriteRenderer spriteRenderer;

    public Transform encounterParent;
    public Transform roadParent;
    Color highlightColor = new Color(1f, 1f, 1f, 1f);
    Color completedColor = new Color(.7f, .7f, .7f, 1f);
    Color inactiveColor = new Color(.4f, .4f, .4f, 1f);
    Color normalColor = new Color(.8f, .8f, .8f, 1f);
    Tween colorTween;
    bool _highlighted;
    public TileEncounterType tileEncounterType;
    public TileBiome tileBiome;

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
    public static float zRadFactor = Mathf.Sqrt(3/2);
    private int gridWidth = 2;
    Color tempColor;

    public List<EncounterHex> encounters;
    public List<EncounterHex> encountersExits;

    public bool highlighted
    {
        get => _highlighted;
        set
        {
            _highlighted = value;
            if (value == true)
            {
                colorTween?.Kill();
                spriteRenderer.color = highlightColor;
            }
            else
            {
                tileState = _tileState; // trigger setter
            }
        }
    }

    

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
                Debug.Log("Not used atm");
            }
            else if (_tileState == TileState.Completed)
            {
                gridManager.completedTiles.Add(this);
                spriteRenderer.color = completedColor;
            }
            else if (_tileState == TileState.InactiveHighlight)
            {
                StartFadeInOutColor();
            }
            else if (_tileState == TileState.Inactive)
            {
                spriteRenderer.color = inactiveColor;
            }
            else if (_tileState == TileState.Special)
            {
                gridManager.specialTiles.Add(this);
                SetSpecialImage();
                spriteRenderer.color = completedColor;
            }
            else if (_tileState == TileState.Current)
            {
                SetCurrentTile();
            }
        }
    }

    public void SetCurrentTile()
    {
        gridManager.currentTile = this;
        if (hexMapController.zoomStep != 0)
        {
            StartFadeInOutColor();
        }
        else
        {
            StopFadeInOutColor();
        }
    }

    public void StartFadeInOutColor()
    {
        colorTween?.Kill();
        float t = Helpers.InverseLerp(inactiveColor.r, highlightColor.r, spriteRenderer.color.r);
        colorTween = spriteRenderer.DOColor(inactiveColor, t).SetEase(Ease.InSine).OnComplete(() => {

            spriteRenderer.color = inactiveColor;
            colorTween = spriteRenderer.DOColor(highlightColor, 1f).SetEase(Ease.InSine).SetLoops(-1, LoopType.Yoyo).OnKill(() => tempColor = spriteRenderer.color);
        });
    }

    public IEnumerator AnimateVisible()
    {
        float timer = 0.1f * Helpers.timeMultiplier * .5f;
        for (int i = 0; i < encounterParent.childCount; i++)
        {
            // turn off nodes
            Transform node = encounterParent.GetChild(i);
            node.gameObject.SetActive(false);
        }
        for (int i = 0; i < roadParent.childCount; i++)
        {
            // turn off nodes
            Transform node = roadParent.GetChild(i);
            node.gameObject.SetActive(false);

            for (int j = 0; j < node.childCount; j++)
            {
                // turn off road parents
                Transform roadParent = node.GetChild(j);
                roadParent.gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < encounterParent.childCount; i++)
        {
            // turn on nodes
            Transform node = encounterParent.GetChild(i);
            node.gameObject.SetActive(true);
            yield return new WaitForSeconds(timer);
        }
        for (int i = 0; i < roadParent.childCount; i++)
        {
            // turn on nodes
            Transform node = roadParent.GetChild(i);
            node.gameObject.SetActive(true);

            for (int j = 0; j < node.childCount; j++)
            {
                // turn on road parents
                Transform roadParent = node.GetChild(j);
                roadParent.gameObject.SetActive(true);
                yield return new WaitForSeconds(timer * 0.5f);
            }
        }
    }

    public void StopFadeInOutColor()
    {
        colorTween?.Kill();

        spriteRenderer.color = normalColor;
    }

    public void Init()
    {
        gridManager = WorldSystem.instance.gridManager;
        hexMapController = gridManager.GetComponent<HexMapController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        EncountersInitializePositions(posToEncounter, gridWidth);
    }

    public void Activate(bool activeDebug = true)
    {
        if (coord == Vector3.zero)
        {
            if(Random.Range(0,2) == 0)
                availableDirections = new List<int>{0,2,4};
            else
                availableDirections = new List<int>{1,3,5};
        }
        else
        {
            availableDirections = gridManager.AddNeighbours();
        }
        if (activeDebug)
        {
            availableDirections.ForEach(x => exits[x].gameObject.SetActive(true));
        }

        spriteRenderer.sprite = gridManager.activeTilesSprite[0];
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
        //GetComponent<PolygonCollider2D>().enabled = false; //ska bort n�r man ska l�gga ut sj�lv
        spriteRenderer.color = Color.white;
        if (enterPlacement)
        {
            Activate(false);
            Debug.Log(transform.localScale);
            gridManager.animator.SetBool("IsPlacing", true);
            hexMapController.FocusTile(this, ZoomState.Mid, true);
            if (gridManager.currentTile != null)
            {
                
            }

            tileState = TileState.Animation;

            List<int> requiredExits = gridManager.GetNewExits(this);
            foreach (int dir in requiredExits)
            {
                if(!availableDirections.Contains(dir))
                {
                    availableDirections[requiredExits.IndexOf(dir)] = dir;
                }
            }
            WorldSystem.instance.encounterManager.GenerateHexEncounters(this, new List<Vector3Int>() { Vector3Int.zero});
            encounterParent.gameObject.SetActive(false);
            roadParent.gameObject.SetActive(false);
        }
        else
        {
            SetSpecialImage();
        }

        LeanTween.rotateAround(gameObject, new Vector3(0,1,0), 270.0f, 0.5f).setEaseInCubic().setOnComplete(() => EndFlipUpNewTile(enterPlacement));
        return 1f;
    }

    public void SetSpecialImage()
    {
        spriteRenderer.sprite = gridManager.inactiveTilesSprite[(int)tileBiome];
    }

    public void EndFlipUpNewTile(bool enterPlacement)
    {
        encounterParent.gameObject.SetActive(true);
        roadParent.gameObject.SetActive(true);
        availableDirections.ForEach(x => exits[x].gameObject.SetActive(true));
        if (enterPlacement)
        {
            spriteRenderer.sprite = gridManager.activeTilesSprite[(int)tileBiome];
        }
        
        LeanTween.rotateAround(gameObject, new Vector3(0,1,0), 90.0f, 0.5f).setEaseOutCubic().setOnComplete(
            () => CompleteFlip()
        );
    }

    void CompleteFlip()
    {
        if (gridManager.initialized)
        {
            tileState = TileState.Placement;
            StartPlacement();
        }
        encountersExits[0].encounterType = OverworldEncounterType.Start;
        StartCoroutine(encountersExits[0].Entering(() => { }));

        //WorldSystem.instance.encounterManager.GenerateHexEncounters(this);
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

    public void OffsetRotation(bool instant = false)
    {

            for (int i = 0; i < encounterParent.childCount; i++)
            {
                if (instant)
                    encounterParent.GetChild(i).transform.rotation = Quaternion.identity;
                else
                    encounterParent.GetChild(i).transform.DORotate(Vector3.zero, 0.5f).SetEase(Ease.InExpo);
            }

    }

    public void EndPlacement()
    {
        transform.position = startPosition;
        gridManager.ExitPlacement();
    }

    public void ConfirmTilePlacement()
    {
        Debug.Log("Confirm");
        gridManager.tiles[gridManager.activeTile.coord] = gridManager.activeTile;
        transform.position = gridManager.CellPosToWorldPos(coord);
        Destroy(gridManager.oldHoverTile.gameObject);
        gridManager.oldHoverTile = null;

    }
    void Highlight()
    {
        if(!highlighted)
        {
            highlighted = true;
        }
    }
    void UnHighlight()
    {
        if(highlighted)
        {
            highlighted = false;
        }
    }

    void OnMouseUp()
    {
        if(tileState == TileState.InactiveHighlight && gridManager.gridState == GridState.Placement && hexMapController.zoomStep != 0)
            BeginFlipUpNewTile(true);
        else if(gridManager.gridState == GridState.Play && hexMapController.zoomStep != 0 && tileState != TileState.Inactive)
        {
            hexMapController.FocusTile(this, ZoomState.Inner);
        }
    }
    void OnMouseEnter()
    {
        if (!hexMapController.disableInput && hexMapController.zoomStep != 0)
        {
            Highlight();
        }
    }
    void OnMouseOver()
    {
        // if (!gridManager.hexMapController.disableInput)
        // {
        //     Highlight();
        // }
    }

    void OnMouseExit()
    {

        UnHighlight();
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
                if(!positionsExit.Contains(coords))
                    positionsInner.Add(coords);
            }
        }

    }

    #region Encounters 


    public static Vector3 EncounterPosToLocalCoord(Vector3Int encPos)
    {
        float x = (encPos.x * 0.5f) - (encPos.y * 0.5f);
        float y = zRadFactor*encPos.z * -1;

        return radiusInverse*(new Vector3(x, y));
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

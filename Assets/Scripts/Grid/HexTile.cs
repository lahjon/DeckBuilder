using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class HexTile : MonoBehaviour
{
    [SerializeField] TileState _tileState; 
    public Vector3Int coord;
    public List<int> availableDirections = new List<int>();
    public List<int> lockedDirections = new List<int>();
    public List<SpriteRenderer> exits = new List<SpriteRenderer>();
    static GridManager gridManager;
    static HexMapController hexMapController;
    public int entryDir;
    bool _specialTile;
    public int turnCompleted;
    Vector3 startPosition;
    [HideInInspector] public SpriteRenderer spriteRenderer;

    public Transform encounterParent;
    public Transform roadParent;
    static Color highlightColorPrimary = new Color(1f, 1f, 1f, 1f);
    static Color highlightColorSecondary = new Color(.7f, .7f, .7f, 1f);
    static Color completedColor = new Color(.7f, .7f, .7f, 1f);
    static Color inactiveColor = new Color(.4f, .4f, .4f, 1f);
    static Color normalColor = new Color(.8f, .8f, .8f, 1f);
    Tween colorTween;
    bool _highlightedPrimary;
    bool _highlightedSecondary;
    public TileEncounterType tileEncounterType;
    public TileBiome tileBiome;
    public bool completed;

    public Dictionary<Vector3Int, Encounter> posToEncounter = new Dictionary<Vector3Int, Encounter>();
    public Dictionary<Vector3Int, Encounter> posToEncountersExit = new Dictionary<Vector3Int, Encounter>();
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

    public List<Encounter> encounters;
    public Encounter encounterEntry;
    public List<Encounter> encountersExits = new List<Encounter>();

    public bool highlightedPrimary
    {
        get => _highlightedPrimary;
        set
        {
            _highlightedPrimary = value;
            if (value == true)
            {
                colorTween?.Kill();
                spriteRenderer.color = highlightColorPrimary;
            }
            else
            {
                tileState = _tileState; // trigger setter
            }
        }
    }

    public bool highlightedSecondary
    {
        get => _highlightedSecondary;
        set
        {
            _highlightedSecondary = value;
            if (value == true)
            {
                colorTween?.Kill();
                spriteRenderer.color = highlightColorSecondary;
            }
            else
            {
                tileState = _tileState; // trigger setter
            }
        }
    }

    public bool specialTile
    {
        get
        {
            return _specialTile;
        }
        set 
        {
            _specialTile = value;
            gridManager.specialTiles.Add(this);
            SetSpecialImage();
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
                if (!completed)
                {
                    gridManager.completedTiles.Add(this);
                    completed = true;
                }
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
        if (spriteRenderer.color != inactiveColor)
        {
            spriteRenderer.color = highlightColorSecondary;
        }
        float t = Helpers.InverseLerp(inactiveColor.r, highlightColorPrimary.r, spriteRenderer.color.r);
        colorTween = spriteRenderer.DOColor(inactiveColor, t).SetEase(Ease.InSine).OnComplete(() => {

            spriteRenderer.color = inactiveColor;
            colorTween = spriteRenderer.DOColor(highlightColorPrimary, 1f).SetEase(Ease.InSine).SetLoops(-1, LoopType.Yoyo).OnKill(() => tempColor = spriteRenderer.color);
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

    // public void CloseExits(List<int> openExists)
    // {
    //     // close all exists not connecting to the placed hex
    //     List<int> closedDirections = availableDirections.Except(lockedDirections).Except(openExists).ToList();
    //     availableDirections = availableDirections.Except(closedDirections).ToList();

    //     // debug to display exists that are no longer available
    //     closedDirections.ForEach(x => exits[x].GetComponent<SpriteRenderer>().color = Color.red);
    // }

    public void CloseExits(int dir)
    {
        // close all exists not connecting to the placed hex
        availableDirections = new List<int>{dir};
        List<int> closedDirections = availableDirections.Except(lockedDirections).ToList();

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

    public float BeginFlipUpNewTile(bool enterPlacement = false, bool emptyTile = false)
    {
        spriteRenderer.color = Color.white;
        if (enterPlacement)
        {
            Activate(false);
            if(!emptyTile)
            {
                gridManager.animator.SetBool("IsPlacing", true);
                hexMapController.FocusTile(this, ZoomState.Inner, true);
                entryDir = gridManager.GetEntry(this).Item1;
            }
            tileState = TileState.Animation;

            List<int> requiredExits = new List<int>();
            if (gridManager.bossStarted || emptyTile)
            {
                requiredExits = gridManager.GetNewExits(this);
                availableDirections = requiredExits;
                availableDirections.Add((requiredExits[0] + 2 + 6) % 6);
                availableDirections.Add((requiredExits[0] + 4 + 6) % 6);
                WorldSystem.instance.encounterManager.GenerateBossHexEncounter(this);
            }
            else
            {
                requiredExits = gridManager.GetNewExits(this);
                foreach (int dir in requiredExits)
                {
                    if(!availableDirections.Contains(dir))
                    {
                        availableDirections[requiredExits.IndexOf(dir)] = dir;
                    }
                }
                WorldSystem.instance.encounterManager.GenerateHexEncounters(this, new List<Vector3Int>() { Vector3Int.zero});
            }

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
        spriteRenderer.color = completedColor;
    }

    public void EndFlipUpNewTile(bool enterPlacement)
    {
        encounterParent.gameObject.SetActive(true);
        roadParent.gameObject.SetActive(true);

        if (gridManager.bossStarted) gridManager.UpdateIcons();
        if (enterPlacement)
        {
            spriteRenderer.sprite = gridManager.activeTilesSprite[(int)tileBiome];
            gridManager.currentTurn++;
            turnCompleted = gridManager.currentTurn;
            hexMapController.disableZoom = false;
            encountersExits.ForEach(x => x.UpdateEntry());
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

        //WorldSystem.instance.encounterManager.GenerateHexEncounters(this);
    }


    void StartPlacement()
    {
        startPosition = transform.position;
        //spriteRenderer.sortingOrder += 1;
        gridManager.activeTile = this;
    }

    public void OffsetRotation(bool instant = false)
    {
        for (int i = 0; i < encounterParent.childCount; i++)
        {
            if (instant)
                encounterParent.GetChild(i).transform.rotation = Quaternion.identity;
            else
                encounterParent.GetChild(i).transform.DORotate(Vector3.zero, 0.45f, RotateMode.Fast).SetEase(Ease.InExpo);
        }
    }

    public void MatchRotation()
    {
        // have to have this function instead of OnComplete in OffsetRotation because it causes unknown errors
        for (int i = 0; i < encounterParent.childCount; i++)
        {
            encounterParent.GetChild(i).transform.rotation = Quaternion.identity;
        }
    }

    public void EndPlacement()
    {
        gridManager.ExitPlacement();
        hexMapController.disableZoom = true;
        if (encounterEntry == null)
        {
            Debug.LogError("No valid Entry!");
            return;
        }
        StartCoroutine(encounterEntry.Entering());
    }

    public void CloseExists()
    {
        List<int> dirs = new List<int>();

        Debug.Log("amount of exits:" + encountersExits.Count);
        for (int i = 0; i < encountersExits.Count; i++)
        {
            if (encountersExits[i].status == EncounterHexStatus.Visited)
            {
                dirs.Add(encountersExits[i].ExitDirection());
                //Debug.Log("enc is visited: " + encountersExits.ElementAt(i).Value);
            }
        }

        availableDirections = dirs.Union(lockedDirections).ToList();
    }
    void Highlight()
    {
        if(!highlightedPrimary)
        {
            highlightedPrimary = true;
            if (this.tileState == TileState.InactiveHighlight)
            {
                foreach (HexTile tile in gridManager.highlightedTiles)
                {
                    if (tile != this)
                    {
                        tile.highlightedSecondary = true;
                    }
                }
            }
        }
    }
    void UnHighlight()
    {
        if(highlightedPrimary)
        {
            highlightedPrimary = false;
            if (this.tileState == TileState.InactiveHighlight)
            {
                foreach (HexTile tile in gridManager.highlightedTiles)
                {
                    if (tile != this)
                    {
                        tile.highlightedSecondary = false;
                    }
                }
            }
        }
    }

    void OnMouseUp()
    {
        //Debug.Log(entryDir);
        //encountersExits.ForEach(x => Debug.Log(x));
        //availableDirections.ForEach(x => Debug.Log(x));
        if(tileState == TileState.InactiveHighlight && gridManager.gridState == GridState.Placement)
            BeginFlipUpNewTile(true);
        else if(gridManager.gridState == GridState.Play && hexMapController.zoomStep != 0 && tileState != TileState.Inactive)
        {
            hexMapController.FocusTile(this, ZoomState.Inner);
        }
    }
    void OnMouseEnter()
    {
        if (!hexMapController.disablePanning && hexMapController.zoomStep != 0)
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
        
    }

    public static void EncountersInitializePositions(Dictionary<Vector3Int, Encounter> encPos, int gridWidth)
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

    public void AddEncounter(Vector3Int pos, Encounter enc, bool exit = false)
    {
        if (exit)
        {
            posToEncountersExit[pos] = enc;
            Debug.Log("Adding Exit: " + enc);
            encountersExits.Add(enc);
        }

        posToEncounter[pos] = enc;
        encounters.Add(enc);

    }
    #endregion
}

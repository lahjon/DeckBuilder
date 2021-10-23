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
    static ScenarioMapManager gridManager;
    static HexMapController hexMapController;
    public List<HexTile> neighbours = new List<HexTile>();
    public int entryDir;
    bool _specialTile;
    public int turnCompleted;
    Vector3 startPosition;
    [HideInInspector] public SpriteRenderer spriteRenderer;

    public SpriteRenderer storyMark;
    public SpriteRenderer undiscoveredSpriteRenderer;

    public Transform encounterParent;
    public Transform roadParent;
    static Color highlightColorPrimary = new Color(1f, 1f, 1f, 1f);
    static Color highlightColorSecondary = new Color(.6f, .6f, .6f, 1f);
    static Color completedColor = new Color(.8f, .8f, .8f, 1f);
    static Color inactiveColor = new Color(.8f, .8f, .8f, 1f);
    static Color normalColor = new Color(1f, 1f, 1f, 1f);
    Tween colorTween;
    bool _highlightedPrimary;
    bool _highlightedSecondary;
    public TileEncounterType tileEncounterType;
    public TileBiome tileBiome;
    public TileType type;

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

    public static List<Vector3Int> positionsFixedTile = new List<Vector3Int>()
    {
        new Vector3Int(1, -1, 0),
        new Vector3Int(1, -0, -1),
        new Vector3Int(0, 1, -1),
        new Vector3Int(-1, 1, 0),
        new Vector3Int(-1, 0, 1),
        new Vector3Int(0, -1, 1)
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

    public string storyId = "";
    public EncounterData storyEncounter;

    public bool highlightedPrimary
    {
        get => _highlightedPrimary;
        set
        {
            _highlightedPrimary = value;
            if (value == true)
            {
                colorTween?.Kill();
                undiscoveredSpriteRenderer.color = highlightColorPrimary;
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
                undiscoveredSpriteRenderer.color = highlightColorSecondary;
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
                if (!gridManager.completedTiles.Contains(this))
                {
                    gridManager.completedTiles.Add(this);
                    EventManager.TileCompleted(this);
                }

                spriteRenderer.color = completedColor;
            }
            else if (_tileState == TileState.InactiveHighlight)
            {
                StartFadeInOutColor();
            }
            else if (_tileState == TileState.Inactive)
            {
                undiscoveredSpriteRenderer.color = inactiveColor;
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
        if (undiscoveredSpriteRenderer.color != inactiveColor)
        {
            undiscoveredSpriteRenderer.color = highlightColorSecondary;
        }
        float t = Helpers.InverseLerp(inactiveColor.r, highlightColorPrimary.r, undiscoveredSpriteRenderer.color.r);
        colorTween = undiscoveredSpriteRenderer.DOColor(inactiveColor, t).SetEase(Ease.InSine).OnComplete(() => {

            undiscoveredSpriteRenderer.color = inactiveColor;
            colorTween = undiscoveredSpriteRenderer.DOColor(highlightColorPrimary, 1f).SetEase(Ease.InSine).SetLoops(-1, LoopType.Yoyo).OnKill(() => tempColor = undiscoveredSpriteRenderer.color);
        });
    }

    public IEnumerator AnimateVisible()
    {
        float timer = 0.1f * Helpers.timeMultiplier * .5f;
        ContentVisible(true);
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
        EncountersInitializePositions(gridWidth);
        if (coord == Vector3.zero)
        {
            if(Random.Range(0,2) == 0)
                availableDirections = new List<int>{0,2,4};
            else
                availableDirections = new List<int>{1,3,5};
        }
        else
        {
            availableDirections = gridManager.AddNeighbours(4,5);
        }

        //spriteRenderer.sprite = gridManager.activeTilesSprite[0];
    }

    public void AddNeighbours()
    {
        neighbours = gridManager.GetNeighbours(this);
    }

    public void CloseExits(int dir)
    {
        // close all exists not connecting to the placed hex
        availableDirections = new List<int>{dir};
        List<int> closedDirections = availableDirections.Except(lockedDirections).ToList();

        // debug to display exists that are no longer available
        closedDirections.ForEach(x => exits[x].GetComponent<SpriteRenderer>().color = Color.red);
    }

    public void ContentVisible(bool visibility)
    {
        encounterParent.gameObject.SetActive(visibility);
        roadParent.gameObject.SetActive(visibility);
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

    void DeleteAllContent()
    {
        for (int i = 0; i < encounterParent.childCount; i++)
        {
            Destroy(encounterParent.GetChild(i).gameObject);
        }
        for (int i = 0; i < roadParent.childCount; i++)
        {
            Destroy(roadParent.GetChild(i).gameObject);
        }
        encounterEntry = null;
        encountersExits.Clear();
        encounters.Clear();
        availableDirections.Clear();
    }

    public void RevealTile(bool fromPlacement = false)
    {
        undiscoveredSpriteRenderer.material = gridManager.undiscoveredMaterial;
        undiscoveredSpriteRenderer.material.DOFloat(0f, "Dissolve", 1f).OnComplete(() =>  {
            undiscoveredSpriteRenderer.material.SetFloat("Dissolve", 1);
            undiscoveredSpriteRenderer.material = gridManager.discoveredMaterial;
            undiscoveredSpriteRenderer.gameObject.SetActive(false);
            if (fromPlacement)
                EnterPlacement();
            // else
            // {
            //     entryDir = gridManager.GetEntry(this).Item1;
            //     WorldSystem.instance.encounterManager.GenerateFirstHexEncounters(this);
            //     encountersExits.ForEach(x => x.RotationUpdateEntry());
            // }
        });
    }

    public void EnterPlacement()
    {
        RotateTileExits();
        ContentVisible(true);
        spriteRenderer.color = Color.white;
        storyMark.gameObject.SetActive(false);
        spriteRenderer.color = Color.white;
        gridManager.animator.SetBool("IsPlacing", true);
        hexMapController.FocusTile(this, ZoomState.Inner, true);
        entryDir = gridManager.GetEntry(this).Item1;
        tileState = TileState.Animation;

        WorldSystem.instance.encounterManager.GenerateHexEncounters(this, storyEncounter);
        gridManager.currentTurn++;
        turnCompleted = gridManager.currentTurn;
        hexMapController.disableZoom = false;
        tileState = TileState.Placement;
        startPosition = transform.position;
        gridManager.activeTile = this;
        gridManager.animator.SetBool("IsRotating", true);
        encountersExits.ForEach(x => x.RotationUpdateEntry());
        //StartCoroutine(AnimateVisible());
    }

    public void FlipEmptyTile()
    {


        spriteRenderer.color = Color.white;
            
        WorldSystem.instance.encounterManager.GenerateFirstHexEncounters(this);
        posToEncounter[Vector3Int.zero].encounterType = OverworldEncounterType.Cave;

        encounters.ForEach(x => x.status = EncounterHexStatus.Visited);

        LeanTween.rotateAround(gameObject, new Vector3(0,1,0), 270.0f, 0.4f).setEaseInCubic().setOnComplete(() => 
        {
            // mid flip
            ContentVisible(true);
            //spriteRenderer.sprite = gridManager.activeTilesSprite[(int)tileBiome];

            LeanTween.rotateAround(gameObject, new Vector3(0,1,0), 90.0f, 0.2f).setEaseOutCubic().setOnComplete(() => 
            {
                //end flip
                gridManager.CompleteEmptyTile(this);
                gridManager.HighlightEntries();

            }
        );
        });
    }

    public void RotateTileExits()
    {
        if (gridManager.rotateCounter > 6)
        {
            Debug.Log("Shouldnt Happen");
            gridManager.rotateCounter = 0;
            return;
        }

        for (int i = 0; i < availableDirections.Count; i++)
            availableDirections[i] = (availableDirections[i] + 6) % 6;

        for (int i = 0; i < encountersExits.Count; i++)
            encountersExits[i].coordinates = HexTile.positionsExit[(encountersExits[i].ExitDirection() + 6) % 6];

        if (!gridManager.TilePlacementValid(this))
        {
            RotateTileExits();
            gridManager.rotateCounter++;
        }
    }

    public void RotateTile(bool clockwise, bool instant = false)
    {
        if (gridManager.rotateCounter > 6)
        {
            Debug.Log("Shouldnt Happen");
            gridManager.rotateCounter = 0;
            return;
        }

        if (encounterEntry != null)
            encounterEntry.CancelAnimation();

        int sign = clockwise ? 1 : -1;
        for (int i = 0; i < availableDirections.Count; i++)
            availableDirections[i] = (availableDirections[i] + sign + 6) % 6;

        for (int i = 0; i < encountersExits.Count; i++)
            encountersExits[i].coordinates = HexTile.positionsExit[(encountersExits[i].ExitDirection() + sign + 6) % 6];

        gridManager.rotationAmount += sign*60;

        if (!gridManager.TilePlacementValid(this))
        {
            RotateTile(clockwise, instant);
            gridManager.rotateCounter++;
        }
        else
        {
            if (instant)
            {
                transform.Rotate(new Vector3(0, 0, transform.localRotation.eulerAngles.z + gridManager.rotationAmount));
                gridManager.rotationAmount = 0;
                gridManager.rotateCounter = 0;
                MatchRotation();
            }
            else
            {
                gridManager.buttonRotateConfirm.interactable = false;
                gridManager.buttonRotateLeft.interactable = false;
                gridManager.buttonRotateRight.interactable = false;
                OffsetRotation();
                ResetRoadsEncounters();
                float timer = 0.5f;
                transform.DORotate(new Vector3(0, 0, transform.localRotation.eulerAngles.z + gridManager.rotationAmount), timer, RotateMode.FastBeyond360).SetEase(Ease.InExpo).OnComplete(() => {
                    gridManager.buttonRotateLeft.interactable = true;
                    gridManager.buttonRotateRight.interactable = true;
                    gridManager.buttonRotateConfirm.interactable = true;
                    gridManager.rotationAmount = 0;
                    gridManager.rotateCounter = 0;
                    encountersExits.ForEach(x => x.RotationUpdateEntry());
                    encountersExits.Where(x => x.encounterType == OverworldEncounterType.Start).FirstOrDefault().HighlightReachable();
                    MatchRotation();
                });
            }
        }
    }

    public void ResetRoadsEncounters()
    {
        for(int i = 0; i < encounters.Count; i++)
        {
            encounters[i].status = EncounterHexStatus.Idle;
            for (int j = 0; j < encounters[i].roads.Count; j++)
                encounters[i].roads[j].status = EncounterRoadStatus.Idle;
        }
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
            RevealTile(true);
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

    public static void EncountersInitializePositions(int gridWidth)
    {
        for (int q = -gridWidth; q <= gridWidth; q++)
        {
            int r1 = Mathf.Max(-gridWidth, -q - gridWidth);
            int r2 = Mathf.Min(gridWidth, -q + gridWidth);

            for (int r = r1; r <= r2; r++)
            {
                Vector3Int coords = new Vector3Int(q, r, -q - r);
                if(!positionsExit.Contains(coords) && !positionsInner.Contains(coords))
                    positionsInner.Add(coords);
            }
        }

    } 

    public void SetStoryInfo(string ID, Color color, EncounterData storyEncounter = null)
    {
        storyMark.gameObject.SetActive(true);
        storyMark.color = color;
        storyId = ID;
        this.storyEncounter = storyEncounter;
    }

    public void RemoveStoryInfo()
    {
        storyId = string.Empty;
        storyEncounter = null;
        storyMark.gameObject.SetActive(false);
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
            encountersExits.Add(enc);
        }

        posToEncounter[pos] = enc;
        encounters.Add(enc);

    }
    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class HexTile : MonoBehaviour
{
    public int id;
    [SerializeField] TileState _tileState; 
    public Vector3Int coord;
    public List<GridDirection> availableDirections = new List<GridDirection>();
    public static ScenarioMapManager mapManager;
    static HexMapController hexMapController;
    public List<HexTile> neighbours = new List<HexTile>();
    bool _specialTile;
    [HideInInspector] public SpriteRenderer spriteRenderer;

    public GridDirection directionEntry; 

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
    public TileBiome tileBiome;
    public TileType type;

    public bool encountersSelectable = false;

    public Dictionary<Vector3Int, Encounter> posToEncounter = new Dictionary<Vector3Int, Encounter>();
    public Dictionary<Vector3Int, Encounter> posToEncounterExit = new Dictionary<Vector3Int, Encounter>();
    public Dictionary<Encounter, GridDirection> exitEncToDirection = new Dictionary<Encounter, GridDirection>();

    public static List<Vector3Int> positionsExit = new List<Vector3Int>()
    {
        GridDirection.SouthEast + GridDirection.East,
        GridDirection.East + GridDirection.NorthEast,
        GridDirection.NorthEast + GridDirection.NorthWest,
        GridDirection.NorthWest + GridDirection.West,
        GridDirection.West + GridDirection.SouthWest,
        GridDirection.SouthWest + GridDirection.SouthEast
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


            if (_tileState == TileState.Completed)
            {
                if (!mapManager.completedTiles.Contains(this))
                {
                    mapManager.completedTiles.Add(this);
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
        mapManager.currentTile = this;
        if (hexMapController.zoomStep != 0)
            StartFadeInOutColor();
        else
            StopFadeInOutColor();
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
        mapManager = WorldSystem.instance.gridManager;
        hexMapController = mapManager.GetComponent<HexMapController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        EncountersInitializePositions(gridWidth);
        if (coord == Vector3.zero)
        {
            int modder = Random.Range(0, 1);
            for(int i = 0 + modder; i < 6; i = i + 2) 
                availableDirections.Add(new GridDirection(i));
        }
        else
        {
            List<int> pool = new List<int>() { 0, 1, 2, 3, 4, 5 };
            int nrExits = Random.Range(4,6);
            for (int i = 0; i < nrExits; i++)
            {
                int x = pool[Random.Range(0, pool.Count)];
                pool.Remove(x);
                availableDirections.Add(new GridDirection(x));
            }
        }
    }

    public void ContentVisible(bool visibility)
    {
        encounterParent.gameObject.SetActive(visibility);
        roadParent.gameObject.SetActive(visibility);
    }

    public void RevealTile(bool fromPlacement = false)
    {
        tileState = TileState.Animation;
        undiscoveredSpriteRenderer.material = mapManager.undiscoveredMaterial;
        if (fromPlacement) 
        {
            hexMapController.FocusTile(this, ZoomState.Inner);
            hexMapController.enableInput = false;
        }
        undiscoveredSpriteRenderer.material.DOFloat(0f, "Dissolve", 1f).OnComplete(() =>  {
            undiscoveredSpriteRenderer.material.SetFloat("Dissolve", 1);
            undiscoveredSpriteRenderer.material = mapManager.discoveredMaterial;
            undiscoveredSpriteRenderer.gameObject.SetActive(false);
            if (fromPlacement)
                EnterPlacement();
            else
                tileState = TileState.Completed;
        });
    }

    public void EnterPlacement()
    {
        ContentVisible(true);
        storyMark.gameObject.SetActive(false);
        spriteRenderer.color = Color.white;
        mapManager.animator.SetBool("IsPlacing", true);

        hexMapController.FocusTile(this, ZoomState.Inner);

        WorldSystem.instance.encounterManager.GenerateHexEncounters(this, storyEncounter);

        tileState = TileState.Placement;
        mapManager.currentTile = this;
        mapManager.choosableTiles.Remove(this);

        RotateTile(true,true);
        encountersExits.ForEach(x => x.MarkEntryEncounter());
        //StartCoroutine(AnimateVisible());
    }

    public void RotateTile(bool clockwise, bool instant = false)
    {
        if (mapManager.rotateCounter++ > 6)
        {
            Debug.LogWarning("Shouldnt Happen");
            mapManager.rotateCounter = 0;
            return;
        }

        if (encounterEntry != null)
            encounterEntry.CancelAnimation();

        int sign = clockwise ? 1 : -1;
        for (int i = 0; i < availableDirections.Count; i++)
            availableDirections[i] = availableDirections[i].Turned(sign);
        foreach (Encounter enc in encountersExits)
            exitEncToDirection[enc] = exitEncToDirection[enc].Turned(sign);

        mapManager.rotationAmount += sign*60;

        if (!mapManager.TilePlacementValid(this))
            RotateTile(clockwise, instant);
        else
        {
            mapManager.rotateCounter = 0;
            if (instant)
            {
                Debug.Log("Startin instant");
                transform.Rotate(new Vector3(0, 0, transform.localRotation.eulerAngles.z + mapManager.rotationAmount));
                FinishRotation();
            }
            else
            {
                mapManager.SetButtonsInteractable(false);
                OffsetRotation();
                float timer = 0.5f;
                transform.DORotate(new Vector3(0, 0, transform.localRotation.eulerAngles.z + mapManager.rotationAmount), timer, RotateMode.FastBeyond360).SetEase(Ease.InExpo).OnComplete(() => {
                    mapManager.SetButtonsInteractable(true);
                    FinishRotation();
                });
            }
        }
    }

    private void FinishRotation()
    {
        ResetRoadsEncounters();
        mapManager.rotationAmount = 0;
        encountersExits.ForEach(x => x.MarkEntryEncounter());
        encounterEntry.HighlightReachable(true);
        MatchRotation();
        encounterEntry.status = EncounterHexStatus.Selectable;
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
        for (int i = 0; i < encounterParent.childCount; i++)
            encounterParent.GetChild(i).transform.rotation = Quaternion.identity;
    }

    public void EndPlacement()
    {
        mapManager.ExitPlacement();
        hexMapController.enableInput = true;
        if (encounterEntry == null)
        {
            Debug.LogError("No valid Entry!");
            return;
        }
        StartCoroutine(encounterEntry.Entering());
    }

    void Highlight(bool turnOn, HexTile tileTargeted)
    {
        if (turnOn)
        {
            if (this == tileTargeted)
                highlightedPrimary = turnOn;
            else
                highlightedSecondary = turnOn;
        }
        else
        {
            highlightedPrimary = false;
            highlightedSecondary = false;
        }
    }

    void OnMouseUp()
    {
        if(tileState == TileState.InactiveHighlight && mapManager.gridState == GridState.Placing)
            RevealTile(true);
    }
    void OnMouseEnter()
    {
        if (!hexMapController.enableInput || hexMapController.zoomStep == 0) return;

        if (tileState == TileState.InactiveHighlight)
        {
            foreach (HexTile tile in mapManager.choosableTiles)
                tile.Highlight(true, this);
        }
        else
            Highlight(true,this);
    }

    void OnMouseExit()
    {
        if (tileState == TileState.InactiveHighlight)
        {
            foreach (HexTile tile in mapManager.choosableTiles)
                tile.Highlight(false, this);
        }
        else
            Highlight(false, this);
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

    public void AddEncounter(Encounter enc, bool exit = false)
    {
        if (exit)
        {
            posToEncounterExit[enc.coordinates] = enc;
            encountersExits.Add(enc);
            exitEncToDirection[enc] = new GridDirection(positionsExit.IndexOf(enc.coordinates));
        }

        posToEncounter[enc.coordinates] = enc;
        encounters.Add(enc);

    }

    public void AddNeighboors()
    {
        foreach (GridDirection dir in GridDirection.Directions)
            if (mapManager.GetTile(coord + dir) is HexTile neigh)
                neighbours.Add(neigh);
    }
    #endregion
}

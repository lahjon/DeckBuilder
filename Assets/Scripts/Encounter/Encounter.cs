using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Animations;



public class Encounter : MonoBehaviour
{
    public HexTile tile;
    public List<Encounter> neighboors;
    public List<EncounterRoad> roads = new List<EncounterRoad>();
    public ScenarioEncounterType _encounterType;

    public Vector3Int coordinates;
    public Vector3 startingScale;

    public static Color highlightColor = new Color(.5f, .5f, .5f, 1f);
    SpriteRenderer spriteRenderer;

    private Tween tweenAction1;
    private Tween tweenAction2;

    public string storyID;
    public EncounterData encData;

    public EncounterHexStatus _status = EncounterHexStatus.Idle;
    bool _highlighted;
    public ScenarioEncounterType encounterType
    {
        get
        {
            return _encounterType;
        }
        set
        {
            _encounterType = value;
            spriteRenderer.sprite = DatabaseSystem.instance.GetOverWorldIcon(value);
        }
    }

    public void SetEncounterType(ScenarioEncounterType type)
    {
        _encounterType = type;
        spriteRenderer = GetComponent<SpriteRenderer>();
        List<Sprite> allSprites = GameObject.Find("DatabaseSystem").GetComponent<DatabaseSystem>().allOverworldIcons;
        Debug.Log(allSprites);
        Sprite sprite = allSprites.Where(x => x.name == string.Format("Overworld{0}", type.ToString())).FirstOrDefault();
        if (sprite == null)
            sprite = allSprites.FirstOrDefault(x => x.name == "OverworldPlain");
        spriteRenderer.sprite = sprite;
    }
    public bool highlighted
    {
        get
        {
            return _highlighted;
        }
        set
        {
            _highlighted = value;
            CancelAnimation();
            if (_highlighted)
            {
                if (_status == EncounterHexStatus.Selectable)
                    AnimateEncounterHighlight();
                else
                    transform.localScale = startingScale * 1.35f;
            }
            else
            {
                if (_status == EncounterHexStatus.Selectable)
                    AnimateEncounter();
                else
                    transform.localScale = startingScale;
            }
        }
    }
    public EncounterHexStatus status
    {
        get
        {
            return _status;
        }
        set
        {
            _status = value;
            CancelAnimation();

            if (_status == EncounterHexStatus.Selectable)
            {
                spriteRenderer.color = Color.white;
                AnimateEncounter();
            }
            else if (_status == EncounterHexStatus.Visited)
                spriteRenderer.color = new Color(.6f, .6f, .6f);
            else if (_status == EncounterHexStatus.Unreachable)
            {
                spriteRenderer.color = new Color(.8f, .8f, .8f);
                foreach (EncounterRoad road in roads)
                    if(road.status == EncounterRoadStatus.Idle)
                        road.status = EncounterRoadStatus.Unreachable;
            }
            else
                spriteRenderer.color = Color.white;
        }
    }

    public void Init()
    {
        startingScale = transform.localScale;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void AnimateEncounter()
    {
        //Debug.Log("Start animation " + this);
        transform.localScale *= 1.3f;
        tweenAction1 = transform.DOScale(startingScale * .9f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).OnKill(() => transform.localScale = startingScale);
        tweenAction2 = spriteRenderer.DOColor(highlightColor, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).OnKill(() => spriteRenderer.color = Color.white);
    }

    public void AnimateEncounterHighlight()
    {
        //Debug.Log("Start animation " + this);
        transform.localScale *= 1.5f;
        tweenAction1 = transform.DOScale(startingScale * .9f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).OnKill(() => transform.localScale = startingScale);
        tweenAction2 = spriteRenderer.DOColor(highlightColor, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).OnKill(() => spriteRenderer.color = Color.white);
    }

    public void CancelAnimation()
    {
        tweenAction1?.Kill();
        tweenAction2?.Kill();
    }

    public IEnumerator Entering()
    {
        transform.localScale = startingScale*1.5f;
        status = EncounterHexStatus.Visited;
        Encounter previous = WorldSystem.instance.encounterManager.currentEncounter;

        if (encounterType == ScenarioEncounterType.Start)
        {
            HexTile prevTile = HexTile.mapManager.GetTile(tile.coord + tile.directionEntry);
            Encounter prevEnc = prevTile.encountersExits.Where(x => prevTile.exitEncToDirection[x].IsOpposing(tile.directionEntry)).FirstOrDefault();
            EncounterRoad intraHexRoad = WorldSystem.instance.encounterManager.AddRoad(prevEnc, this, true);
            yield return StartCoroutine(intraHexRoad.AnimateTraverseRoad(this));
        }
        else if (previous != null)
        {
            previous.SetLeaving();
            foreach (EncounterRoad road in roads)
                if ((road.fromEnc == this && road.toEnc == previous) || (road.fromEnc == previous && road.toEnc == this))
                    yield return StartCoroutine(road.AnimateTraverseRoad(this));
        }

        HighlightReachable();
        
        WorldSystem.instance.encounterManager.currentEncounter = this;

        if (!WorldSystem.instance.debugMode || encounterType == ScenarioEncounterType.Exit)
            StartEncounter();
    }

    public void HighlightReachable(bool silentCheck = false)
    {
        EncounterHexStatus origStatus = status;
        _status = EncounterHexStatus.Visited; // needed when overworld map is just testing rotations
        HashSet<Encounter> reachable = FindAllReachableNodes(this);

        foreach (Encounter enc in tile.encounters)
        {
            if (enc == this) continue; 

            if (reachable.Contains(enc))
                enc.status = !silentCheck && neighboors.Contains(enc) ? EncounterHexStatus.Selectable : EncounterHexStatus.Idle;
            else if (enc.status != EncounterHexStatus.Unreachable)
                enc.status = EncounterHexStatus.Unreachable;
        }

        foreach (Encounter enc in reachable)
        {
            foreach (EncounterRoad road in enc.roads)
            {
                if (road.status == EncounterRoadStatus.Traversed) continue;
                Encounter otherEnc = road.OtherEnd(enc);
                if (otherEnc.status == EncounterHexStatus.Visited && otherEnc != this)
                    road.status = EncounterRoadStatus.Unreachable;
            }
        }

        _status = origStatus;
    }

    public static HashSet<Encounter> FindAllReachableNodes(Encounter enc)
    {
        HashSet<Encounter> hs = new HashSet<Encounter>();

        if (enc.encounterType == ScenarioEncounterType.Exit)
        {
            hs.Add(enc);
            return hs;
        }

        List<Encounter> neighs = enc.neighboors.Where(e => e.status == EncounterHexStatus.Idle || e.status == EncounterHexStatus.Selectable).ToList();
        foreach (Encounter neigh in neighs)
        {
            //avoiding animationtriggers
            EncounterHexStatus originalStatus = neigh.status;
            neigh._status = EncounterHexStatus.Visited;
            hs.UnionWith(FindAllReachableNodes(neigh));
            neigh._status = originalStatus;
        }

        if (hs.Any()) hs.Add(enc);
        return hs;
    }


    public void MarkEntryEncounter()
    {
        if (tile.exitEncToDirection[this].Equals(tile.directionEntry))
        {
            tile.encounterEntry = this;
            encounterType = ScenarioEncounterType.Start;
        }
        else
            encounterType = ScenarioEncounterType.Exit;
    }


    public void SetLeaving()
    {
        transform.localScale = startingScale;
        foreach (Encounter e in neighboors)
        {
            e.neighboors.Remove(this);
            if (e.status == EncounterHexStatus.Selectable) e.status = EncounterHexStatus.Idle;
        }
        neighboors.Clear();
    }

    private void OnMouseDown()
    {
        if (tile.tileState != TileState.Current || status != EncounterHexStatus.Selectable) return;
        //Debug.Log("starting click");
        StartCoroutine(Entering());
    }

    void OnMouseEnter()
    {
        highlighted = true;

    }

    void OnMouseExit()
    {
        highlighted = false;
    }


    public bool CheckConnection(Encounter target)
    {
        if (this == target) return true;
        List<Encounter> visited = new List<Encounter>();
        List<Encounter> frontier = new List<Encounter>(neighboors);

        while(frontier.Count != 0)
        {
            List<Encounter> newFrontier = new List<Encounter>();
            foreach(Encounter enc in frontier)
            {
                if (enc == target) return true;
                visited.Add(enc);
                newFrontier.AddRange(enc.neighboors.Except(visited));
            }

            frontier = newFrontier;
        }

        return false;
    }

    public void SetStoryEncounter(EncounterData enc, string ID)
    {
        encData = enc;
        storyID = ID;
        encounterType = ScenarioEncounterType.Story;
    }

    public void RemoveStoryEncounter()
    {
        storyID = null;
        encData = null;
        encounterType = ScenarioEncounterType.Choice;
    }

    public void StartEncounter()
    {
        if (!string.IsNullOrEmpty(storyID))
        {
            if(encData is EncounterDataCombat encComb)
            {
                CombatSystem.instance.encounterData = encComb;
                WorldStateSystem.SetInCombat(true);
                return;
            }
            else if(encData is EncounterDataRandomEvent encEvent)
            {
                WorldSystem.instance.uiManager.encounterUI.encounterData = encEvent;
                WorldStateSystem.SetInChoice(true);
            }
        }

        switch (encounterType)
        {
            case ScenarioEncounterType.CombatNormal:
            case ScenarioEncounterType.CombatElite:
            case ScenarioEncounterType.CombatBoss:
                encData = CombatSystem.instance.encounterData = WorldSystem.instance.scenarioMapManager.GetRndEncounterCombat(encounterType);
                WorldStateSystem.SetInCombat(true);
                break;
            case ScenarioEncounterType.Shop:
                WorldStateSystem.SetInOverworldShop(true);
                break;
            case ScenarioEncounterType.Choice:
                encData = WorldSystem.instance.uiManager.encounterUI.encounterData = WorldSystem.instance.scenarioMapManager.GetRndEncounterChoice();
                WorldStateSystem.SetInChoice(true);
                break;
            case ScenarioEncounterType.Blacksmith:
                WorldStateSystem.SetInBlacksmith(true);
                break;
            case ScenarioEncounterType.Exit:
                WorldSystem.instance.scenarioMapManager.CompleteCurrentTile();
                break;
            case ScenarioEncounterType.Bonfire:
                WorldStateSystem.SetInBonfire(true);
                break;
            default:
                Debug.LogWarning(string.Format("Implement a case for: {0}", encounterType.ToString()));
                break;
        }

        WorldSystem.instance.scenarioMapManager.finishedEncounterToReport = this;
    }


}

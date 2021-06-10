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
    public OverworldEncounterType _encounterType;

    public Vector3Int coordinates;
    public Vector3 startingScale;

    public static Color highlightColor = new Color(.5f, .5f, .5f, 1f);
    SpriteRenderer spriteRenderer;

    private Tween tweenAction1;
    private Tween tweenAction2;


    public EncounterHexStatus _status = EncounterHexStatus.Idle;
    bool _highlighted;
    public OverworldEncounterType encounterType
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
    public bool highlighted
    {
        get
        {
            return _highlighted;
        }
        set
        {
            _highlighted = value;
            if (_highlighted)
            {
                CancelAnimation();
                if (_status == EncounterHexStatus.Selectable)
                    AnimateEncounterHighlight();
                else
                    transform.localScale = startingScale * 1.35f;
            }
            else
            {
                CancelAnimation();
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
            tweenAction1?.Kill();
            tweenAction2?.Kill();

            if (_status == EncounterHexStatus.Selectable)
                AnimateEncounter();
            if(_status == EncounterHexStatus.Visited)
                spriteRenderer.color = new Color32(50, 50, 50,255);
            else if(_status == EncounterHexStatus.Unreachable)
                spriteRenderer.color = new Color32(200, 200, 200, 255);
            else
                spriteRenderer.color = new Color32(255, 255, 255, 255);
        }
    }

    public void Awake()
    {
        startingScale = transform.localScale;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void AnimateEncounter()
    {
        //tweenAction = DOTween.To(() => transform.localScale, x => transform.localScale = x, startingScale * 1.2f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        transform.localScale *= 1.3f;
        tweenAction1 = transform.DOScale(startingScale * .9f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).OnKill(() => transform.localScale = startingScale);
        //tweenAction2 = spriteRenderer.DOColor(highlightColor, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).OnKill(() => spriteRenderer.color = new Color(.8f, .8f, .8f, 1f));
        tweenAction2 = spriteRenderer.DOColor(highlightColor, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).OnKill(() => spriteRenderer.color = Color.white);
    }

    public void AnimateEncounterHighlight()
    {
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

        tile.encounters.Remove(this);

        Debug.Log(encounterType);
        Debug.Log(WorldSystem.instance.gridManager.GetEntry(tile).Item2);
        if (encounterType == OverworldEncounterType.Start && WorldSystem.instance.gridManager.GetEntry(tile).Item2 is Encounter encEntry)
        {
            EncounterRoad intraHexRoad = WorldSystem.instance.encounterManager.AddRoad(encEntry, this, true);
            yield return StartCoroutine(intraHexRoad.AnimateTraverseRoad(this));
        }

        Encounter previous = WorldSystem.instance.encounterManager.currentEncounterHex;
        if (previous != null)
        {
            previous.SetLeaving();
            foreach (EncounterRoad road in roads)
                if ((road.fromEnc == this && road.toEnc == previous) || (road.fromEnc == previous && road.toEnc == this))
                    yield return StartCoroutine(road.AnimateTraverseRoad(this));
                
        }
        

        HashSet<Encounter> reachable = WorldSystem.instance.encounterManager.FindAllReachableNodes(this);

        foreach (Encounter enc in tile.encounters)
        {
            if (reachable.Contains(enc))
                enc.status = neighboors.Contains(enc) ? EncounterHexStatus.Selectable : EncounterHexStatus.Idle;
            else if (enc.status == EncounterHexStatus.Idle)
            {
                enc.status = EncounterHexStatus.Unreachable;
                foreach (EncounterRoad road in enc.roads)
                    road.status = EncounterRoadStatus.Unreachable;
            }
        }

        foreach(Encounter enc in reachable)
        {
            foreach (EncounterRoad road in enc.roads)
            {
                Encounter otherEnc = road.OtherEnd(enc);
                if( otherEnc.status == EncounterHexStatus.Visited 
                    && otherEnc != this
                    && road.status != EncounterRoadStatus.Traversed) {
                    road.status = EncounterRoadStatus.Unreachable;
                }
            }
        }
        
        WorldSystem.instance.encounterManager.currentEncounterHex = this;

        if (!WorldSystem.instance.debugMode || encounterType == OverworldEncounterType.Exit)
            encounterType.Invoke();
    }

    public int ExitDirection()
    {
        return HexTile.positionsExit.IndexOf(coordinates);
    }

    public void UpdateEntry()
    {
        //Debug.Log(HexTile.positionsExit[tile.encountersExits[this]]);
        //Debug.Log(tile.entryDir);
        if (tile.encountersExits.IndexOf(this) >= 0 && tile.entryDir == ExitDirection())
        {
            AnimateEncounter();
            tile.encounterEntry = this;
            encounterType = OverworldEncounterType.Start;
        }
        else
        {
            encounterType = OverworldEncounterType.Exit;
        }

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
        if (status != EncounterHexStatus.Selectable) return;
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
}

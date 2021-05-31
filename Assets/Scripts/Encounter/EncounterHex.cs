using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Animations;

public class EncounterHex : Encounter
{
    public List<EncounterHex> hexNeighboors;
    public Vector3Int coordinates;
    public Vector3 startingScale;
    private Tween tweenAction1;
    private Tween tweenAction2;
    public HexTile tile;
    public static Color highlightColor = new Color(.5f, .5f, .5f, 1f);
    SpriteRenderer spriteRenderer;

    public EncounterHexStatus _status = EncounterHexStatus.Idle;
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

        //RotationConstraint rotCon = GetComponent<RotationConstraint>();
        
        //rotCon.AddSource(WorldSystem.instance.gridManager.gameObject);
    }

    public void AnimateEncounter()
    {
        //tweenAction = DOTween.To(() => transform.localScale, x => transform.localScale = x, startingScale * 1.2f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        transform.localScale *= 1.3f;
        tweenAction1 = transform.DOScale(startingScale * .9f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).OnKill(() => transform.localScale = startingScale);
        tweenAction2 = spriteRenderer.DOColor(highlightColor, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).OnKill(() => spriteRenderer.color = Color.white);
    }

    public void CancelAnimation()
    {
        tweenAction1.Kill();
        tweenAction2.Kill();
    }

    public override IEnumerator Entering(System.Action VisitAction)
    {
        transform.localScale = startingScale*1.5f;
        status = EncounterHexStatus.Visited;

        tile.encounters.Remove(this);
        //tile.encountersExits.Remove(this);

        if (WorldSystem.instance.encounterManager.currentEncounterHex != null && encounterType == OverworldEncounterType.Start && WorldSystem.instance.gridManager.GetEntry(tile).Item2 is EncounterHex encEntry)
            WorldSystem.instance.encounterManager.DrawRoad(encEntry, this, true);

        WorldSystem.instance.encounterManager.currentEncounterHex?.SetLeaving(this);

        HashSet<EncounterHex> reachable = WorldSystem.instance.encounterManager.FindAllReachableNodes(this);

        foreach (EncounterHex enc in tile.encounters)
        {
            if (reachable.Contains(enc))
                enc.status = hexNeighboors.Contains(enc) ? EncounterHexStatus.Selectable : EncounterHexStatus.Idle;
            else if (enc.status == EncounterHexStatus.Idle)
                enc.status = EncounterHexStatus.Unreachable;
        }

        
        WorldSystem.instance.encounterManager.currentEncounterHex = this;

        encounterType.Invoke();

        yield return null;



        VisitAction();
    }

    public void UpdateEntry()
    {
        Debug.Log(HexTile.positionsExit[tile.encountersExits[this]]);
        Debug.Log(tile.entryDir);
        if (tile.entryDir == tile.encountersExits[this])
        {
            AnimateEncounter();
            tile.encounterEntry = (this, tile.entryDir);
            encounterType = OverworldEncounterType.Start;
        }
        else
        {
            encounterType = OverworldEncounterType.Exit;
        }

    }


    public override void SetLeaving(Encounter nextEnc)
    {
        transform.localScale = startingScale;
        foreach (EncounterHex e in hexNeighboors)
        {
            e.hexNeighboors.Remove(this);
            if (e.status == EncounterHexStatus.Selectable) e.status = EncounterHexStatus.Idle;
        }
        hexNeighboors.Clear();
    }

    private void OnMouseDown()
    {
        if (status != EncounterHexStatus.Selectable) return;
        //Debug.Log("starting click");
        StartCoroutine(Entering(() => { } ));
    }

    private void OnMouseEnter()
    {
        //Debug.Log("Collider works");
    }

    public bool CheckConnection(EncounterHex target)
    {
        if (this == target) return true;
        List<EncounterHex> visited = new List<EncounterHex>();
        List<EncounterHex> frontier = new List<EncounterHex>(hexNeighboors);

        while(frontier.Count != 0)
        {
            List<EncounterHex> newFrontier = new List<EncounterHex>();
            foreach(EncounterHex enc in frontier)
            {
                if (enc == target) return true;
                visited.Add(enc);
                newFrontier.AddRange(enc.hexNeighboors.Except(visited));
            }

            frontier = newFrontier;
        }

        return false;
    }
}

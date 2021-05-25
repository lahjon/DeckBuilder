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
    private Tween tweenAction;
    public HexTile tile;

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
            tweenAction.Kill();

            if (_status == EncounterHexStatus.Selectable)
                tweenAction = DOTween.To(() => transform.localScale, x => transform.localScale = x, startingScale * 1.2f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
                
            if(_status == EncounterHexStatus.Visited)
                GetComponent<SpriteRenderer>().color = new Color32(0, 0, 0,255);
            else if(_status == EncounterHexStatus.Unreachable)
                GetComponent<SpriteRenderer>().color = new Color32(200, 200, 200, 255);
            else
                GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
        }
    }

    public void Awake()
    {
        startingScale = transform.localScale;
        RotationConstraint rotCon = GetComponent<RotationConstraint>();
        
        //rotCon.AddSource(WorldSystem.instance.gridManager.gameObject);
    }

    public override IEnumerator Entering(System.Action VisitAction, Encounter fromEncounter = null)
    {
        transform.localScale = startingScale*1.5f;
        status = EncounterHexStatus.Visited;

        tile.encounters.Remove(this);
        tile.encountersExits.Remove(this);

        WorldSystem.instance.encounterManager.currentEncounterHex?.SetLeaving(this);

        HashSet<EncounterHex> reachable = WorldSystem.instance.encounterManager.FindAllReachableNodes(this);

        foreach (EncounterHex enc in tile.encounters)
        {
            if (reachable.Contains(enc))
                enc.status = hexNeighboors.Contains(enc) ? EncounterHexStatus.Selectable : EncounterHexStatus.Idle;
            else if (enc.status == EncounterHexStatus.Idle)
                enc.status = EncounterHexStatus.Unreachable;
        }

        /*
        foreach (EncounterHex e in hexNeighboors)
        {
            bool canReachExit = WorldSystem.instance.encounterManager.CanReachExitNode(e, tile.encountersExits);
            e.status = canReachExit ? EncounterHexStatus.Selectable : EncounterHexStatus.Unreachable;
        }
        */

        WorldSystem.instance.encounterManager.currentEncounterHex = this;

        yield return null;
        VisitAction();
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterRoad : MonoBehaviour
{
    public GameObject roadSegmentTemplate;
    private EncounterRoadStatus _status = EncounterRoadStatus.Idle;
    public EncounterRoadStatus status { get { return _status; } set{
            _status = value;
            if (value == EncounterRoadStatus.Traversed)
                roadSprites.ForEach(x => x.color = Color.white);
            else if (value == EncounterRoadStatus.Unreachable)
                roadSprites.ForEach(x => x.color = Color.gray);
            else
                roadSprites.ForEach(x => x.color = Color.white);
        } 
    }

    static float width = 0.1f;
    static float gap = 0.05f;
    static float break_gap = 0.05f;

    public Encounter fromEnc;
    public Encounter toEnc;

    List<SpriteRenderer> roadSprites = new List<SpriteRenderer>();

    public IEnumerator AnimateTraverseRoad(Encounter hex)
    {
        if (hex != toEnc) roadSprites.Reverse();
        foreach (SpriteRenderer r in roadSprites)
        {
            r.color = Color.white;
            yield return new WaitForSeconds(0.10f);
        }
        _status = EncounterRoadStatus.Traversed;
        yield return null;
    }

    public void DrawRoad(Encounter fromEnc, Encounter toEnc, bool animate = false)
    {
        this.fromEnc = fromEnc;
        this.toEnc = toEnc;
        Vector3 from = fromEnc.transform.position;
        Vector3 to = toEnc.transform.position;
        Vector3 dir = Vector3.Normalize(to - from);
        float dist = Vector3.Distance(from, to);
        float dist_t = break_gap + width;

        this.name = string.Format("road_{0}_to_{1}", fromEnc.gameObject.name, toEnc.gameObject.name);
   
        int count = 0;

        while (dist_t < dist - break_gap)
        {
            GameObject roadSegment = Instantiate(roadSegmentTemplate, this.transform);
            roadSegment.transform.localScale = roadSegment.transform.localScale * WorldSystem.instance.encounterManager.tileSizeInverse;
            roadSprites.Add(roadSegment.GetComponent<SpriteRenderer>());
            roadSegment.transform.position = from + dir * dist_t;
            roadSegment.transform.rotation = Quaternion.LookRotation(from, to);
            dist_t += gap + width;
            count++;
            if (animate) roadSegment.SetActive(false);
        }

        if (animate) StartCoroutine(AnimateRoad());
    }

    IEnumerator AnimateRoad()
    {
        WorldSystem.instance.gridManager.animator.SetBool("IsAnimating", true);
        for (int i = 0; i < roadSprites.Count; i++)
        {
            roadSprites[i].gameObject.SetActive(true);
            yield return new WaitForSeconds(.15f);
        }
        WorldSystem.instance.gridManager.animator.SetBool("IsAnimating", false);
    }

    public Encounter OtherEnd(Encounter OneEnd)
    {
        if (fromEnc != OneEnd && toEnc != OneEnd)
            return null;
        else
            return fromEnc == OneEnd ? toEnc : fromEnc;
    }

}

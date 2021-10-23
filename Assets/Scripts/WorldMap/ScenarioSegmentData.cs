using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ScenarioSegmentData
{
    public string SegmentName;
    public ScenarioSegmentType segmentType;
    public Color color = Color.magenta;

    public string description;

    [NonReorderable]
    public List<Vector3Int> gridCoordinates = new List<Vector3Int>();
    [NonReorderable]
    public List<EncounterData> encounters = new List<EncounterData>();
    [NonReorderable]
    public List<EncounterData> missEncounters = new List<EncounterData>();
    [NonReorderable]
    public List<string> requiredSegmentsOR = new List<string>();
    [NonReorderable]
    public List<string> requiredSegmentsAND = new List<string>();
    [NonReorderable]
    public List<string> cancelSegmentsOnStart = new List<string>();

    public int nrSkippableTiles;
    public int nrDecoys;
}

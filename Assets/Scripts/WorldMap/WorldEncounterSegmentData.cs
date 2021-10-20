using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldEncounterSegmentData
{
    public string ID;
    public WorldEncounterSegmentType segmentType;
    public Color color = Color.magenta;

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

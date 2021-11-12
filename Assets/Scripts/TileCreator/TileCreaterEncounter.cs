using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileCreatorEncounter
{
    public TileCreatorEncounter(Vector3 aPos, OverworldEncounterType aType, GridDirection aDir, int anIdx)
    {
        position = aPos;
        idx = anIdx;
        overworldEncounterType = aType;
        direction = aDir;
        neighbourIndex = new List<int>();
    }
    public Vector3 position;
    public GridDirection direction;
    public List<int> neighbourIndex;
    public int idx;
    public OverworldEncounterType overworldEncounterType;
}
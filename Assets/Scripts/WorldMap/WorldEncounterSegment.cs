using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
using System;

public class WorldEncounterSegment
{
    public WorldEncounter encounter;
    public WorldEncounterSegmentData data;

    public Condition condition;

    public WorldEncounterSegment(WorldEncounterSegmentData data)
    {
        this.data = data;
    }

    public IEnumerator SetupSegment()
    {
        switch (data.segmentType)
        {
            case WorldEncounterSegmentType.ClearTiles:
                SetupClearTiles();
                break;
            case WorldEncounterSegmentType.ClearEncounters:
                SetupClearEncounters();
                break;
            case WorldEncounterSegmentType.FindTheEncounters:
                SetupFindEncounters();
                break;
            default:
                Debug.LogError("No setup available for SegmentType " + data.segmentType);
                break;
        }

        yield return null;
    }

    private void SetupClearTiles()
    {
        foreach (Vector3Int vec in data.gridCoordinates)
        {
            HexTile tile = WorldSystem.instance.gridManager.GetTile(vec);
            tile.storyMark.gameObject.SetActive(true);
            tile.storyId = data.ID;
        }

        condition = new CountingCondition(new ConditionData() { type = ConditionType.StoryTileCompleted, strParameter = data.ID, numValue = data.gridCoordinates.Count }, null, () => Debug.Log("issa complete"));
        condition.Subscribe();
    }

    public void SetupClearEncounters()
    {
        for(int i = 0; i < data.gridCoordinates.Count; i++)
        {
            HexTile tile = WorldSystem.instance.gridManager.GetTile(data.gridCoordinates[i]);
            tile.storyMark.gameObject.SetActive(true);
            tile.storyId = data.ID;
            tile.storyEncounter = data.encounters[i];
        }

        condition = new CountingCondition(new ConditionData() { type = ConditionType.EncounterCompleted, strParameter = data.ID , numValue = data.encounters.Count}, null, () => Debug.Log("issa complete"));
        condition.Subscribe();
    }

    public void SetupFindEncounters()
    {
        List<Vector3Int> coords = new List<Vector3Int>(data.gridCoordinates);
        int misses = 0;

        for (int i = 0; i < data.gridCoordinates.Count; i++)
        {
            Vector3Int coord = coords[UnityEngine.Random.Range(0, coords.Count)];
            coords.Remove(coord);
            HexTile tile = WorldSystem.instance.gridManager.GetTile(coord);
            tile.storyMark.gameObject.SetActive(true);
            if (misses++ < data.nrDecoys)
            {
                tile.storyId = data.ID + "_miss";
                tile.storyEncounter = data.missEncounters[i % data.missEncounters.Count];
            }
            else
            {
                tile.storyId = data.ID;
                tile.storyEncounter = data.encounters[(i - data.nrDecoys) % data.encounters.Count];
            }
        }

        condition = new CountingCondition(new ConditionData() { type = ConditionType.EncounterCompleted, strParameter = data.ID, numValue = data.encounters.Count }, null, () => Debug.Log("issa complete"));
        condition.Subscribe();
    }




}

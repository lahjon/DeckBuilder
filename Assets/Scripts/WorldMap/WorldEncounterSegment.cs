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

    public ConditionCounting conditionClear;
    public ConditionCounting conditionStartAnd;
    public ConditionCounting conditionStartOr;

    public WorldEncounterSegment(WorldEncounterSegmentData data, WorldEncounter encounter)
    {
        this.encounter = encounter;
        this.data = data;
        SetupStartConditions();
    }

    public void SetupStartConditions()
    {
        if(data.requiredSegmentsAND.Count != 0)
        {
            conditionStartAnd = new ConditionCounting(
                new ConditionData() { type = ConditionType.StorySegmentCompleted, numValue = data.requiredSegmentsAND.Count, strParameters = data.requiredSegmentsAND },
                null,
                () => {
                    encounter.nextStorySegments.Add(this);
                    conditionStartAnd.Unsubscribe();
                    conditionStartOr?.Unsubscribe();
                    });
        }

        if (data.requiredSegmentsOR.Count != 0)
        {
            conditionStartOr = new ConditionCounting(
                new ConditionData() { type = ConditionType.StorySegmentCompleted, numValue = 1, strParameters = data.requiredSegmentsOR },
                null,
                () => {
                    encounter.nextStorySegments.Add(this);
                    conditionStartOr.Unsubscribe();
                    conditionStartAnd?.Unsubscribe();
                });
        }

        conditionStartOr?.Subscribe();
        conditionStartAnd?.Subscribe();
    }

    public IEnumerator SetupSegment()
    {
        CancelPrevious();

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
            WorldSystem.instance.gridManager.GetTile(vec).SetStoryInfo(data.ID, data.color);

        conditionClear = new ConditionCounting(
            new ConditionData() { type = ConditionType.StoryTileCompleted, strParameter = data.ID, numValue = data.gridCoordinates.Count-data.nrSkippableTiles }, 
            null,
            SegmentFinished);

        conditionClear.Subscribe();
    }

    public void SetupClearEncounters()
    {
        for(int i = 0; i < data.gridCoordinates.Count; i++)
            WorldSystem.instance.gridManager.GetTile(data.gridCoordinates[i]).SetStoryInfo(data.ID, data.color, data.encounters[i]);

        conditionClear = new ConditionCounting(
            new ConditionData() { type = ConditionType.EncounterCompleted, strParameter = data.ID , numValue = data.encounters.Count- data.nrSkippableTiles}, 
            null,
            SegmentFinished);

        conditionClear.Subscribe();
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
            if (misses++ < data.nrDecoys)
                tile.SetStoryInfo(data.ID, data.color, data.missEncounters[i % data.missEncounters.Count]);
            else
                tile.SetStoryInfo(data.ID, data.color, data.encounters[(i - data.nrDecoys) % data.encounters.Count]);
        }

        conditionClear = new ConditionCounting(
            new ConditionData() { type = ConditionType.EncounterCompleted, strParameter = data.ID, numValue = data.encounters.Count }, 
            null,
            SegmentFinished);

        conditionClear.Subscribe();
    }


    private void SegmentFinished()
    {
        Debug.Log("segment finishi");
        EventManager.StorySegmentCompleted(this);
        ClearRemnants();
        WorldSystem.instance.gridManager.StartCoroutine(encounter.SetupNextSegments());
    }

    private void ClearRemnants()
    {
        conditionClear.Unsubscribe();
        foreach (Vector3Int vec in data.gridCoordinates)
        {
            HexTile tile = WorldSystem.instance.gridManager.GetTile(vec);
            if (tile.tileState != TileState.Completed)
                tile.RemoveStoryInfo();
        }
    }

    private void CancelPrevious()
    {
        for (int i = 0; i < encounter.segments.Count; i++)
            if (data.cancelSegmentsOnStart.Contains(encounter.segments[i].data.ID))
                encounter.segments[i].ClearRemnants();
    }

}
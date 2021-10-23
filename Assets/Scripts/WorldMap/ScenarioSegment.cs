using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ScenarioSegment
{
    public Scenario scenario;
    public ScenarioSegmentData data;

    public ConditionCounting conditionClear;
    public ConditionCounting conditionStartAnd;
    public ConditionCounting conditionStartOr;

    private TMP_Text txt_description;

    public ScenarioSegment(ScenarioSegmentData data, Scenario encounter)
    {
        this.scenario = encounter;
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
                    scenario.nextStorySegments.Add(this);
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
                    scenario.nextStorySegments.Add(this);
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
            case ScenarioSegmentType.ClearTiles:
                SetupClearTiles();
                break;
            case ScenarioSegmentType.ClearEncounters:
                SetupClearEncounters();
                break;
            case ScenarioSegmentType.FindTheEncounters:
                SetupFindEncounters();
                break;
            default:
                Debug.LogError("No setup available for SegmentType " + data.segmentType);
                break;
        }

        txt_description = WorldSystem.instance.gridManager.objectiveDisplayer.GetDescriptionSlot();
        RefreshDescription();
        yield return null;
    }

    private void SetupClearTiles()
    {
        foreach (Vector3Int vec in data.gridCoordinates)
            WorldSystem.instance.gridManager.GetTile(vec).SetStoryInfo(data.SegmentName, data.color);

        conditionClear = new ConditionCounting(
            new ConditionData() { type = ConditionType.StoryTileCompleted, strParameter = data.SegmentName, numValue = data.gridCoordinates.Count-data.nrSkippableTiles }, 
            RefreshDescription,
            SegmentFinished);

        conditionClear.Subscribe();
    }

    public void SetupClearEncounters()
    {
        for(int i = 0; i < data.gridCoordinates.Count; i++)
            WorldSystem.instance.gridManager.GetTile(data.gridCoordinates[i]).SetStoryInfo(data.SegmentName, data.color, data.encounters[i]);

        conditionClear = new ConditionCounting(
            new ConditionData() { type = ConditionType.EncounterCompleted, strParameter = data.SegmentName , numValue = data.encounters.Count- data.nrSkippableTiles},
            RefreshDescription,
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
                tile.SetStoryInfo(data.SegmentName + "_miss", data.color, data.missEncounters[i % data.missEncounters.Count]);
            else
                tile.SetStoryInfo(data.SegmentName, data.color, data.encounters[(i - data.nrDecoys) % data.encounters.Count]);
        }

        conditionClear = new ConditionCounting(
            new ConditionData() { type = ConditionType.EncounterCompleted, strParameter = data.SegmentName, numValue = data.gridCoordinates.Count - data.nrDecoys },
            RefreshDescription,
            SegmentFinished);

        conditionClear.Subscribe();
    }


    private void SegmentFinished()
    {
        Debug.Log("segment finishi");
        EventManager.StorySegmentCompleted(this);
        ClearRemnants();
        WorldSystem.instance.gridManager.StartCoroutine(scenario.SetupNextSegments());
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
        WorldSystem.instance.gridManager.objectiveDisplayer.ReturnDescriptionSlot(txt_description);
    }

    private void CancelPrevious()
    {
        for (int i = 0; i < scenario.segments.Count; i++)
            if (data.cancelSegmentsOnStart.Contains(scenario.segments[i].data.SegmentName))
                scenario.segments[i].ClearRemnants();
    }

    private void RefreshDescription()
    {
        Debug.Log(ColorUtility.ToHtmlStringRGB(data.color).ToString());
        if (txt_description == null) return;
        txt_description.text = 
            data.description.Replace("<color>","<color=#" + ColorUtility.ToHtmlStringRGB(data.color) + ">")
            + (conditionClear.conditionData.numValue != 1 ? 
            "\n" + conditionClear.currentAmount + "/" + conditionClear.conditionData.numValue 
            : ""); 
    }

}
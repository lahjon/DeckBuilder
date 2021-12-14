using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
using System;

public class Scenario
{
    public List<ScenarioSegment> segments = new List<ScenarioSegment>();
    public ScenarioData data;
    public List<ScenarioSegment> nextStorySegments = new List<ScenarioSegment>();

    public Scenario(ScenarioData aData)
    {
        data = aData;
    }
    public void SetupInitialSegments()
    {
        segments.Clear();

        foreach (ScenarioSegmentData segmentData in data.SegmentDatas)
            if (segmentData.requiredSegmentsAND.Count == 0 && segmentData.requiredSegmentsOR.Count == 0)
                nextStorySegments.Add(new ScenarioSegment(segmentData,this));
        WorldSystem.instance.scenarioMapManager.StartCoroutine(SetupNextSegments());
    }
    public IEnumerator SetupNextSegments()
    {
        if (nextStorySegments.Count == 0)
        {
            if(data.linkedScenarioId == -1)
                WorldSystem.instance.worldMapManager.CompleteScenario();
            else
                WorldSystem.instance.scenarioMapManager.StartLinkedScenario();

        }

        for(int i = 0; i < nextStorySegments.Count;i++)
        {
            ScenarioSegment segment = nextStorySegments[i];
            yield return WorldSystem.instance.scenarioMapManager.StartCoroutine(segment.SetupSegment());
        }

        nextStorySegments.Clear();
    }
}

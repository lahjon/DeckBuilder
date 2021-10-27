using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
using System;

public class Scenario : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    WorldEncounterType _worldEncounterType;
    public ScenarioData worldEncounterData;
    public Reward encounterReward;

    public List<ScenarioSegment> segments = new List<ScenarioSegment>();
    bool _completed;

    public List<ScenarioSegment> nextStorySegments = new List<ScenarioSegment>();

    public bool completed
    {
        get => _completed;
        set
        {
            _completed = value;
            EventManager.CompleteWorldEncounter();
            CollectReward();
        }
    }
    public WorldEncounterType worldEncounterType
    {
        get => _worldEncounterType;
        set
        {
            _worldEncounterType = value;
            
            switch (_worldEncounterType)
            {
                case WorldEncounterType.Main:
                    GetComponent<RectTransform>().sizeDelta = new Vector2(50, 50);
                    GetComponent<Image>().color = Color.red;
                    break;
                case WorldEncounterType.Repeatable:
                    GetComponent<RectTransform>().sizeDelta = new Vector2(25, 25);
                    GetComponent<Image>().color = Color.blue;
                    break;
                case WorldEncounterType.Special:
                    GetComponent<RectTransform>().sizeDelta = new Vector2(25, 25);
                    GetComponent<Image>().color = Color.green;
                    break;
                default:
                    break;
            }
        }
    }

    public void ButtonOnClick()
    {
        WorldSystem.instance.worldMapManager.currentWorldEncounter = this;
        WorldSystem.instance.worldMapManager.worldMapConfirmWindow.OpenConfirmWindow(this);
        WorldSystem.instance.worldMapManager.worldEncounterTooltip.DisableTooltip();
    }

    public void BindData()
    {
        if (worldEncounterType == WorldEncounterType.None)
        {
            worldEncounterType = worldEncounterData.type;
            encounterReward = WorldSystem.instance.rewardManager.CreateReward(worldEncounterData.rewardStruct.type, worldEncounterData.rewardStruct.value, transform, false);
            segments.Clear();
            gameObject.SetActive(true);
            foreach (ScenarioSegmentData segmentData in worldEncounterData.SegmentDatas)
                segments.Add(new ScenarioSegment(segmentData,this));
        }
    }

    void RemoveEncounter()
    {
        WorldSystem.instance.worldMapManager.availableWorldEncounters.Remove(worldEncounterData.ScenarioName);
        WorldSystem.instance.worldMapManager.completedWorldEncounters.Add(worldEncounterData.ScenarioName);
        if (worldEncounterData.unlocksScenarios?.Any() == true)
        {
            foreach (ScenarioData enc in worldEncounterData.unlocksScenarios)
            {
                WorldSystem.instance.worldMapManager.availableWorldEncounters.Add(enc.ScenarioName);
            }
        }

        Destroy(gameObject);
    }

    public void CollectReward()
    {
        Debug.Log("Collect Reward");
        encounterReward.AddReward();
        RemoveEncounter();
        WorldStateSystem.SetInTownReward(true);
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        WorldSystem.instance.worldMapManager.worldEncounterTooltip.EnableTooltip(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        WorldSystem.instance.worldMapManager.worldEncounterTooltip.DisableTooltip();
    }


    public void SetEncounterDescription()
    {
        WorldSystem.instance.gridManager.conditionText.text = "hejhej";
    }


    public void SetupInitialSegments()
    {
        foreach (ScenarioSegment segment in segments)
            if (segment.data.requiredSegmentsAND.Count == 0 && segment.data.requiredSegmentsOR.Count == 0)
                nextStorySegments.Add(segment);
        WorldSystem.instance.gridManager.StartCoroutine(SetupNextSegments());
    }

    public IEnumerator SetupNextSegments()
    {
        Debug.Log("Setup next segments: " + nextStorySegments.Count);
        for(int i = 0; i < nextStorySegments.Count;i++)
        {
            ScenarioSegment segment = nextStorySegments[i];
            yield return WorldSystem.instance.gridManager.StartCoroutine(segment.SetupSegment());
        }

        nextStorySegments.Clear();
    }

}

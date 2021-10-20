using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
using System;

public class WorldEncounter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    WorldEncounterType _worldEncounterType;
    public WorldEncounterData worldEncounterData;
    public Reward encounterReward;
    public ConditionCounting conditionClear;
    public ConditionCounting conditionStartAnd;
    public ConditionCounting conditionStartOr;

    public List<WorldEncounterSegment> segments = new List<WorldEncounterSegment>();
    bool _completed;

    public List<WorldEncounterSegment> nextStorySegments = new List<WorldEncounterSegment>();

    public bool completed
    {
        get => _completed;
        set
        {
            _completed = value;
            EventManager.CompleteWorldEncounter();
            CollectReward();
            conditionClear.Unsubscribe();
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
            conditionClear = new ConditionCounting(worldEncounterData.clearCondition, OnPreconditionUpdate, OnConditionTrue);
            encounterReward = WorldSystem.instance.rewardManager.CreateReward(worldEncounterData.rewardStruct.type, worldEncounterData.rewardStruct.value, transform, false);
            segments.Clear();
            foreach (WorldEncounterSegmentData segmentData in worldEncounterData.SegmentDatas)
                segments.Add(new WorldEncounterSegment(segmentData,this));
        }
    }

    void RemoveEncounter()
    {
        conditionClear?.Unsubscribe();
        WorldSystem.instance.worldMapManager.availableWorldEncounters.Remove(worldEncounterData.worldEncounterName);
        WorldSystem.instance.worldMapManager.completedWorldEncounters.Add(worldEncounterData.worldEncounterName);
        if (worldEncounterData.unlockableEncounters?.Any() == true)
        {
            foreach (WorldEncounterData enc in worldEncounterData.unlockableEncounters)
            {
                WorldSystem.instance.worldMapManager.availableWorldEncounters.Add(enc.worldEncounterName);
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

    public void GetEncounterDescription()
    {
        WorldSystem.instance.gridManager.conditionText.text = conditionClear?.GetDescription(true);
    }

    public void OnPreconditionUpdate()
    {
        GetEncounterDescription();
    }

    public void OnConditionTrue()
    {
        completed = true;
    }

    public void SetupInitialSegments()
    {
        foreach (WorldEncounterSegment segment in segments)
            if (segment.data.requiredSegmentsAND.Count == 0 && segment.data.requiredSegmentsOR.Count == 0)
                nextStorySegments.Add(segment);
        WorldSystem.instance.gridManager.StartCoroutine(SetupNextSegments());
    }

    public IEnumerator SetupNextSegments()
    {
        Debug.Log("Setup next segments: " + nextStorySegments.Count);
        for(int i = 0; i < nextStorySegments.Count;i++)
        {
            WorldEncounterSegment segment = nextStorySegments[i];
            yield return WorldSystem.instance.gridManager.StartCoroutine(segment.SetupSegment());
        }

        nextStorySegments.Clear();
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
using System;

public class Scenario : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    WorldEncounterType _worldScenarioType;
    public ScenarioData data;
    public Reward scenarioReward;

    public List<ScenarioSegment> segments = new List<ScenarioSegment>();
    public bool completed;

    public List<ScenarioSegment> nextStorySegments = new List<ScenarioSegment>();

    public WorldEncounterType worldEncounterType
    {
        get => _worldScenarioType;
        set
        {
            _worldScenarioType = value;
            
            switch (_worldScenarioType)
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
        WorldSystem.instance.worldMapManager.currentWorldScenario = this;
        WorldSystem.instance.worldMapManager.worldMapConfirmWindow.OpenConfirmWindow(this);
        WorldSystem.instance.worldMapManager.worldScenarioTooltip.DisableTooltip();
    }

    public void BindData()
    {
        Debug.Log("Binding Data");
        if (worldEncounterType == WorldEncounterType.None)
        {
            worldEncounterType = data.type;
            scenarioReward = WorldSystem.instance.rewardManager.CreateRewardNormal(data.rewardStruct.type, data.rewardStruct.value, transform);
            segments.Clear();
            gameObject.SetActive(true);
            foreach (ScenarioSegmentData segmentData in data.SegmentDatas)
                segments.Add(new ScenarioSegment(segmentData,this));
        }
    }

    void RemoveScenario()
    {
        WorldSystem.instance.worldMapManager.availableScenarios.Remove(data.id);
        WorldSystem.instance.worldMapManager.completedScenarios.Add(data.id);
        if (data.unlocksScenarios?.Any() == true)
        {
            foreach (int id in data.unlocksScenarios)
            {
                WorldSystem.instance.worldMapManager.UnlockScenario(id);
            }
        }
        if (WorldSystem.instance.worldMapManager.worldScenarios.FirstOrDefault(x => x == this) is Scenario scenario) WorldSystem.instance.worldMapManager.worldScenarios.Remove(scenario);
        WorldSystem.instance.worldMapManager.currentWorldScenario = null;
        Destroy(gameObject);
    }

    public void CollectReward()
    {
        Debug.Log("Collect Reward");
        scenarioReward.AddReward();
        RemoveScenario();
        WorldStateSystem.SetInTownReward(true);
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        WorldSystem.instance.worldMapManager.worldScenarioTooltip.EnableTooltip(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        WorldSystem.instance.worldMapManager.worldScenarioTooltip.DisableTooltip();
    }

    public void SetupInitialSegments()
    {
        foreach (ScenarioSegment segment in segments)
            if (segment.data.requiredSegmentsAND.Count == 0 && segment.data.requiredSegmentsOR.Count == 0)
                nextStorySegments.Add(segment);
        WorldSystem.instance.scenarioMapManager.StartCoroutine(SetupNextSegments());
    }

    public IEnumerator SetupNextSegments()
    {
        if (nextStorySegments.Count == 0)
        {
            if(data.linkedScenarioId == -1)
                WorldSystem.instance.worldMapManager.CompleteScenario(this);
            else
            {
                WorldStateSystem.instance.overrideTransitionType = TransitionType.EnterMap;
                WorldSystem.instance.scenarioMapManager.scenarioData = DatabaseSystem.instance.scenarios.Where(s => s.id == data.linkedScenarioId).FirstOrDefault();
                WorldSystem.instance.scenarioMapManager.ResetEncounters();
                Helpers.DelayForSeconds(1f, () => { 
                    WorldSystem.instance.scenarioMapManager.DeleteMap();
                    WorldSystem.instance.scenarioMapManager.GenerateMap();
                });
                WorldStateSystem.SetInOverworld();
            }

        }

        for(int i = 0; i < nextStorySegments.Count;i++)
        {
            ScenarioSegment segment = nextStorySegments[i];
            yield return WorldSystem.instance.scenarioMapManager.StartCoroutine(segment.SetupSegment());
        }

        nextStorySegments.Clear();
    }

}

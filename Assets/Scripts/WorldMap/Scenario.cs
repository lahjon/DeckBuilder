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
    public ScenarioData worldScenarioData;
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
            worldEncounterType = worldScenarioData.type;
            scenarioReward = WorldSystem.instance.rewardManager.CreateReward(worldScenarioData.rewardStruct.type, worldScenarioData.rewardStruct.value, transform, false);
            segments.Clear();
            gameObject.SetActive(true);
            foreach (ScenarioSegmentData segmentData in worldScenarioData.SegmentDatas)
                segments.Add(new ScenarioSegment(segmentData,this));
        }
    }

    void RemoveScenario()
    {
        WorldSystem.instance.worldMapManager.availableWorldScenarios.Remove(worldScenarioData.id);
        WorldSystem.instance.worldMapManager.completedWorldScenarios.Add(worldScenarioData.id);
        if (worldScenarioData.unlocksScenarios?.Any() == true)
        {
            foreach (int id in worldScenarioData.unlocksScenarios)
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

    public void SetScenariosDescription()
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
using System;

public class ScenarioUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    WorldEncounterType _worldScenarioType;
    public ScenarioData data;
    public Reward scenarioReward;

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
        WorldSystem.instance.worldMapManager.currentWorldScenarioData = data;
        WorldSystem.instance.worldMapManager.currentScenarioUI = this;
        WorldSystem.instance.worldMapManager.worldMapConfirmWindow.OpenConfirmWindow(this);
        WorldSystem.instance.worldMapManager.worldScenarioTooltip.DisableTooltip();
    }

    public void BindData()
    {
        if (worldEncounterType == WorldEncounterType.None)
        {
            worldEncounterType = data.type;
            scenarioReward = WorldSystem.instance.rewardManager.CreateRewardNormal(data.rewardStruct.type, data.rewardStruct.value, transform, false);
            scenarioReward.rewardText.gameObject.SetActive(false);
            gameObject.SetActive(true);
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        WorldSystem.instance.worldMapManager.worldScenarioTooltip.EnableTooltip(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        WorldSystem.instance.worldMapManager.worldScenarioTooltip.DisableTooltip();
    }

}

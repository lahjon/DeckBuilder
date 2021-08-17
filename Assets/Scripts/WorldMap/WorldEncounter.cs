using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class WorldEncounter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ICondition
{
    WorldEncounterType _worldEncounterType;
    public WorldEncounterData worldEncounterData;
    public Reward encounterReward;
    Condition condition;
    bool _completed;
    public bool completed
    {
        get => _completed;
        set
        {
            _completed = value;
            EventManager.CompleteWorldEncounter();
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
            condition = Condition.CreateCondition(worldEncounterData.clearCondition, UpdateCondition, CompleteCondition);
            Debug.Log(worldEncounterData.clearCondition);
            WorldSystem.instance.worldMapManager.worldEncounterTooltip.descriptionText.text = Condition.GetDescription(worldEncounterData.clearCondition);
            encounterReward = Instantiate(WorldSystem.instance.rewardManager.rewardPrefab, transform).GetComponent<Reward>();
            encounterReward.SetupReward(worldEncounterData.rewardStruct.type, worldEncounterData.rewardStruct.value, true);
            encounterReward.gameObject.SetActive(false);
        }
    }

    public void RemoveEncounter()
    {
        condition?.Unsubscribe();
        WorldSystem.instance.worldMapManager.availableWorldEncounters.Remove(worldEncounterData.worldEncounterName);
        WorldSystem.instance.worldMapManager.completedWorldEncounters.Add(worldEncounterData.worldEncounterName);
        if (worldEncounterData.unlockableEncounters?.Any() == true)
        {
            foreach (WorldEncounterData enc in worldEncounterData.unlockableEncounters)
            {
                WorldSystem.instance.worldMapManager.availableWorldEncounters.Add(enc.worldEncounterName);
            }
        }
    }

    public void CollectReward()
    {
        encounterReward.CollectCombatReward();
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        WorldSystem.instance.worldMapManager.worldEncounterTooltip.EnableTooltip(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        WorldSystem.instance.worldMapManager.worldEncounterTooltip.DisableTooltip();
    }

    public void UpdateCondition()
    {
        Debug.Log("Updating Condition: " + this + " " + condition);
    }

    public void CompleteCondition()
    {
        completed = true;
    }
}

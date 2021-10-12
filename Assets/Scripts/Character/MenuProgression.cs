using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class MenuProgression : MonoBehaviour
{
    public Transform objParent, missionParent;
    public GameObject progressionItemPrefab;
    public List<MenuItemProgression> completedObjs, currentObjs = new List<MenuItemProgression>();
    public List<MenuItemProgression> completedMissions, currentMissions = new List<MenuItemProgression>();
    public Image objCompletedButton, objCurrentButton;
    public Image missionCompletedButton, missionCurrentButton;
    public GameObject descriptionWindow;
    public TMP_Text descriptionTitle, descriptionText, descriptionGoals;
    public ProgressionData currentProgressionData;

    void OnEnable()
    {
        DisableDescription();
        UpdateMenu();
    }
    void UpdateMenu()
    {
        UpdateMissions();
    }
    public void DisableDescription()
    {
        if (descriptionWindow.activeSelf)
        {
            currentProgressionData = null;
            descriptionWindow.SetActive(false);
        }
    }
    public void EnableDescription(ProgressionData aData, Mission mission)
    {
        Debug.Log("Enable");
        currentProgressionData = aData;
        descriptionTitle.text = aData.aName;
        descriptionText.text = aData.description;
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        if (mission != null)
        {
            for (int i = 0; i < aData.conditionStructs.Count; i++)
            {
                sb.AppendLine(aData.conditionStructs[i].GetMissionDescription(mission.countingConditions[i].currentAmount, mission.countingConditions[i].requiredAmount));
            }
            descriptionGoals.text = sb.ToString();
        }
        else
        {
            descriptionGoals.text = aData.conditionStructs[0].GetMissionDescription(0, 0);
        }
        descriptionWindow.SetActive(true);
    }

    void UpdateMissions()
    {
        completedMissions.Clear();
        currentMissions.Clear();
        List<Mission> allCurrentMissions = WorldSystem.instance.missionManager.GetAllMission();
        List<ProgressionData> allCompletedMissions = new List<ProgressionData>();
        allCompletedMissions.AddRange(WorldSystem.instance.missionManager.clearedMissions);

        while (missionParent.childCount > allCurrentMissions.Count + allCompletedMissions.Count)
        {
            Destroy(missionParent.GetChild(0));
        }
        while (missionParent.childCount < allCurrentMissions.Count + allCompletedMissions.Count)
        {
            Instantiate(progressionItemPrefab, missionParent);
        }
        int idx = 0;
        for (int i = 0; i < missionParent.childCount; i++)
        {
            if (idx < allCurrentMissions.Count)
            {
                if (missionParent.GetChild(i).GetComponent<MenuItemProgression>() is MenuItemProgression item && allCurrentMissions[i] is Mission mission)
                {
                    item.SetMissionItem(mission);
                    currentMissions.Add(item);
                }
                idx++;
            }
            else
            {
                if (missionParent.GetChild(i).GetComponent<MenuItemProgression>() is MenuItemProgression item && allCompletedMissions[i - idx] is ProgressionData data)
                {
                    Debug.Log(data.aName);
                    item.SetMissionItem(data);
                    completedMissions.Add(item);
                }
            }
        }

        allCompletedMissions.ForEach(x => Debug.Log(x.aName));

        ButtonShowCurrentMissions();
    }

    public void ButtonShowCurrentMissions()
    {
        completedMissions.ForEach(x => x.gameObject.SetActive(false));
        currentMissions.ForEach(x => x.gameObject.SetActive(true));
        missionCompletedButton.color = Color.gray;
        missionCurrentButton.color = Color.white;
        //DisableDescription();
    }
    public void ButtonShowCompletedMissions()
    {
        completedMissions.ForEach(x => x.gameObject.SetActive(true));
        currentMissions.ForEach(x => x.gameObject.SetActive(false));
        missionCompletedButton.color = Color.white;
        missionCurrentButton.color = Color.gray;
        //DisableDescription();
    }
}
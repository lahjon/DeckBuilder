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
        UpdateObjectives();
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
    public void EnableDescription(ProgressionData aData, int cAmount, int rAmount)
    {
        Debug.Log("Enable");
        currentProgressionData = aData;
        descriptionTitle.text = aData.aName;
        descriptionText.text = aData.description;
        descriptionGoals.text = rAmount != 0 ? string.Format("{0} - ({1} / {2})", aData.conditionStructs[0].type.ToString(), cAmount.ToString(), rAmount.ToString()) : "Completed";
        descriptionWindow.SetActive(true);
    }

    void UpdateMissions()
    {
        completedMissions.Clear();
        currentMissions.Clear();
        List<Mission> allCurrentMissions = WorldSystem.instance.missionManager.GetAllMission();
        List<ProgressionData> allCompletedMissions = new List<ProgressionData>();
        allCompletedMissions.AddRange(WorldSystem.instance.missionManager.clearedMissions);

        int counter = 0;
        while (missionParent.childCount > allCurrentMissions.Count + allCompletedMissions.Count)
        {
            counter++;
            Destroy(missionParent.GetChild(0));
            if (counter > 50)
            {
                Debug.LogError("DangerDestr!");
                break;
            }
        }
        counter = 0;
        while (missionParent.childCount < allCurrentMissions.Count + allCompletedMissions.Count)
        {
            counter++;
            Instantiate(progressionItemPrefab, missionParent);
            if (counter > 50)
            {
                Debug.LogError("DangerInstant!");
                break;
            }
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

    void UpdateObjectives()
    {
        completedObjs.Clear();
        currentObjs.Clear();
        List<Objective> allCurrentObjs = WorldSystem.instance.objectiveManager.GetAllObjectives();
        List<ProgressionData> allCompletedObjs = new List<ProgressionData>();
        allCompletedObjs.AddRange(WorldSystem.instance.objectiveManager.clearedObjectives);

        int counter = 0;
        while (objParent.childCount > allCurrentObjs.Count + allCompletedObjs.Count)
        {
            counter++;
            Destroy(objParent.GetChild(0));
            if (counter > 50)
            {
                Debug.LogError("DangerDestr!");
                break;
            }
        }
        counter = 0;
        while (objParent.childCount < allCurrentObjs.Count + allCompletedObjs.Count)
        {
            counter++;
            Instantiate(progressionItemPrefab, objParent);
            if (counter > 50)
            {
                Debug.LogError("DangerInstant!");
                break;
            }
        }
        int idx = 0;
        for (int i = 0; i < objParent.childCount; i++)
        {
            if (idx < allCurrentObjs.Count)
            {
                if (objParent.GetChild(i).GetComponent<MenuItemProgression>() is MenuItemProgression item && allCurrentObjs[i] is Objective objective)
                {
                    item.SetObjectiveItem(objective);
                    currentObjs.Add(item);
                }
                idx++;
            }
            else
            {
                if (objParent.GetChild(i).GetComponent<MenuItemProgression>() is MenuItemProgression item && allCompletedObjs[i - idx] is ProgressionData data)
                {
                    Debug.Log(data.aName);
                    item.SetObjectiveItem(data);
                    completedObjs.Add(item);
                }
            }
        }

        allCompletedObjs.ForEach(x => Debug.Log(x.aName));

        ButtonShowCurrentObjectives();
    }

    public void ButtonShowCurrentObjectives()
    {
        completedObjs.ForEach(x => x.gameObject.SetActive(false));
        currentObjs.ForEach(x => x.gameObject.SetActive(true));
        objCompletedButton.color = Color.gray;
        objCurrentButton.color = Color.white;
        //DisableDescription();
    }
    public void ButtonShowCompletedObjectives()
    {
        completedObjs.ForEach(x => x.gameObject.SetActive(true));
        currentObjs.ForEach(x => x.gameObject.SetActive(false));
        objCompletedButton.color = Color.white;
        objCurrentButton.color = Color.gray;
        //DisableDescription();
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
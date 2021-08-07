using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class MissionUI : MonoBehaviour
{
    public Canvas canvas;
    public string missionName;
    public Dictionary<string, bool> goals = new Dictionary<string, bool>();
    public TMP_Text missionText;
    List<GameObject> goalObjs = new List<GameObject>();
    public GameObject goalPrefab;
    public Transform panel;
    public void UpdateUI(bool newMission)
    {
        if (WorldSystem.instance.missionManager.mission != null)
        {
            missionName = WorldSystem.instance.missionManager.mission.aName;
            missionText.text = missionName;

            goals.Clear();

            WorldSystem.instance.missionManager.mission.goals.ForEach(x => goals.Add(x.goalName, x.completed));
            if (newMission)
            {
                ClearUI();
                CreateGoals();
            }
            for (int i = 0; i < goals.Count; i++)
            {
                if (goals.ElementAt(i).Value == true)
                {
                    goalObjs[i].GetComponent<TMP_Text>().color = Color.green;
                }
            }
        }
      
    }
    public void ClearUI(bool nameChange = false)
    {
        goalObjs?.ForEach(x => Destroy(x));
        goalObjs.Clear();
        
        if (nameChange)
        {
            missionText.text = "No mission";
        }
    }

    void CreateGoals()
    {
        foreach (string goal in goals.Keys)
        {
            GameObject newGoal = Instantiate(goalPrefab, panel);
            newGoal.GetComponent<TMP_Text>().text = goal;
            goalObjs.Add(newGoal);
        }
    }
}

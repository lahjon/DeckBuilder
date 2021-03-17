using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardScreen : MonoBehaviour
{
    public GameObject content;
    public EncounterData encounterData;
    public GameObject rewardScreenCard;
    public GameObject rewardScreenCardContent;
    public GameObject canvas;
    public GameObject canvasCard;
    public Reward currentReward;
    public List<GameObject> combatRewardNormal;
    public List<GameObject> combatRewardElite;
    public List<GameObject> combatRewardBoss;
    public GameObject cardDisplayPrefab;
    int[] keys = new int[] { 1,2,3,4,5};

    void Update()
    {
        for(int i = 0; i < keys.Length && i < content.transform.childCount; i++)
        {
            if (Input.GetKeyDown(keys[i].ToString()) && WorldStateSystem.instance.currentWorldState == WorldState.Reward && currentReward == null)
            {
                Reward currentReward = content.transform.GetChild(keys[i] - 1).GetComponent<Reward>();
                if(currentReward is RewardCard)
                {
                    currentReward.OnClick(false);
                }
                else if (currentReward is RewardGold)
                {
                    currentReward.OnClick();
                }
                break;
            }
        }
        if(Input.GetKeyDown(KeyCode.Space ) && WorldStateSystem.instance.currentWorldState == WorldState.Reward)
            RemoveRewardScreen();
    }

    public void OnCanvasEnable()
    {
        int i = 0;
        while (content.transform.childCount > 0)
        {
            i++;
            if (i > 50)
            {
                break;
            }
            DestroyImmediate(content.transform.GetChild(0).gameObject);
        }

        encounterData = WorldSystem.instance.encounterManager.currentEncounter.encounterData;

        switch (encounterData.type)
        {
            case EncounterType.OverworldCombatElite:
                CreateRewards(combatRewardElite);
                break;

            case EncounterType.OverworldCombatBoss:
                CreateRewards(combatRewardBoss);
                break;

            case EncounterType.OverworldCombatNormal:
                CreateRewards(combatRewardNormal);
                break;

            default:
                Debug.Log("No default rewards!");
                break;
        }
    }

    public void ResetCurrentReward()
    {
        if (currentReward != null)
        {
            currentReward.RemoveReward();
            currentReward = null;
        }
        else
        {
            WorldStateSystem.SetInReward(false);
        }
    }

    public void ResetCurrentRewardEvent()
    {
        WorldStateSystem.SetInReward(false);
    }

    private void CreateRewards(List<GameObject> rewards)
    {
        foreach (GameObject reward in rewards)
        {
            GameObject newObject = Instantiate(reward, new Vector3(0, 0, 0), Quaternion.identity);
            newObject.transform.SetParent(content.transform);
            newObject.transform.localScale =  new Vector3(1, 1, 1);
        }
    }

    public void RemoveRewardScreen()
    {
        canvas.SetActive(false);
        canvasCard.SetActive(false);
        WorldStateSystem.SetInReward(false);
        Debug.Log("RewardScreen Removing card!");
        if(encounterData.type == EncounterType.OverworldCombatBoss)
            WorldSystem.instance.EndCombat(true);
        else
            WorldSystem.instance.EndCombat();
    }
}

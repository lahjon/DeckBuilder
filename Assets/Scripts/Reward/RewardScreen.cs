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
    public Reward currentReward;
    public List<GameObject> combatRewardNormal;
    public List<GameObject> combatRewardElite;
    public List<GameObject> combatRewardBoss;
    int[] keys = new int[] { 1,2,3,4,5};

    void Update()
    {
        for(int i = 0; i < keys.Length && i < content.transform.childCount; i++)
        {
            if (Input.GetKeyDown(keys[i].ToString()) && WorldSystem.instance.worldState == WorldState.Reward)
            {
                Reward tempReward = content.transform.GetChild(keys[i] - 1).GetComponent<Reward>();
                if(tempReward is RewardCard)
                {
                    tempReward.OnClick(false);
                }
                else if (tempReward is RewardGold)
                {
                    tempReward.OnClick();
                }
                break;
            }
        }
        if(Input.GetKeyDown(KeyCode.Space ) && WorldSystem.instance.worldState == WorldState.Reward)
            RemoveRewardScreen();
    }

    void OnCanvasEnable()
    {
        while (content.transform.childCount > 0)
        {
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

            default:
                CreateRewards(combatRewardNormal);
                break;
        }
    }

    public void ResetCurrentReward()
    {
        currentReward.OnClick();
        rewardScreenCard.SetActive(false);
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
    public void GetCombatReward()
    {
        OnCanvasEnable();
        WorldSystem.instance.combatManager.combatController.content.gameObject.SetActive(false);
        
        WorldSystem.instance.SwapState(WorldState.Reward);
        canvas.SetActive(true);
    }

    public void RemoveRewardScreen()
    {
        canvas.SetActive(false);
        if(encounterData.type == EncounterType.OverworldCombatBoss)
            WorldSystem.instance.EndCombat(true);
        else
            WorldSystem.instance.EndCombat();
    }
}

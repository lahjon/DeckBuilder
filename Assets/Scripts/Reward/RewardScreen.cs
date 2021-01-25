using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardScreen : MonoBehaviour
{
    public GameObject content;
    public EncounterData encounterData;
    public GameObject rewardScreenCard;
    public GameObject rewardScreenCardContent;
    public Reward currentReward;
    public List<GameObject> combatRewardNormal;
    public List<GameObject> combatRewardElite;
    public List<GameObject> combatRewardBoss;

    void OnEnable()
    {
        int iter = 0;
        while (content.transform.childCount > 0)
        {
            
            DestroyImmediate(content.transform.GetChild(0).gameObject);
            iter++;
            if(iter > 50)
            {
                Debug.Log("safety break in loop if error");
                break;
            }
        }

        encounterData = WorldSystem.instance.encounterManager.currentEncounter.encounterData;
        Debug.Log(encounterData.type);

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
        }
    }
    public void GetCombatReward()
    {
        WorldSystem.instance.combatManager.combatController.content.gameObject.SetActive(false);
        
        WorldSystem.instance.SwapState(WorldState.Reward);
        gameObject.SetActive(true);
    }

    public void RemoveRewardScreen()
    {
        gameObject.SetActive(false);
        if(encounterData.type == EncounterType.OverworldCombatBoss)
            WorldSystem.instance.EndCombat(true);
        else
            WorldSystem.instance.EndCombat();
    }
}

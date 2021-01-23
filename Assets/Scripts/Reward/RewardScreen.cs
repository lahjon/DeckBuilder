using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardScreen : MonoBehaviour
{
    public GameObject rewardPrefab;
    public GameObject content;
    public Reward rewards;
    public EncounterData encounterData;

    void Start()
    {
        encounterData = WorldSystem.instance.combatManager.combatController.encounterData;

        switch (encounterData.type)
        {
            case EncounterType.CombatElite:
                break;

            case EncounterType.CombatBoss:
                break;

            default:
                break;
        }

        // foreach (Reward reward in rewards)
        // {
        //     GameObject newObject = Instantiate(rewardPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        //     newObject.transform.parent = content.transform;
        //     newObject.GetComponent<RewardButton>().reward = reward;
        // }
    }
    
    // public void CreateReward()
    // {
    //     GameObject newObject = Instantiate(rewardPrefab, new Vector3(0, 0, 0), Quaternion.identity);
    //     newObject.transform.parent = content.transform;
    //     RewardButton rewardButton = newObject.GetComponent<RewardButton>();
    //     rewardButton.image
    // }
}

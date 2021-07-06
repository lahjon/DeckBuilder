using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardScreen : MonoBehaviour
{
    public Reward rewardPrefab;
    public GameObject content;
    public Transform anchor;
    public Canvas canvas;
    public Reward reward;

    public void GetReward(RewardType rewardType, string value = "")
    {
        reward = Instantiate(rewardPrefab, anchor).GetComponent<Reward>();   
        reward.SetupReward(rewardType);
        Destroy(reward.rewardText.gameObject);
        reward.transform.localScale = Vector3.one * 2;
        reward.GetComponent<Button>().onClick.RemoveAllListeners();
        reward.GetComponent<Button>().onClick.AddListener(() => {
            reward.CollectCombatReward();
            reward.GetComponent<ToolTipScanner>().ExitAction();
            ClearScreen();
        });
        content.GetComponent<Button>().onClick.RemoveAllListeners();
        content.GetComponent<Button>().onClick.AddListener(() => {
            reward.CollectCombatReward();
            reward.GetComponent<ToolTipScanner>().ExitAction();
            ClearScreen();
        });
        WorldStateSystem.SetInRewardScreen();
    }

    public void OpenScreen()
    {
        canvas.gameObject.SetActive(true);
    }

    public void ClearScreen()
    {
        if (reward != null)
            Destroy(reward.gameObject);
            
        WorldStateSystem.TriggerClear();
        canvas.gameObject.SetActive(false);
    }
}

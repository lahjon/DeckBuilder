using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class RewardScreenCombat : MonoBehaviour
{
    public Transform content;
    public int rewardCount;
    public GameObject rewardScreenCardContent;
    public GameObject canvas;
    public GameObject canvasCard;
    public System.Action callback;
    public GameObject cardDisplayPrefab;
    public void SetupRewards()
    {
        canvas.SetActive(true);
        CombatEncounterType type;
        Debug.Log("Setup Rewards");
        if (CombatSystem.instance.encounterData != null)
            type = CombatSystem.instance.encounterData.type;
        else
            type = CombatEncounterType.Normal;

        while (content.childCount > 0)
            Destroy(content.GetChild(0).gameObject);

        switch (type)
        {
            case CombatEncounterType.Normal:
                CreateCombatRewards(WorldSystem.instance.combatRewardManager.combatRewardNormal);
                break;
            case CombatEncounterType.Elite:
                CreateCombatRewards(WorldSystem.instance.combatRewardManager.combatRewardElite);
                break;
            case CombatEncounterType.Boss:
                CreateCombatRewards(WorldSystem.instance.combatRewardManager.combatRewardBoss);
                break;
            default:
                break;
        }

        CanvasGroup canvasGroup = canvas.GetComponent<CanvasGroup>();
        canvasGroup.interactable = false;
        canvasGroup.alpha = 0;

        DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 1, 1.5f).OnComplete( () => canvasGroup.interactable = true );
    }

    public void ResetReward()
    {
        if (rewardCount < 1 )
            WorldStateSystem.SetInCombatReward(false);
    }

    public void ResetCurrentRewardEvent()
    {
        WorldStateSystem.SetInCombatReward(false);
    }

    private void CreateCombatRewards(RewardCombatStruct[] rewards)
    {
        foreach (RewardCombatStruct reward in rewards)
        {
            Reward newReward = WorldSystem.instance.combatRewardManager.CreateRewardCombat(reward.type, reward.value);
            newReward.gameObject.SetActive(true);
            rewardCount++;
        }
    }

    public void RemoveRewardScreen()
    {
        canvas.SetActive(false);
        canvasCard.SetActive(false);
        rewardCount = 0;

        if (callback != null)
        {
            callback.Invoke();
            callback = null;
        }

        WorldStateSystem.SetInCombatReward(false);

        for (int i = 0; i < content.childCount; i++)
        {
            Destroy(content.GetChild(i).gameObject);
        }
    }
}

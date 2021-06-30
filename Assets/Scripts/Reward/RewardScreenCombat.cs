using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class RewardScreenCombat : MonoBehaviour
{
    public GameObject content;
    public int rewardCount;
    public EncounterDataCombat encounterData;
    public GameObject rewardScreenCard;
    public GameObject rewardScreenCardContent;
    public GameObject canvas;
    public GameObject canvasCard;
    public RewardType[] combatRewardNormal;
    public RewardType[] combatRewardElite;
    public RewardType[] combatRewardBoss;
    public GameObject cardDisplayPrefab;
    public void SetupRewards()
    {
        canvas.SetActive(true);

        while (content.transform.childCount > 0)
            Destroy(content.transform.GetChild(0).gameObject);

        switch (CombatSystem.instance.encounterData.type)
        {
            case CombatEncounterType.Normal:
                CreateRewards(combatRewardNormal);
                break;
            case CombatEncounterType.Elite:
                CreateRewards(combatRewardElite);
                break;
            case CombatEncounterType.Boss:
                CreateRewards(combatRewardBoss);
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
            WorldStateSystem.SetInReward(false);
    }

    public void ResetCurrentRewardEvent()
    {
        WorldStateSystem.SetInReward(false);
    }

    private void CreateRewards(RewardType[] rewards)
    {
        foreach (RewardType reward in rewards)
        {
            Reward newReward = Instantiate(WorldSystem.instance.rewardManager.rewardPrefab, content.transform).GetComponent<Reward>();
            newReward.rewardType = reward;
            newReward.SetupReward();
            rewardCount++;
        }
    }

    public void RemoveRewardScreen()
    {
        canvas.SetActive(false);
        canvasCard.SetActive(false);
        rewardCount = 0;

        if (WorldSystem.instance.gridManager.bossStarted)
        {
            WorldSystem.instance.BossDefeated();
        }

        WorldStateSystem.SetInReward(false);

        for (int i = 0; i < content.transform.childCount; i++)
        {
            Destroy(content.transform.GetChild(i).gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Reward : MonoBehaviour
{
    protected abstract void CollectCombatReward();
    public virtual void OnClick(bool destroy = true)
    {
        CollectCombatReward();
        if(destroy == true)
        {
            RemoveReward();
        }
    }

    public virtual void RemoveReward()
    {
        DestroyImmediate(this.gameObject);
        if(WorldSystem.instance.rewardManager.rewardScreenCombat.content.transform.childCount == 0)
            WorldSystem.instance.rewardManager.rewardScreenCombat.RemoveRewardScreen();
    }
}

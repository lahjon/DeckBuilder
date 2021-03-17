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
        if(WorldSystem.instance.uiManager.rewardScreen.content.transform.childCount == 0)
            WorldSystem.instance.uiManager.rewardScreen.RemoveRewardScreen();
    }
    
    // public ArtifactData GetArtifact(bool random = true, ArtifactData artifactData = null)
    // {
    //     return null;
    // }

    // public List<CardData> ChooseCard(CharacterClass characterClass, int amount = 3, bool random = true, CardData cardData = null)
    // {
    //     List<CardData> cardReward = new List<CardData>(); 

    //     for (int i = 0; i < amount; i++)
    //     {
    //         cardReward.Add(DatabaseSystem.instance.GetRandomCard(characterClass));
    //     }

    //     return cardReward;
    // }
}

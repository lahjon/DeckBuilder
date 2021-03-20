using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class LevelReward : MonoBehaviour
{
    public string rewardName;
    public Image artwork;
    public virtual void AddEffect()
    {
        
    }
}

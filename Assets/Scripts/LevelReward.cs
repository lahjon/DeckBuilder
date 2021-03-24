using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[CreateAssetMenu(fileName = "NewLevelReward", menuName = "CardGame/LevelReward")]
public class LevelReward : ScriptableObject
{
    public string rewardName;
    [TextArea(5,5)]
    public string description;
    public Sprite artwork;
    public List<string> effectName = new List<string>();
}

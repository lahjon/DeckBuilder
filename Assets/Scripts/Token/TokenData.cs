using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewTokenData", menuName = "CardGame/TokenData")]
public class TokenData : ScriptableObject
{
    public int cost;

    [TextArea(5,5)]
    public string description;
    public Sprite artwork;
    public List<string> effectName = new List<string>();
}

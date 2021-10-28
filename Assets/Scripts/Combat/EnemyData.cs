using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "NewEnemy", menuName = "CardGame/Enemy")]
public class EnemyData : ScriptableObject
{
    public int StartingHP;
    public int enemyId;
    public string enemyName;
    public Sprite artwork;
    public GameObject characterArt;
    public int tier;
    public int experience;
    public List<CardData> deck = new List<CardData>();

    public List<CardEffectCarrier> startingEffects = new List<CardEffectCarrier>();
    public List<CardActivity> startingActivities = new List<CardActivity>();

    public bool shuffleInit = true;
    public bool stochasticReshuffle = true;

}

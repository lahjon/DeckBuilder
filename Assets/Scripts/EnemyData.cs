using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "NewEnemy", menuName = "CardGame/Enemy")]
public class EnemyData : ScriptableObject
{
    public int StartingHP;
    public string enemyId;
    public string enemyName;
    public Sprite artwork;
    public GameObject characterArt;
    public int tier;
    public int experience;
    public List<CardData> deck;
    
}

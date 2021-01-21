using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "NewEnemy", menuName = "Enemy")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public Sprite artwork;
    public int tier;
    public CardData deck;
    
}

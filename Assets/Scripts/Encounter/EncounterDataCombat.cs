using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEncounter", menuName = "CardGame/EncounterDataCombat")]
public class EncounterDataCombat : EncounterData
{
    public EnemyType type;

    public List<EnemyData> enemyData = new List<EnemyData>();
  
    public List<CardEffect> startingEffects = new List<CardEffect>();
    public List<int> startEffectsTargets = new List<int>();
    public List<CardActivity> startingActivities = new List<CardActivity>();

}

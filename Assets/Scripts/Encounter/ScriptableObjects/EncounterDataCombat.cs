using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEncounter", menuName = "CardGame/EncounterDataCombat")]
public class EncounterDataCombat : EncounterData
{
    public CombatEncounterType type;
    public FormationType formation;

    public List<EnemyData> enemyData = new List<EnemyData>();
  
    public List<CardEffectInfo> startingEffects = new List<CardEffectInfo>();
    public List<int> startEffectsTargets = new List<int>();
    public List<CardActivity> startingActivities = new List<CardActivity>();

}

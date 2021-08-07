using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "NewProgressionData", menuName = "CardGame/ProgressionData")]
public class ProgressionData : ScriptableObject
{
    public string id;
    public string aName;
    public List<KillEnemyGoalStruct> goalKillEnemy;
    public List<EnterBuildingGoalStruct> goalEnterBuilding;

}

[System.Serializable] public struct KillEnemyGoalStruct
{
    public string enemyId;
    public int requiredAmount;
}

[System.Serializable] public struct EnterBuildingGoalStruct
{
    public BuildingType buildingType;
    public int requiredAmount;
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Item", menuName = "CardGame/Item")][System.Serializable]
public class ItemUseableData : ItemData
{
    public ConditionData itemCondition;
    public List<WorldState> statesUsable = new List<WorldState>();

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Item", menuName = "CardGame/Item")][System.Serializable]
public class UseItemData : ItemData
{
    public ItemConditionStruct itemCondition;
    public List<WorldState> itemUseCondition;

}
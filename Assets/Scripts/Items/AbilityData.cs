using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Ability", menuName = "CardGame/Ability")]
public class AbilityData : ItemData
{
    public ConditionData itemCondition;
    public List<WorldState> statesUsable = new List<WorldState>();

}

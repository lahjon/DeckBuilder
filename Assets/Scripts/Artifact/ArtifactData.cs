using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewArtifactData", menuName = "CardGame/ArtifactData")]
public class ArtifactData : ItemData
{
    public int goldValue;
    public CharacterClassType characterClassType;
    public bool isBuff;
    public ConditionData conditionCounting = new ConditionData();
    public ConditionCountingOnTrueType conditionCountingOnTrueType = ConditionCountingOnTrueType.Nothing;
    public ConditionType conditionResetEvent = ConditionType.None;
}

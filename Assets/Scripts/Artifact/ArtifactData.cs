using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewArtifactData", menuName = "CardGame/ArtifactData")]
public class ArtifactData : ItemData
{
    public ArtifactType type;
    public int goldValue;
    public CharacterClassType characterClassType;
    public bool isBuff;
    public ConditionData condition = new ConditionData();
    public ConditionCountingOnTrueType conditionCountingOnTrueType = ConditionCountingOnTrueType.Nothing;
    public ConditionType conditionResetEvent = ConditionType.None;
}

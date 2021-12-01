using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewArtifactData", menuName = "CardGame/ArtifactData")]
public class ArtifactData : ItemData
{
    public int goldValue;
    public CharacterClassType characterClassType;
}

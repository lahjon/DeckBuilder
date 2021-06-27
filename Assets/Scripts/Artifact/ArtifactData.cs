using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewArtifactData", menuName = "CardGame/ArtifactData")]
public class ArtifactData : ScriptableObject
{
    public string artifactName;
    public int artifactId;

    [TextArea(5,5)]
    public string description;
    public Rarity rarity;
    public Sprite artwork;

}

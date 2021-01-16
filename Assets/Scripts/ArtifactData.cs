using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewArtifact", menuName = "Artifact")]
public class ArtifactData : ScriptableObject
{
    public string artifactName;
    public int goldValue;
    public Sprite artwork;
    public Rarity rarity;
}
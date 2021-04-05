using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardRandomArtifact : Reward
{
    public Image icon;
    public TMP_Text text;
    ArtifactData artifact;

    void Start()
    {
        artifact = WorldSystem.instance.artifactManager.GetRandomAvailableArtifact();
        icon.sprite = artifact.artwork;
        text.text = artifact.artifactName;
    }

    protected override void CollectCombatReward()
    {
        WorldSystem.instance.artifactManager.AddArifact(artifact.artifactName);
    }
}
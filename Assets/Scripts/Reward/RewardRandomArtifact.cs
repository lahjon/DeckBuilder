using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardRandomArtifact : Reward
{
    public Image icon;
    public TMP_Text text;
    ArtifactData artifactData;

    void Start()
    {
        artifactData = WorldSystem.instance.artifactManager.GetRandomAvailableArtifact();
        icon.sprite = artifactData.artwork;
        text.text = artifactData.artifactName;
        icon?.GetComponent<Artifact>().BindArtifactData(artifactData);
    }

    protected override void CollectCombatReward()
    {
        WorldSystem.instance.artifactManager.AddArifact(artifactData.artifactName);
    }
}
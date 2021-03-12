using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardRandomArtifact : Reward
{
    public Image icon;
    public TMP_Text text;
    GameObject artifact;

    void Start()
    {
        artifact = WorldSystem.instance.artifactManager.GetRandomAvailableArtifact();
        icon.sprite = artifact.GetComponent<Image>().sprite;
        text.text = artifact.GetComponent<Artifact>().displayName;
    }

    protected override void CollectCombatReward()
    {
        WorldSystem.instance.artifactManager.AddArifact(artifact);
    }
}
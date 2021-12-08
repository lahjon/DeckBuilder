using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class ArtifactCounting : Artifact
{

    public ConditionCounting condition;
    public override void BindData(ArtifactData anArtifactData)
    {
        if (anArtifactData is null) return;
        base.BindData(anArtifactData);
        itemEffect.itemEffectStruct.addImmediately = false;

        itemEffect.Register();
        condition = new ConditionCounting(artifactData.condition, null, itemEffect.ApplyEffect, artifactData.conditionCountingOnTrueType, artifactData.conditionResetEvent);
        condition.Subscribe();
    }
}


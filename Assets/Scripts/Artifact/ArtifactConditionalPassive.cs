using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class ArtifactConditionalPassive : Artifact
{

    public Condition condition;
    public override void BindData(ArtifactData anArtifactData)
    {
        if (anArtifactData is null) return;
        base.BindData(anArtifactData);
        itemEffect.itemEffectStruct.addImmediately = false;

        condition = new Condition(artifactData.condition, null, null,itemEffect.Register ,itemEffect.DeRegister);
        condition.Subscribe();
    }
}


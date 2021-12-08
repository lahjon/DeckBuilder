using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class ArtifactPassive : Artifact
{
    public override void BindData(ArtifactData anArtifactData)
    {
        if (anArtifactData is null) return;
        base.BindData(anArtifactData);
        itemEffect.itemEffectStruct.addImmediately = true;
        itemEffect.Register();
    }
}


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

        condition = Condition.Factory(artifactData.condition, null, null,itemEffect.Register ,itemEffect.DeRegister);
        condition.Subscribe();
    }
    public override void NotifyRegister()
    {
        if (tween != null) tween.Kill();
        tween = transform.DOScale(1.1f * Vector3.one, 1f).SetLoops(-1, LoopType.Yoyo).OnKill(() => transform.localScale = Vector3.one);
    }

    public override void NotifyDeregister()
    {
        Debug.Log("Deregister conditionalPassive");
        tween?.Kill();
    }

    public override void NotifyUsed()
    {
    }

}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class ArtifactCounting : Artifact
{

    public ConditionCounting condition;
    public TMP_Text lblCounter;
    Tween myTween;

    public override void BindData(ArtifactData anArtifactData)
    {
        if (anArtifactData is null) return;
        base.BindData(anArtifactData);
        itemEffect.itemEffectStruct.addImmediately = true;

        itemEffect.Register();
        condition = ConditionCounting.Factory(artifactData.condition, UpdateText, itemEffect.ApplyEffect, artifactData.conditionCountingOnTrueType, artifactData.conditionResetEvent);
        condition.Subscribe();
        UpdateText();
    }

    void UpdateText()
    {
        if (condition.requiredAmount == 1)
            lblCounter.text = "";
        else
        {
            lblCounter.text = condition.currentAmount.ToString();
            if (myTween != null) myTween.Kill();
            myTween = lblCounter.transform.DOScale(1.2f * Vector3.one, .3f).SetLoops(2, LoopType.Yoyo).OnKill(() => transform.localScale = Vector3.one);
        }
    }
}


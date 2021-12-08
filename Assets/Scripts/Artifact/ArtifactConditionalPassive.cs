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
        Initialize();
        if (anArtifactData != null)
        {
            artifactData = anArtifactData;
            image.sprite = artifactData.artwork;
            id = artifactData.itemId;
            itemName = anArtifactData.itemName;
            itemEffect = ItemEffect.Factory(artifactData.itemEffectStruct, this);
            condition = new Condition(artifactData.condition, () => Debug.Log("I Reacted"), null, itemEffect.Register,itemEffect.DeRegister);
            Debug.Log("Subscribing for artifact onditional");
            condition.Subscribe();
        }
    }
}


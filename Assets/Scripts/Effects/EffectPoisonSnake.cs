using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPoisonSnake : Effect
{
    public override void AddEffect()
    {
        Debug.Log(string.Format("Adding effect {0}!", this.GetType().Name));
        //WorldSystem.instance.townManager.scribe.extraCards.Add(DatabaseSystem.instance.GetCardsByName());
    }

    public override void RemoveEffect()
    {
        Debug.Log(string.Format("Removing effect {0}!", this.GetType().Name));
    }
}

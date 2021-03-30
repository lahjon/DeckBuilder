using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEffect1 : Effect
{
    public override void AddEffect()
    {
        Debug.Log(string.Format("Adding effect {0}!", this.GetType().Name));
        WorldSystem.instance.characterManager.characterStats.ModifyHealth(51);
    }

    public override void RemoveEffect()
    {
        Debug.Log(string.Format("Removing effect {0}!", this.GetType().Name));
        WorldSystem.instance.characterManager.characterStats.ModifyHealth(-51);
    }
}

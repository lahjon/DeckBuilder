// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class EffectSpikeyTail : ItemEffect
// {
//     public override void AddEffect()
//     {
//         Debug.Log(string.Format("Adding effect {0}!", this.GetType().Name));
//         CombatSystem.instance.Hero.actionsStartCombat.Add(ApplyThorns);
//     }

//     public override void RemoveEffect()
//     {
//         Debug.Log(string.Format("Removing effect {0}!", this.GetType().Name));
//         CombatSystem.instance.Hero.actionsStartCombat.Remove(ApplyThorns);
//     }

//     IEnumerator ApplyThorns()
//     {
//         StartCoroutine(CombatSystem.instance.Hero.RecieveEffectNonDamageNonBlock(new CardEffectCarrier(EffectType.Thorns, 3)));
//         yield return null;
//     }
// }

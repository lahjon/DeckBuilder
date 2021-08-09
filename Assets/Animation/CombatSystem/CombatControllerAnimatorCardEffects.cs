using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CombatControllerAnimatorCardEffects : CombatControllerAnimatorCard
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetRefs(animator);
        CombatSystem.instance.StartCoroutine(DealEffects());
    }

    IEnumerator DealEffects()
    {
        //Debug.Log("Starting effets transmittion");
        List<CombatActor> targets;

        foreach (CardEffectInfo e in card.effectsOnPlay.Where(e=> card.EffectToCondition[e].value))
        {
            targets = CombatSystem.instance.GetTargets(CombatSystem.instance.ActiveActor, e.Target, suppliedTarget);

            for (int i = 0; i < e.Times; i++)
            {
                foreach (CombatActor actor in targets)
                    yield return CombatSystem.instance.StartCoroutine(actor.RecieveEffectNonDamageNonBlock(e));

                //Redraw enemy if random
                if(e.Target == CardTargetType.EnemyRandom && i != e.Times-1)
                    targets = CombatSystem.instance.GetTargets(CombatSystem.instance.ActiveActor, e.Target, suppliedTarget);
            }
        }

        CombatSystem.instance.animator.SetTrigger("CardEffectsHandled");
        yield return null;
    }
        



}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CombatControllerAnimatorInitialize : CombatControllerAnimator
{
    (CardEffect effect, List<CombatActor> targets) effectAndTarget;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetRefs(animator);
        combatController.StartCoroutine(SetupCombat());
        combatController.combatOverlay.AnimateCombatStart();
    }

    public IEnumerator SetupCombat()
    {
        combatController.BindCharacterData();
        foreach (CombatActor actor in combatController.ActorsInScene)
            actor.InitializeCombat();

        foreach (Func<IEnumerator> func in combatController.Hero.actionsStartCombat)
            yield return combatController.StartCoroutine(func.Invoke());

        yield return new WaitForSeconds(0.5f);

        EncounterDataCombat encounterData = combatController.encounterData;
        List<CardEffect>    startingEffects = encounterData.startingEffects;
        List<int>           startingTargets = encounterData.startEffectsTargets;
        int counter = 0;

        foreach (CardEffect e in startingEffects)
        {
            effectAndTarget = combatController.GetTargets(combatController.Hero, e, combatController.EnemiesInScene[startingTargets[counter++]]);

            for (int i = 0; i < effectAndTarget.effect.Times; i++)
                foreach (CombatActor actor in effectAndTarget.targets)
                    actor.RecieveEffectNonDamageNonBlock(effectAndTarget.effect);
        }

        for (int i = 0; i < encounterData.enemyData.Count; i++)
            foreach (CardEffect e in encounterData.enemyData[i].startingEffects)
                combatController.EnemiesInScene[i].RecieveEffectNonDamageNonBlock(e);

        combatController.animator.SetTrigger("SetupComplete");
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CombatControllerAnimatorInitialize : CombatControllerAnimator
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetRefs(animator);
        combat.StartCoroutine(SetupCombat());
        combat.combatOverlay.AnimateCombatStart();
    }

    public IEnumerator SetupCombat()
    {
        combat.BindCharacterData();
        foreach (CombatActor actor in combat.ActorsInScene)
            actor.InitializeCombat();

        foreach (Func<IEnumerator> func in combat.Hero.actionsStartCombat)
            yield return combat.StartCoroutine(func.Invoke());

        yield return new WaitForSeconds(0.5f);

        EncounterDataCombat encounterData = combat.encounterData;
        List<CardEffectInfo>    startingEffects = encounterData.startingEffects;
        List<int>               startingTargets = encounterData.startEffectsTargets;
        int counter = 0;

        foreach (CardEffectInfo e in startingEffects)
        {
            List<CombatActor> targets = combat.GetTargets(combat.Hero, e.Target, combat.EnemiesInScene[startingTargets[counter++]]);
            for (int i = 0; i < e.Times; i++)
                foreach (CombatActor actor in targets)
                    yield return combat.StartCoroutine(actor.RecieveEffectNonDamageNonBlock(e));
        } 

        for (int i = 0; i < encounterData.enemyData.Count; i++)
            foreach (CardEffectInfo e in encounterData.enemyData[i].startingEffects)
                yield return combat.StartCoroutine(combat.EnemiesInScene[i].RecieveEffectNonDamageNonBlock(e));

        combat.animator.SetTrigger("SetupComplete");
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CombatControllerAnimatorInitialize : CombatControllerAnimator
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetRefs(animator);
        CombatSystem.instance.StartCoroutine(SetupCombat());
        CombatSystem.instance.combatOverlay.AnimateCombatStart();
    }

    public IEnumerator SetupCombat()
    {
        CombatSystem.instance.BindCharacterData();
        foreach (CombatActor actor in CombatSystem.instance.ActorsInScene)
            actor.InitializeCombat();

        foreach (Func<IEnumerator> func in CombatSystem.instance.Hero.actionsStartCombat)
            yield return CombatSystem.instance.StartCoroutine(func.Invoke());

        yield return new WaitForSeconds(0.5f);

        EncounterDataCombat encounterData = CombatSystem.instance.encounterData;
        List<CardEffectInfo>    startingEffects = encounterData.startingEffects;
        List<int>               startingTargets = encounterData.startEffectsTargets;
        int counter = 0;

        foreach (CardEffectInfo e in startingEffects)
        {
            List<CombatActor> targets = CombatSystem.instance.GetTargets(CombatSystem.instance.Hero, e, CombatSystem.instance.EnemiesInScene[startingTargets[counter++]]);
            for (int i = 0; i < e.Times; i++)
                foreach (CombatActor actor in targets)
                    yield return CombatSystem.instance.StartCoroutine(actor.RecieveEffectNonDamageNonBlock(e));
        }

        for (int i = 0; i < encounterData.enemyData.Count; i++)
            foreach (CardEffectInfo e in encounterData.enemyData[i].startingEffects)
                yield return CombatSystem.instance.StartCoroutine(CombatSystem.instance.EnemiesInScene[i].RecieveEffectNonDamageNonBlock(e));

        CombatSystem.instance.animator.SetTrigger("SetupComplete");
    }


}

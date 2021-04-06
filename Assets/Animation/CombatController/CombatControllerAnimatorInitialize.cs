using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatControllerAnimatorInitialize : CombatControllerAnimator
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetRefs(animator);
        combatController.StartCoroutine(SetupCombat());
    }


    public IEnumerator SetupCombat()
    {
        combatController.BindCharacterData();
        Debug.Log("Starting combat");
        yield return new WaitForSeconds(0.5f);

        EncounterData encounterData = WorldSystem.instance.encounterManager.currentEncounter.encounterData;
        List<CardEffect> startingEffects    = encounterData.startingEffects;
        List<int> startingTargets           = encounterData.startEffectsTargets;

        foreach (CardEffect e in startingEffects)
        {
            List<CombatActor> targetedActors = new List<CombatActor>();
            if (e.Target == CardTargetType.Self)
                targetedActors.Add(combatController.Hero);
            else if (e.Target == CardTargetType.EnemySingle)
                targetedActors.Add(combatController.EnemiesInScene[startingTargets[startingEffects.IndexOf(e)]]);
            else if (e.Target == CardTargetType.EnemyAll)
                targetedActors.AddRange(combatController.EnemiesInScene);
            else if(e.Target == CardTargetType.All)
            {
                targetedActors.Add(combatController.Hero);
                targetedActors.AddRange(combatController.EnemiesInScene);
            }
            else if(e.Target == CardTargetType.EnemyRandom)
            {
                for(int i = 0; i < e.Times; i++)
                {
                    int index = Random.Range(0, combatController.EnemiesInScene.Count);
                    targetedActors.Add(combatController.EnemiesInScene[index]);
                }
            }


            targetedActors.ForEach(x => x.healthEffects.RecieveEffectNonDamageNonBlock(e));
        }

        for(int i = 0; i < encounterData.enemyData.Count; i++)
        {
            foreach (CardEffect e in encounterData.enemyData[i].startingEffects)
                combatController.EnemiesInScene[i].healthEffects.RecieveEffectNonDamageNonBlock(e);
        }

        combatController.animator.SetTrigger("SetupComplete");
    }


}

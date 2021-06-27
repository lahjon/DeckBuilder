using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatControllerAnimatorPlayerEnd : CombatControllerAnimator
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log("Entered Turn End");
        SetRefs(animator);
        CombatSystem.instance.StartCoroutine(EndPlayerturn());
    }


    public IEnumerator EndPlayerturn()
    {
        CombatSystem.instance.acceptEndTurn = false;
        CombatSystem.instance.ActiveActor.EffectsOnEndTurnBehavior();
        for (int i = 0; i < CombatSystem.instance.Hero.actionsEndTurn.Count; i++)
            yield return CombatSystem.instance.StartCoroutine(CombatSystem.instance.Hero.actionsEndTurn[i].Invoke());

        CombatSystem.instance.EndTurn();
        yield return new WaitForSeconds(0.05f);


        CombatSystem.instance.enemiesWaiting.Clear();
        CombatSystem.instance.EnemiesInScene.ForEach(x => CombatSystem.instance.enemiesWaiting.Enqueue(x));
        CombatSystem.instance.animator.SetTrigger("PlayerTurnEnded");
        yield return null;
    }


}

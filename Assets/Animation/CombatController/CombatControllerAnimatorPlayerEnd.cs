using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatControllerAnimatorPlayerEnd : CombatControllerAnimator
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log("Entered Turn End");
        SetRefs(animator);
        combatController.StartCoroutine(EndPlayerturn());
    }


    public IEnumerator EndPlayerturn()
    {
        combatController.acceptEndTurn = false;
        combatController.ActiveActor.EffectsOnEndTurnBehavior();
        for (int i = 0; i < combatController.ActiveActor.actionsEndTurn.Count; i++)
            yield return combatController.StartCoroutine(combatController.ActiveActor.actionsEndTurn[i].Invoke());
        combatController.EndTurn();
        yield return new WaitForSeconds(0.05f);


        combatController.enemiesWaiting.Clear();
        combatController.EnemiesInScene.ForEach(x => combatController.enemiesWaiting.Enqueue(x));
        combatController.animator.SetTrigger("PlayerTurnEnded");
        yield return null;
    }


}

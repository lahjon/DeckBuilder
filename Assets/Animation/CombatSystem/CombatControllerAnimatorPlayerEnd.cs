using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatControllerAnimatorPlayerEnd : CombatControllerAnimator
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log("Entered Turn End");
        SetRefs(animator);
        combat.StartCoroutine(EndPlayerturn());
    }


    public IEnumerator EndPlayerturn()
    {
        combat.acceptEndTurn = false;
        yield return combat.StartCoroutine(combat.Hero.EffectsOnEndTurnBehavior());
        for (int i = 0; i < combat.Hero.actionsEndTurn.Count; i++)
            yield return combat.StartCoroutine(combat.Hero.actionsEndTurn[i].Invoke());

        combat.EndTurn();
        yield return new WaitForSeconds(0.05f);


        combat.enemiesWaiting.Clear();
        combat.EnemiesInScene.ForEach(x => combat.enemiesWaiting.Enqueue(x));
        combat.animator.SetTrigger("PlayerTurnEnded");
        yield return null;
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatControllerAnimatorPlayerEnd : CombatControllerAnimator
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetRefs(animator);
        combatController.StartCoroutine(EndPlayerturn());
    }


    public IEnumerator EndPlayerturn()
    {
        combatController.acceptSelections = false;
        combatController.EndTurn();
        yield return new WaitForSeconds(0.05f);

        combatController.animator.SetTrigger("PlayerTurnEnded");
        yield return null;
    }


}

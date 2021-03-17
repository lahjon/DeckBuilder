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
        WorldSystem.instance.characterManager.characterVariablesUI.UpdateCharacterHUD();
        Debug.Log("Starting combat");
        yield return new WaitForSeconds(0.5f);
        Debug.Log("Combat has Started");
        //CombatController.InitializeCombat();
        combatController.animator.SetTrigger("SetupComplete");
    }


}

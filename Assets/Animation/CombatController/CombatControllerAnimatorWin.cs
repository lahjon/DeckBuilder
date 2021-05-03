using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatControllerAnimatorWin : CombatControllerAnimator
{
    CombatActor hero;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetRefs(animator);
        Debug.Log("Now you won and you are stuck here MOHAHAHAH");
    }


    public void CallTheseSometime()
    {
        combatController.CleanUp();
        WorldStateSystem.SetInReward(true);
    }


}

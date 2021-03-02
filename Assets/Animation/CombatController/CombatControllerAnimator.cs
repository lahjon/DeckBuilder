using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatControllerAnimator : StateMachineBehaviour
{
    public CombatController combatController;

    public void SetRefs(Animator animator)
    {
        if (combatController is null)
        {
            combatController = animator.GetComponent<CombatController>();
            
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatControllerAnimator : StateMachineBehaviour
{
    public static CombatSystem combat;
    public virtual void SetRefs(Animator animator)
    {
        if (combat == null)
            combat = CombatSystem.instance;
    }

}

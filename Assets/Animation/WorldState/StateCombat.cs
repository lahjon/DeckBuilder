using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateCombat : WorldStateAnimator
{
    KeyCode[] AlphaNumSelectCards = new KeyCode[] { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9, KeyCode.Alpha0 };
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Init(TransitionType.Normal, WorldState.Combat);

        world.toolTipManager.canvas.worldCamera = world.cameraManager.combatCamera;

        world.cameraManager.SwapToCombat();
        CombatSystem.instance.StartCombat();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        WorldStateSystem.SetInCombat(false);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        for (int i = 0; i < AlphaNumSelectCards.Length && i < CombatSystem.instance.Hand.Count; i++)
        {
            if (Input.GetKeyDown(AlphaNumSelectCards[i]) && WorldStateSystem.instance.currentWorldState == WorldState.Combat)
            {
                if (CombatSystem.instance.ActiveCard == CombatSystem.instance.Hand[i])
                {
                    Debug.Log("AlphaNum Deselct");
                    CombatSystem.instance.ActiveCard.OnMouseRightClick(false);
                }
                else
                    CombatSystem.instance.Hand[i].OnMouseClick();

                break;
            }
        }
        if (Input.GetKeyDown(KeyCode.Space) && CombatSystem.instance.acceptEndTurn == true)
        {
            CombatSystem.instance.PlayerInputEndTurn();
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardCombatAnimatorSelectedNoTarget : CardCombatAnimator
{
    public static float speed = 10f;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log("Selected OnStateEnter:" + Time.frameCount);
        SetRefs(animator);
        Debug.Log("Selected card " + card.cardData.name);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        //Debug.Log("Select OnStateUpdate:" + Time.frameCount);
        Vector3 targetPos = WorldSystem.instance.cameraManager.mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
        card.transform.position = Vector3.Lerp(card.transform.position, targetPos, speed*Time.deltaTime);
        card.transform.localEulerAngles = AngleLerp(card.transform.localEulerAngles, Vector3.zero, 1.2f*speed* Time.deltaTime);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log("Select OnStateExit:" + Time.frameCount);
        combatController.ResetSiblingIndexes();
        animator.SetBool("AllowMouseOver",false);
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CardCombatAnimatorDrawCard : CardCombatAnimator
{
    float speed; // 1 divided by speed == time needed
    public float breakTolerance = 3f;
    AnimationCurve curve;

    (Vector3 pos, Vector3 scale, Vector3 angles) TargetTransInfo;
    (Vector3 pos, Vector3 scale, Vector3 angles) StartTransInfo;

    private float time;


    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetRefs(animator);
        //Initiate
        card.MouseReact = false;
        card.selectable = true;
        card.transform.SetAsLastSibling();
        time = 0;
        curve = card.transitionCurveDraw;
        card.transform.position = CombatSystem.instance.txtDeck.transform.position;
        card.transform.localScale = Vector3.zero;
        card.transform.localEulerAngles = Vector3.zero;
        speed = 3.5f;

        //Reset any modified values from previous buffs
        CombatSystem.instance.SetCardCalcDamage(card);

        //Called every frame since we might draw more cards. 
        (Vector3, Vector3) tempTransInfo = CombatSystem.instance.GetPositionInHand(card);
        TargetTransInfo = (tempTransInfo.Item1, Vector3.one, tempTransInfo.Item2);

        StartTransInfo = TransSnapshot();
        card.selectable = true;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        time += speed * Time.deltaTime;


        CardLerp(StartTransInfo, TargetTransInfo, curve.Evaluate(time));

        if(Vector3.Distance(card.transform.localPosition, TargetTransInfo.pos) < breakTolerance || time > 1){
            if(time < 1) CardLerp(StartTransInfo, TargetTransInfo, time);
            animator.SetTrigger("DoneDrawing");
        }
        CombatSystem.instance.RefreshHandPositions(card);
    }




}


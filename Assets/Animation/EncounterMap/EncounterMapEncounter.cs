// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class EncounterMapEncounter : EncounterMapAnimator
// {
//     public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
//     {
//         base.Init();
//         gridManager.gridState = GridState.Encounter;
//     }
//     public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
//     {
//         if (WorldSystem.instance.gridManager.CheckClearCondition()) return;
//     }

//     public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
//     {

//     }
// }

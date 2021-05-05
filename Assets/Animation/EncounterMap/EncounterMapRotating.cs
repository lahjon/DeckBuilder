using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterMapRotating : EncounterMapAnimator
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.Init();
        
        gridManager.canvas.gameObject.SetActive(true);
        gridManager.gridState = GridState.Rotating;
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        gridManager.canvas.gameObject.SetActive(false);
        tile.spriteRenderer.sortingOrder -= 1;

        if (animator.GetBool("IsComplete"))
        {
            //Debug.Log("Stuff");
        }
        else
        {
            //Debug.Log("Nistus");
            tile.EndPlacement();
        }

        animator.SetBool("IsRotating", false);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Input.GetMouseButtonDown(1))
        {
            animator.SetBool("IsRotating", false);
        }
    }
}

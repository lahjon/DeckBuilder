using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CardCombatAnimatorPerformAction : CardCombatAnimator
{
    public ParticleSystem animationSystem;
    GameObject animationObject;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetRefs(animator);
        if (card.cardData.animationPrefab != null) // has animation
        {
            CreateAnimation();
            Debug.Log("Created Animation");
        }
        else // no animation
        {
            Debug.Log("No Animation");
            animationSystem = null;
        }
        card.transform.localEulerAngles = Vector3.zero;
        card.StartCoroutine(WaitForAnimation(animationSystem));
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        card.transform.localPosition = Vector3.Lerp(card.transform.localPosition, combatController.cardHoldPos.localPosition, 20*Time.deltaTime);
    }

    public void CreateAnimation()
    {
        if (card.cardData.animationPrefab != null)
        {
            GameObject child = card.cardData.animationPrefab.transform.GetChild(0).gameObject;
            animationObject = Instantiate(card.cardData.animationPrefab, card.transform.position, Quaternion.Euler(0, 0, 0)) as GameObject;
            animationSystem = child.GetComponent<ParticleSystem>();
            animationObject.transform.position = card.transform.position;
            animationSystem.Stop();
        }
    }


    IEnumerator WaitForAnimation(ParticleSystem particleSystem = null)
    {
        if (particleSystem != null)
        {
            if (!animationSystem.isPlaying)
                animationSystem.Play();
            yield return new WaitForSeconds(particleSystem.main.duration);
            DestroyImmediate(animationObject);
        }
        else
        {
            yield return new WaitForSeconds(0.4f);
        }

        card.animator.SetTrigger("Discarded");
    }




}


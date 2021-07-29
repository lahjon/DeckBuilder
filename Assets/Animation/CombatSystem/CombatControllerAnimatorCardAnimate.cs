using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatControllerAnimatorCardAnimate : CombatControllerAnimatorCard
{
    public ParticleSystem animationSystem;
    GameObject animationObject;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetRefs(animator);
        if (card.animationPrefab != null) // has animation
        {
            CreateAnimation();
            //Debug.Log("Created Animation");
        }
        else // no animation
        {
            //Debug.Log("No Animation");
            animationSystem = null;
        }

        layerName = CombatSystem.instance.ActiveActor == CombatSystem.instance.Hero ? "Resolve Card" : "EnemyCard";

        if (card.Block.Value != 0)
            nextState = "Block";
        else if (card.Damage.Value != 0)
            nextState = "Attack";
        else if (card.effectsOnPlay.Count != 0)
            nextState = "Effects";
        else
            nextState = "Activities & Discard";

        CombatSystem.instance.StartCoroutine(WaitForAnimation(animationSystem));
    }

    public void CreateAnimation()
    {
        if (card.cardData.animationPrefab != null)
        {
            GameObject child = card.cardData.animationPrefab.transform.GetChild(0).gameObject;
            animationObject = Instantiate(card.cardData.animationPrefab,CombatSystem.instance.cardHoldPos) as GameObject;
            animationSystem = child.GetComponent<ParticleSystem>();
            animationObject.transform.position = CombatSystem.instance.cardHoldPos.position;
            animationSystem.Stop();
        }
    }


    IEnumerator WaitForAnimation(ParticleSystem particleSystem = null)
    {
        if (particleSystem != null)
        {
            animationSystem.Play();
            yield return new WaitForSeconds(particleSystem.main.duration);
            DestroyImmediate(animationObject);
        }
        else
        {
            yield return new WaitForSeconds(0.4f);
        }


        CombatSystem.instance.animator.Play(nextLayerState);
    }



}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

        if (combat.actorTurn == CombatActorTypes.Hero)
            layerName = "Resolve Card";
        else if(combat.actorTurn == CombatActorTypes.Enemy)
            layerName = "EnemyCard";
        else    
            layerName = "CompanionCard";
            
        if (card.Blocks.Any())
            nextState = "Block";
        else if (card.Attacks.Any())
            nextState = "Attack";
        else if (card.effectsOnPlay.Any())
            nextState = "Effects";
        else
            nextState = "Activities & Discard";

        combat.StartCoroutine(WaitForAnimation(animationSystem));
    }

    public void CreateAnimation()
    {
        if (card.cardData.animationPrefab != null)
        {
            GameObject child = card.cardData.animationPrefab.transform.GetChild(0).gameObject;
            animationObject = Instantiate(card.cardData.animationPrefab,combat.cardHoldPos) as GameObject;
            animationSystem = child.GetComponent<ParticleSystem>();
            animationObject.transform.position = combat.cardHoldPos.position;
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
            yield return new WaitForSeconds(0.2f);
        }

        if(!combat.animator.GetBool("HasWon"))
            combat.animator.Play(nextLayerState);
    }



}

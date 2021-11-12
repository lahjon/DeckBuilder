using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardCombatAnimatorDiscarding : CardCombatAnimator
{
    public ParticleSystem animationSystem;
    GameObject animationObject;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetRefs(animator);

        if (card.animator.GetBool("Resolved"))
        {
            if (card.HasProperty(CardSingleFieldPropertyType.Exhaust))
                CombatSystem.instance.Hero.ExhaustCard(card);
            else if (card.cardType == CardType.Oath)                    // Oath
                card.StartBezierAnimation(2);
            else                                                        // Discard
            {
                CombatSystem.instance.Hero.DiscardCardNoTrigger(card);
                card.StartBezierAnimation(1);
            }
        }
        else
        {
            if (card.animator.GetBool("ToDeckPileOverride"))
            {
                card.StartBezierAnimation(0);
                card.animator.SetBool("ToDeckPileOverride", false);
            }
            else
            {
                CombatSystem.instance.Hero.DiscardCardNoTrigger(card);
                card.StartBezierAnimation(1);
            }
        }

        card.animator.SetBool("Resolved",false);
    }

}


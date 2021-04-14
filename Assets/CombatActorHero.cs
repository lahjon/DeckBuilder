using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CombatActorHero : CombatActor
{
    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = WorldSystem.instance.characterManager.character.characterData.artwork;
    }


    void OnEnable()
    {

    }

    public override void DiscardCard(Card card)
    {
        ((CardCombat)card).animator.SetTrigger("Discarded");
        base.DiscardCard(card);
    }

    public override void CardResolved(Card card)
    {
        ((CardCombat)card).animator.SetTrigger("Resolved");
        base.DiscardCard(card);
    }

    public void ClearAllEffects()
    {
        List<RuleEffect> effects = effectTypeToRule.Values.ToList();

        foreach(RuleEffect effect in effects)
        {
            Destroy(healthEffectsUI.effectToDisplay[effect.type].gameObject);
            effect.Dismantle();
        }
    }
}

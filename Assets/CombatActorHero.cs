using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CombatActorHero : CombatActor
{
    public override void SetupAlliesEnemies()
    {
        allies.Add(this);
        enemies.AddRange(CombatSystem.instance.EnemiesInScene);
    }

    void OnEnable()
    {
        spriteRenderer.sprite = WorldSystem.instance.characterManager.character.characterData.artwork;
        collision = gameObject.AddComponent<BoxCollider2D>();
    }

    public override void DiscardCard(Card card)
    {
        ((CardCombat)card).animator.SetTrigger("Discarded");
        base.DiscardCard(card);
    }

    public override void CardResolved(Card card)
    {
        if (card.exhaust) Destroy(card.gameObject);
        else
        {
            ((CardCombat)card).animator.SetTrigger("Resolved");
            base.DiscardCard(card);
        }
    }

    public void ClearAllEffects()
    {
        List<CardEffect> effects = effectTypeToRule.Values.ToList();
        effectTypeToRule.Clear();

        foreach(CardEffect effect in effects)
        {
            if (healthEffectsUI.effectToDisplay[effect.type].gameObject != null)
                Destroy(healthEffectsUI.effectToDisplay[effect.type].gameObject);

            effect.Dismantle();
        }

        healthEffectsUI.effectToDisplay.Clear();
    }
}

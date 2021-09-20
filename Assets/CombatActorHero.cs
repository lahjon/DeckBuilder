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

    private void Awake()
    {
        deck = new ListEventReporter<Card>(EventManager.DeckCountChanged);
        discard = new ListEventReporter<Card>(EventManager.DiscardCountChanged);        
    }

    void Start()
    {
        spriteRenderer.sprite = WorldSystem.instance.characterManager.character.characterData.artwork;
        collision = gameObject.AddComponent<BoxCollider2D>();
    }

    public override void DiscardCard(Card card)
    {
        if (card is CardCombat cc)
        {
            CombatSystem.instance.Hand.Remove(cc);
            cc.animator.SetTrigger("Discarded");
        }
        base.DiscardCard(card);
    }

    public override void CardResolved(Card card)
    {
        if (card.HasProperty(CardSingleFieldPropertyType.Exhaust)) 
            ExhaustCard(card);
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

    public override void RecalcDamage()
    {
        CombatSystem.instance.RecalcAllCardsDamage();
    }
}

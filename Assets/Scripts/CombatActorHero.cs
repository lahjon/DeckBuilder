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
        spriteRenderer.sprite = WorldSystem.instance.characterManager.characterData.artwork;
        collision = gameObject.AddComponent<BoxCollider2D>();
    }

    public void DiscardCardNoTrigger(CardCombat card)
    {
        base.DiscardCard(card);
    }

    public override void DiscardCard(Card card)
    {
        if (card is CardCombat cc)
        {
            CombatSystem.instance.Hand.Remove(cc);
            cc.animator.SetTrigger("Discarded");
        }
    }

    public override void CardResolved(Card card)
    {
        ((CardCombat)card).animator.SetBool("Resolved",true);
    }

    public void ClearAllEffects()
    {
        List<StatusEffect> effects = effectTypeToRule.Values.ToList();
        effectTypeToRule.Clear();

        foreach(StatusEffect effect in effects)
            effect.Dismantle();
    }

    public override void RecalcDamage()
    {
        CombatSystem.instance.RecalcAllCardsDamage();
    }

    public override int GetStat(StatType stat) => CharacterStats.stats[stat];
}

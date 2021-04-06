using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
}

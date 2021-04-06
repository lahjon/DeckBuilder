using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatActor : MonoBehaviour
{
    public HealthEffects healthEffects;
    public CombatController combatController;

    public List<Func<float, float>> dealAttackMods = new List<Func<float, float>>();
    public List<Func<float, float>> takeAttackMods = new List<Func<float, float>>();

    public List<Func<IEnumerator>> actionsNewTurn = new List<Func<IEnumerator>>();
    public List<Func<IEnumerator>> actionsEndTurn = new List<Func<IEnumerator>>();

    public List<Func<CombatActor, IEnumerator>> onAttackRecieved = new List<Func<CombatActor,IEnumerator>>();

    public List<Card> deck = new List<Card>();
    public List<Card> discard = new List<Card>();

    public void Awake()
    {
        actionsNewTurn.Add(RemoveAllBlock);
    }

    public IEnumerator RemoveAllBlock()
    {
        healthEffects.RemoveAllBlock();
        yield return new WaitForSeconds(1);
    }

    public void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            Card temp = deck[i];
            int index = UnityEngine.Random.Range(i, deck.Count);
            deck[i] = deck[index];
            deck[index] = temp;
        }
    }

    public virtual void DiscardCard(Card card)
    {
        discard.Insert(0,card);
    }

    public virtual void AddToDeck(Card card)
    {
        deck.Insert(0, card);
    }
}

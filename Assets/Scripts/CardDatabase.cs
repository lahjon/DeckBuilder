using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCardDatabase", menuName = "Database/Card Database")]
public class CardDatabase : ScriptableObject
{
    public List<Card> allCards;

    public void UpdateDatabase(List<Card> allCardsUpdated)
    {
        allCards = allCardsUpdated;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCardDatabase", menuName = "Database/Card Database")]
public class CardDatabase : ScriptableObject
{
    public List<CardData> allCards;

    public void UpdateDatabase(List<CardData> allCardsUpdated)
    {
        allCards = allCardsUpdated;
    }
}
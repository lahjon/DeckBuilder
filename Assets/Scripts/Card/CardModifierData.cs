﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card Modifier", menuName = "CardGame/CardModifier")]
public class CardModifierData : ScriptableObject
{
    public string id;
}


[System.Serializable] 
public class CardWrapper
{
    public CardWrapper(string anId, List<string> allCardModifiers)
    {
        cardId = anId;
        cardModifiersId = allCardModifiers;
    }
    public CardWrapper(string anId)
    {
        cardId = anId;
    }
    public string cardId;
    public int timesUpgraded;
    public List<string> cardModifiersId;
}
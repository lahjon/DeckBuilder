using System.Collections;
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
    public CardWrapper(string anId, List<string> allCardModifiers, int anIdx)
    {
        cardId = anId;
        cardModifiersId = allCardModifiers;
        idx = anIdx;
    }
    public CardWrapper(string anId, int anIdx)
    {
        cardId = anId;
        idx = anIdx;
    }
    public string cardId;
    public int timesUpgraded;
    public int idx;
    public List<string> cardModifiersId = new List<string>();
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card Functionality", menuName = "CardGame/CardFunctionalityData")]
public class CardFunctionalityData : ScriptableObject
{
    public string id;

    public List<(CardSingleFieldPropertyType prop, bool val)> singleFieldProperties = new List<(CardSingleFieldPropertyType prop, bool val)>();
    public List<CardEffectCarrierData> effects = new List<CardEffectCarrierData>();
    public List<CardActivitySetting> activities = new List<CardActivitySetting>();

    public void ResetFunctionality()
    {
        singleFieldProperties.Clear();
        effects.Clear();
        activities.Clear();
    }
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card Functionality", menuName = "CardGame/CardFunctionalityData")]
public class CardFunctionalityData : ScriptableObject
{
    public string id;

    public List<CardSingleFieldPropertyTypeWrapper> singleFieldProperties = new List<CardSingleFieldPropertyTypeWrapper>();
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
public struct CardSingleFieldPropertyTypeWrapper
{
    //public string Name => prop.ToString();
    public string Name;
    public CardSingleFieldPropertyTypeWrapper(CardSingleFieldPropertyType prop, bool val)
    {
        this.prop = prop;
        this.val = val;
        Name = prop.ToString() + ", " + val.ToString();
    }

    public CardSingleFieldPropertyType prop;
    public bool val;
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
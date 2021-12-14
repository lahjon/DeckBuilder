using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayableCharacterData", menuName = "CardGame/PlayableCharacterData")]
public class PlayableCharacterData : CharacterData
{
    public GameObject artworkAnimated;
    public List<StatStruct> stats = new List<StatStruct>()
                                    {
                                        new StatStruct(0, StatType.Health),
                                        new StatStruct(0, StatType.Power), 
                                        new StatStruct(0, StatType.Endurance), 
                                        new StatStruct(0, StatType.Wit), 
                                        new StatStruct(0, StatType.Amplitude), 
                                        new StatStruct(0, StatType.Syphon), 
                                        new StatStruct(0, StatType.DraftAmount),
                                        new StatStruct(0, StatType.OptionalEnergyMax),
                                        new StatStruct(0, StatType.OptionalEnergyTurn),
                                        new StatStruct(0, StatType.MaxCardsOnHand)
                                    };
    public CharacterClassType classType;
    public string startingItem;
    public bool unlocked;
}

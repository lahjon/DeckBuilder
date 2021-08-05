using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayableCharacterData", menuName = "CardGame/PlayableCharacterData")]
public class PlayableCharacterData : CharacterData
{
    public GameObject artworkAnimated;

    public List<Stat> stats = new List<Stat>()
                                    {
                                        new Stat(0, StatType.Strength), 
                                        new Stat(0, StatType.Endurance), 
                                        new Stat(0, StatType.Wit), 
                                        new Stat(0, StatType.Energy), 
                                        new Stat(0, StatType.Health)
                                    };
    public CharacterClassType classType;
    public string startingItem;
    public bool unlocked;


    void Init()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Playable Character Data", menuName = "CardGame/PlayableCharacterData")]
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
    public int blockModifier;
    public int drawCardsAmount;
    public int energy;
    public int maxHp;
    public int level;
    public CharacterClassType classType;
    public bool unlocked;


    public CardDatabase startingDeck;

    void Init()
    {
        
    }
}

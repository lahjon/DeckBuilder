
public enum CardType
{
Attacker, 
Defender
};

public enum CardRarity
{
Starting,
Common, 
Uncommon, 
Rare
};

public enum CharacterType
{
Brute, 
Rogue
};

public enum EffectType
{
    Damage,
    Poison,
    Block,
    Weak,
    Vurnerable
}

public enum CharacterStats
{
    Strength,
    Cunning,
    Speed,
    Endurance,
    Wisdom
}

public enum WorldStates
{
    MainMenu,
    Combat,
    Overworld,
    Menu,
    Town,
    Transition
}

public enum EncounterTypes
{
    Start,
    RandomEvent,
    Shop,
    NormalCombat,
    EliteCombat,
    BossCombat,
    Transition
}
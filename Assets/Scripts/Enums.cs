
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

public enum CharacterEffect
{
Gold, 
Artifact,
Card,
Companion
};

public enum EffectType
{
    Damage,
    Poison,
    Block,
    Weak,
    Vurnerable
}

public enum CharacterStat
{
    Strength,
    Cunning,
    Speed,
    Endurance,
    Wisdom
}

public enum WorldState
{
    MainMenu,
    Combat,
    Overworld,
    Menu,
    Town,
    Transition
}

public enum EncounterType
{
    Start,
    RandomEvent,
    Shop,
    NormalCombat,
    EliteCombat,
    BossCombat,
    Transition
}
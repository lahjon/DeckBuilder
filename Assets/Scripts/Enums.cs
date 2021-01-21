
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

public enum Rarity
{
    Common, 
    Uncommon, 
    Rare
};

public enum CardTargetType
{
    Single, 
    Multiple
}
public enum EncounterDifficulty
{
    Easy, 
    Normal,
    Hard,
    Challenging,
    Impossible
}

public enum CharacterType
{
    Brute, 
    Rogue,
    Enemy,
    EnemyElite, 
    EnemyBoss
};

public enum CharacterVariables
{
    Health, 
    Gold
};

public enum EncounterOutcomeType
{
    Stat,
    Health,
    Gold, 
    Artifact,
    CardSpecified,
    CardChoose,
    CompanionSpecified,
    CompanionChoose
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
    Shop,
    Display,
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
    CombatNormal,
    CombatElite,
    CombatBoss,
    Transition
}

public enum EncounterValue
{
    Positive, 
    Negative, 
    Neutral
}


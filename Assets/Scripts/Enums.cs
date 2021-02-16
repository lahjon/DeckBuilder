
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
    ALL,
    Random
}
public enum CharacterClass
{
    None,
    Brute, 
    Rogue,
    Enemy,
    EnemyElite, 
    EnemyBoss
};

public enum CharacterVariables
{
    MaxHealth, 
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
    Vurnerable,
    Barricade
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
    Event,
    Reward,
    Shop,
    Display,
    Combat,
    Overworld,
    Menu,
    Town,
    TownHall,
    Transition,
    Dead
}

public enum BuildingType
{
    Shop, 
    Tavern, 
    Church, 
    TownHall,
    Barracks,
    Leave
}

public enum EncounterType
{
    OverworldStart,
    OverworldRandomEvent,
    OverworldShop,
    OverworldCombatNormal,
    OverworldCombatElite,
    OverworldCombatBoss,
    OverworldTransition,
    TownEvent
}
public enum EncounterValue
{
    Positive, 
    Negative, 
    Neutral
}

public enum EnemyBehavior
{
    Random, 
    Sequential
}
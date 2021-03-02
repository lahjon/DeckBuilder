public static class EnumExtenstions
{
    public static string GetDescription(this EffectType type)
    {
        if (type == EffectType.Barricade)
            return $"<b>Barricade</b>\nBlock is not removed at the start of your turn";
        else if (type == EffectType.Vurnerable)
            return $"<b>Vurnerable</b>\nAn actor with Vurnerable recieves 25% more attack damage";
        else
            return $"<b>{type.ToString()}</b>\nSeth is a very lazy man and has not written a tip for this effect. <i>(Also Fredrik smokes dicks.)</i>";
    }
}

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
    Self,
    EnemySingle,
    EnemyAll,
    EnemyRandom,
    All
}
public enum CharacterClassType
{
    None,
    Brute, 
    Rogue,
    Splicer,
    Beastmaster,
};

public enum EncounterEventType
{
    None,
    Leave, 
    Combat, 
    CardSpecific,
    CardRandom,
    NewMap,
    NewEvent
}

public enum GameEventStatus
{
    NotStarted,
    Started, 
    Done
}

public enum CharacterAbility
{
    Objective, 
    Mission
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

// public enum CharacterStat
// {
//     Strength,
//     Cunning,
//     Speed,
//     Endurance,
//     Wisdom
// }

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
    Building,
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
    Leave,
    Any
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

public enum CardActivityType
{
    DrawCard,
    AddCardToDeck
}
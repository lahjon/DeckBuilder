public static class EnumExtenstions
{
    public static string GetDescription(this EffectType type)
    {
        switch (type)
        {
            case EffectType.Barricade:
                return $"<b>Barricade</b>\nBlock is not removed at the start of your turn";
            case EffectType.Vurnerable:
                return $"<b>Vurnerable</b>\nRecieve 25% more attack damage";
            case EffectType.Thorns:
                return $"<b>Thorns</b>\nDeal damage back when attacked";
            default:
                return $"<b>{type.ToString()}</b>\nSeth is a very lazy man and has not written a tip for this effect. <i>(Also Fredrik smokes dicks.)</i>";
        }
    }

    public static RuleEffect GetRuleEffect(this EffectType type)
    {
        switch (type)
        {
            case EffectType.Barricade:
                return new RuleEffectBarricade();
            case EffectType.Weak:
                return new RuleEffectWeak();
            case EffectType.Vurnerable:
                return new RuleEffectVulnerable();
            case EffectType.Thorns:
                return new RuleEffectThorns();
            case EffectType.Splice:
                return new RuleEffectSplice();
            case EffectType.Poison:
                return new RuleEffectPoison();
            default:
                return null;
        }
    }
}

public enum Rarity
{
    None,
    Starting,
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

public enum DialogueParticipant
{
    None,
    Brute, 
    Rogue,
    Splicer,
    Beastmaster,
    Shopkeeper,
    Major
}

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

public enum StatType
{
    Health,     
    Strength,   // attack
    Endurance,  // block
    Wit,        // draw cards amount
    Energy
}

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
    Barricade,
    Thorns,
    Splice
}

public enum OverlayState
{
    None,
    Transition,
    DeathScreen,
    EscapeMenu,
    Display,
    CharacterSheet
}

public enum WorldState
{
    MainMenu,
    Event,
    Reward,
    Shop,
    Combat,
    WorldMap,
    Town,
    Building,
    Dialogue,
    Overworld,
    Transition,
    Deathscreen,
    Cutscene,
}

public enum TransitionType
{
    None,
    Normal,
    DeathScreen, 
    EnterAct

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
    AddCardToDeck,
    Splice,
    ExhaustDiscard
}
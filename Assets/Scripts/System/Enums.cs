using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public static class EnumExtenstions
{
    public static string GetToolTip(this EffectType type)
    {
        switch (type)
        {
            case EffectType.Barricade:
                return $"<b>Barricade</b>\nBlock is retained between turns";
            case EffectType.Vulnerable:
                return $"<b>Vurnerable</b>\nReceive 25% more attack damage";
            case EffectType.Thorns:
                return $"<b>Thorns</b>\nDeal damage back when attacked";
            case EffectType.Weak:
                return $"<b>Weak</b>\nDeals 25% less damage";
            case EffectType.Challenge:
                return $"<b>Challenge</b>\nChallenged actors deal double damage against eachother";
            case EffectType.Poison:
                return $"<b>Poison</b>\nLose life at the end of the turn";
            case EffectType.Strength:
                return $"<b>Strength</b>\nIncreases the value of all attacks";
            case EffectType.StrengthTemp:
                return $"<b>StrengthTemp</b>\nTemporarily changes strength during your turn";
            case EffectType.Block:
                return $"<b>Block</b>\nBlock negates incoming attack damage until the end of the round";
            case EffectType.Confused:
                return $"<b>Confused</b>\nCards with a single target will land on a random enemy";
            case EffectType.Splice:
                return $"<b>Splice</b>\nThe next splice card you play will meld with the top splice card in discard";
            case EffectType.Envenom:
                return $"<b>Envenom</b>\nUnblocked damage will deal poison to the target.";
            case EffectType.Dazed:
                return $"<b>Dazed</b>\nThe next time you would gain block this turn, gain 0 block instead.";
            case EffectType.Spiked:
                return $"<b>Spiked</b>\nUntil your next turn, retailate when attacked.";
            default:
                return $"<b>{type}</b>\nSeth is a very lazy man and has not written a tip for this effect. <i>(Also Fredrik smokes dicks.)</i>";
        }
    }

    public static string GetDescription(this Profession type)
    {
        switch (type)
        {
            case Profession.Base:
                return "<b>Base</b>\nThis is the base profession.";
            default:
                return string.Format("<b>{0}</b>\nMissing Description!", type.ToString());
        }
    }

    public static bool IsReward(this EncounterEventChoiceEffect effect)
    {
        if ((int)effect >= 20) return true;
        else return false;
    }
    

    public static CardEffect GetRuleEffect(this EffectType type)
    {
        switch (type)
        {
            case EffectType.Barricade:
                return new CardEffectBarricade();
            case EffectType.Weak:
                return new CardEffectWeak();
            case EffectType.Vulnerable:
                return new CardEffectVulnerable();
            case EffectType.Thorns:
                return new CardEffectThorns();
            case EffectType.Splice:
                return new CardEffectSplice();
            case EffectType.Poison:
                return new CardEffectPoison();
            case EffectType.Challenge:
                return new CardEffectChallenge();
            case EffectType.Strength:
                return new CardEffectStrength();
            case EffectType.StrengthTemp:
                return new CardEffectStrengthTemp();
            case EffectType.Confused:
                return new CardEffectConfused();
            case EffectType.Envenom:
                return new CardEffectEnvenom();
            case EffectType.Dazed:
                return new CardEffectDazed();
            case EffectType.Spiked:
                return new CardEffectSpiked();
            default:
                return null;
        }
    } 

}

public enum EquipmentType
{
    None, 
    Head,
    Chest,
    Hands,
    Legs, 
    Feet
}

public enum ConditionType
{
    None,
    KillEnemy,
    ClearTile,
    EnterBuilding,
    WinCombat,
    CardsPlayedAtLeast,
    CardsPlayedAtMost,
    LastCardPlayedTurnType,
    Discharge,
    KillBoss,
    StoryTileCompleted,
    EncounterDataCompleted,
    EncounterCompleted,
    StorySegmentCompleted
}

public enum Rarity
{
    None = 0,
    Starting = 1,
    Common= 11,
    Uncommon = 12,
    Rare = 13,
    Epic = 14
};

public enum ZoomState
{
    Inner, 
    Mid, 
    Outer
}

public enum CardLocation
{
    Deck,
    Hand,
    Discard,
    Exhaust,
    InProcess
}

public enum TileState
{
    Inactive,
    Placement,
    InactiveHighlight,
    Current,
    Completed,
    Animation
}

public enum GridState
{
    Creating,
    Animating,
    Placing,
    Play,
    Panning,
    Complete,
}

public enum CardType
{
    Attack, // deal damage
    Disorder, // do negative effect
    Order, // do positive effect, best judgement
    Oath, // do permanent effect, best judgement
    Burden, // classless, only exists in combat
    Torment // classless, permanent bad card
}

public enum CardTargetType /*OBS order matters for splice! ^*/
{
    Self,
    AlliesExclSelf,
    AlliesInclSelf,
    EnemySingle,
    EnemyAll,
    EnemyRandom,
    All
}
public enum CharacterClassType
{
    None = 0,
    Berserker = 1,
    Rogue = 2,
    Splicer = 3,
    Beastmaster = 4
};

public enum CardClassType
{
    None = 0,
    Berserker = 1,
    Rogue = 2,
    Splicer = 3,
    Beastmaster = 4,
    Colorless,
    Burden,
    Torment,
    Enemy
}

public enum CardSingleFieldPropertyType
{
    Fortify = -4,
    Immediate = -3,
    Unplayable = -2,
    Unstable = -1,
    Exhaust = 0
}

public enum CardLinkablePropertyType
{
    None,
    Handsize,
    NrCardsDeck,
    NrCardsDiscard,
    EnergyAvailable,
    CardEnergySpent,
    CountPlayedCardsSameName
}

public enum CardComponentExecType
{
    OnPlay,
    OnDraw
}

public enum RewardType
{
    // overworld reward
    None = 0,
    Gold = 1,
    Card = 2,
    Heal = 3,
    Shard = 4,
    // permanent reward
    UnlockCard = 50,
    Item = 51,
    Artifact = 52,
    Perk = 53
}

public enum CurrencyType
{
    None, 
    Gold, 
    Shard
}

public enum DialogueParticipant
{
    None,
    Player,
    Berserker,
    Rogue,
    Splicer,
    Beastmaster,
    Shopkeeper,
    Major
}

public enum EncounterEventChoiceOutcome
{
    None,
    Leave,
    Combat,
    NewEvent
}

public enum WorldEncounterType
{
    None,
    Main,
    Repeatable,
    Special
}

public enum ScenarioSegmentType
{
    ClearTiles,
    ClearEncounters,
    FindTheEncounters
}

public enum EncounterEventChoiceEffect
{
    LifeCurrent,
    LifeMax,
    Gold,

    // reward effects
    GetCards = 20,
    SelectionCards,
    Artifact
}

public enum GameEventType
{
    None,
    Custom,                     // parm = name of event | value = whatever needed by your custom event
    HighlightBuilding,          // parm = string (BuildingType) | value = bool, ex: (0, true)
    GetReward,                  // parm = string (RewardType) | value = id (int), ex1: (Item, 0)
    TriggerReward,               // parm = string (WorldState) ändra, orkar inte göra det nu
    ToggleWorldMap,             // parm = None | value = bool, ex: (true)
    UnlockScenario              // parm = None | value = idx (int), ex: (0)
}
public enum ItemEffectType
{
    None = 0,
    Custom,
    AddStat,
    AddCombatEffect,
    Heal
}



public enum EncounterHexStatus
{
    Visited,
    Selectable,
    Unreachable,
    Idle
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

public enum EncounterRoadStatus
{
    Idle,
    Traversed,
    Unreachable
} 

public enum EffectType
{
    Damage,
    Poison,
    Block,
    Weak,
    Vulnerable,
    Barricade,
    Thorns,
    Splice,
    Challenge,
    Strength,
    StrengthTemp,
    Confused,
    Envenom,
    Dazed,
    Spiked
}

public enum OverlayState
{
    None,
    Transition,
    Dialogue,
    DeathScreen,
    EscapeMenu,
    Display,
    RewardScreen,
    CharacterSheet
}

public enum WorldState
{
    MainMenu,
    OverworldEvent,
    OverworldBonfire,
    OverworldShop,
    CombatReward,
    EventReward,
    TownReward,
    TownShop,
    Combat,
    WorldMap,
    WorldMapAnimation,
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
    Town,
    DeathScreen,
    EnterMap

}

public enum ScenarioDifficulty
{
    None, 
    Easy, 
    Moderate,
    Hard,
    Impossible
}

public enum EnemyType
{
    Minion,
    Normal,
    Elite,
    Boss
}
public enum FormationType
{
    Single,
    Duo,
    Trio,
    AlternatingFive,
    HoveringAroundLeader
}

public enum ProgressionGoalType
{
    KillEnemy,
    EnterBuilding,
}

public enum BuildingType
{
    Shop = 0,           // buy stuff for permanent progress
    Tavern = 1,         // hopefully used for companions
    Church = 2,         // no idea, maybe remove
    TownHall = 3,       // manage town and upgrade town, adding new buildings, story stuff, quests etc
    Scribe = 4,         // manage the side deck, upgrade cards etc
    Barracks = 5,       // manage characters, swap proffession
    Jeweler = 6,        // manage tokens
    Any = 20
}

public enum OverworldEncounterType
{
    None,
    Exit,
    Choice,
    Shop,
    CombatNormal = 4,
    CombatElite = 5,
    CombatBoss = 6,
    Cave,
    Bonfire,
    Story,
    Start
}

public enum CombatEncounterType
{
    Normal = 4,
    Elite = 5,
    Boss = 6
}

public enum TileEncounterType
{
    None,
    Treasure,
    Elite,
    Special
}

public enum TileBiome
{
    None,
    Forest, 
    Snow,
    Desert
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

public enum CardHighlightType
{
    None,
    Selected,
    Playable,
    PlayableSpecial
}

public enum CardActivityType
{
    DrawCard,
    AddCardToCombat,
    Splice,
    ExhaustDiscard,
    CombatCostChange,
    SetRandomBroken,
    Heal,
    ModifyEnergy,
    DrawDiscard,
    DualWield
}

public enum CombatActorType
{
    Hero,
    Companion,
    Enemy
}

public enum CombatStateType
{
    Idle, 
    NotIdle
}

public enum Profession
{
    Base,
    Berserker1 = 11,
    Berserker2 = 12,
    Berserker3 = 13,
    Rogue1 = 21,
    Rogue2 = 22,
    Rogue3 = 23,
    Splicer1 = 31,
    Splicer2 = 32,
    Splicer3 = 33,
    Beastmaster1 = 41,
    Beastmaster2 = 42,
    Beastmaster3 = 43,
}

public enum EncounterEventLayoutType
{
    NoImage,
    ImageRight,
    ImageLeft
}

public enum TileType
{
    None,
    Forest,
    Cave,
    Plains,
    BanditCamp,
    Town,
    Caravan
}

public enum EnergyType
{
    Standard,
    Rage
}

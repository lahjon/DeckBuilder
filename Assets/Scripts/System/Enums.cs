using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public static class EnumExtenstions
{
    public static string GetDescription(this ProfessionType type)
    {
        switch (type)
        {
            case ProfessionType.Base:
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

public enum Rarity
{
    None = 0,
    Starting = 1,
    Common= 11,
    Uncommon = 12,
    Rare = 13,
    Epic = 14
};

public enum EncounterTag
{
    None,
    General,
    Tinkerbots
}

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
    Inescapable = -5,
    Fortify = -4,
    Immediate = -3,
    Unplayable = -2,
    Unstable = -1,
    Exhaust = 0,
}

public enum CardLinkablePropertyType
{
    None,
    Handsize,
    NrCardsDeck,
    NrCardsDiscard,
    EnergyAvailableStandard,
    CardEnergySpentStandard,
    CountPlayedCardsSameName,
    EnergyAvailableRage,
    CardEnergySpentRage
}

public enum CardComponentExecType
{
    OnPlay,
    OnDraw
}

public enum RewardCombatType
{
    None = 0,
    Gold = 1,
    Card = 2,
    Heal = 3,
    Shard,
    Artifact,
    Ember
}

public enum RewardNormalType
{
    None = 0,
    UnlockCard = 1,
    Ability = 2,
    Perk = 4,
    Shard,
    FullEmber
}

public enum CostType
{
    FullyUpgraded,
    OutOfStock
}

public enum CurrencyType
{
    None, 
    Gold, 
    Shard,
    ArmorShard,
    Ember,
    FullEmber
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

public enum EncounterHexStatus
{
    Visited,
    Selectable,
    Unreachable,
    Idle
}
public enum StatType
{
    None = 0,

    // main stats
    Health,                     // HP
    Power,                      // damage to attacks
    Wit,                        // cards to draw each turn
    Amplitude,                  // energy max
    Syphon,                     // energy / turn
    Endurance,                  // armor / block ??

    // hidden stats
    OptionalEnergyMax = 10,
    OptionalEnergyTurn,
    DraftAmount,
    MaxCardsOnHand
    
}

public enum EncounterRoadStatus
{
    Idle,
    Traversed,
    Unreachable
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

public enum ModifierType
{
    Upgrade,
    UpgradePermanent,
    Cursed
}

public enum WorldState
{
    MainMenu,
    OverworldEvent,
    OverworldBonfire,
    OverworldBlacksmith,
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
    BuildingScribe,
    BuildingTownHall,
    BuildingBarracks,
    BuildingJeweler,
    BuildingTavern,
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
    Any = 20,
    None
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
    Start,
    Blacksmith
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
    ModifyLife,
    ModifyEnergy,
    DrawDiscard,
    DualWield,
    EmptySavedEnergy,
    EN_AddCardModifier
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

public enum ProfessionType
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

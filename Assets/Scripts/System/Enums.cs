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
            default:
                return $"<b>{type.ToString()}</b>\nSeth is a very lazy man and has not written a tip for this effect. <i>(Also Fredrik smokes dicks.)</i>";
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
            default:
                return null;
        }
    } 

    public static void Invoke(this OverworldEncounterType type)
    {
        switch (type)
        {
            case OverworldEncounterType.CombatNormal:
            case OverworldEncounterType.CombatElite:
            case OverworldEncounterType.CombatBoss:
                Debug.Log(CombatSystem.instance);
                CombatSystem.instance.encounterData = DatabaseSystem.instance.GetRndEncounterCombat(type);
                WorldStateSystem.SetInCombat(true);
                break;
            case OverworldEncounterType.Shop:
                WorldStateSystem.SetInOverworldShop(true);
                break;
            case OverworldEncounterType.RandomEvent:
                WorldSystem.instance.uiManager.encounterUI.encounterData = DatabaseSystem.instance.GetRndEncounterEvent();
                WorldStateSystem.SetInEvent(true);
                break;
            case OverworldEncounterType.Exit:
                WorldSystem.instance.gridManager.CompleteCurrentTile();
                break;
            case OverworldEncounterType.Bonfire:
                WorldStateSystem.SetInBonfire(true);
                break;
            default:
                break;
        }
    }
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
    Discharge
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
    Inventory,
    Placement,
    InactiveHighlight,
    Current,
    Active,
    Completed,
    Animation
}

public enum GridState
{
    Creating,
    Animating,
    Placement,
    Play,
    Panning,
    Overview,
    Dragging,
    Rotating,
    Complete,
    Encounter
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

public enum CardTargetType
{
    Self,
    AlliesInclSelf,
    AlliesExclSelf,
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
    CardEnergySpent
}

public enum RewardType
{
    None,
    Gold,
    Shard,
    Card,
    Heal,
    UnlockCard,
    Item = 10,
    Artifact = 11
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
    Envenom
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

public enum EncounterDifficulty
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
    Shop,           // buy stuff for permanent progress
    Tavern,         // hopefully used for companions
    Church,         // no idea, maybe remove
    TownHall,       // manage town and upgrade town, adding new buildings, story stuff, quests etc
    Scribe,         // manage the side deck, upgrade cards etc
    Barracks,       // manage characters, swap proffession
    Jeweler,        // manage tokens
    Any
}

public enum OverworldEncounterType
{
    Start,
    Exit,
    RandomEvent,
    Shop,
    CombatNormal = 4,
    CombatElite = 5,
    CombatBoss = 6,
    Cave,
    Bonfire
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
    ModifyEnergy
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

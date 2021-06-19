using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public static class EnumExtenstions
{
    public static string GetDescription(this EffectType type)
    {
        switch (type)
        {
            case EffectType.Barricade:
                return $"<b>Barricade</b>\nBlock is not removed at the start of your turn";
            case EffectType.Vulnerable:
                return $"<b>Vurnerable</b>\nRecieve 25% more attack damage";
            case EffectType.Thorns:
                return $"<b>Thorns</b>\nDeal damage back when attacked";
            case EffectType.Weak:
                return $"<b>Weak</b>\nDeals 25% less damage";
            case EffectType.Challenge:
                return $"<b>Challenge</b>\nChallanged actors deal double damage against eachother";
            case EffectType.Poison:
                return $"<b>Poison</b>\nLoose life at the end of the turn";
            case EffectType.StrengthEOT:
                return $"<b>StrengthEOT</b>\nTemporarily changes strength for the remainder of the turn.";
            default:
                return $"<b>{type.ToString()}</b>\nSeth is a very lazy man and has not written a tip for this effect. <i>(Also Fredrik smokes dicks.)</i>";
        }
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
            case EffectType.StrengthEOT:
                return new CardEffectStrengthEOT();
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
                WorldSystem.instance.combatManager.combatController.encounterData = DatabaseSystem.instance.GetRndEncounterCombat(type);
                WorldStateSystem.SetInCombat(true);
                break;
            case OverworldEncounterType.Shop:
                WorldStateSystem.SetInShop(true);
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



public enum Rarity
{
    None,
    Starting,
    Common,
    Uncommon,
    Rare
};

public enum ZoomState
{
    Inner, 
    Mid, 
    Outer
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
    Complete
}

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

public enum CardClassType
{
    None,
    Brute,
    Rogue,
    Splicer,
    Beastmaster,
    Burden,
    Torment,
    Enemy
}

public enum DialogueParticipant
{
    None,
    Player,
    Brute,
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
    CardSpecific,
    CardRandom,
    NewMap,
    NewEvent
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
    StrengthEOT
}

public enum OverlayState
{
    None,
    Transition,
    Dialogue,
    DeathScreen,
    EscapeMenu,
    Display,
    CharacterSheet
}

public enum WorldState
{
    MainMenu,
    Event,
    Bonfire,
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
    Town,
    DeathScreen,
    EnterAct

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

public enum CardActivityType
{
    DrawCard,
    AddCardToDeck,
    Splice,
    ExhaustDiscard,
    CombatCostChange,
    SetRandomBroken
}
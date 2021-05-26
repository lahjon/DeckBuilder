using UnityEngine;
using System.Linq;

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
            case EffectType.Vulnerable:
                return new RuleEffectVulnerable();
            case EffectType.Thorns:
                return new RuleEffectThorns();
            case EffectType.Splice:
                return new RuleEffectSplice();
            case EffectType.Poison:
                return new RuleEffectPoison();
            case EffectType.Challenge:
                return new RuleEffectChallenge();
            default:
                return null;
        }
    }

    public static Sprite GetIcon(this OverworldEncounterType type)
    {
        Sprite found = DatabaseSystem.instance.iconDatabase.allIcons.Where(x => x.name == "Overworld" + type.ToString()).FirstOrDefault();
        if (found != null) Debug.Log("returning icon with name: " + found.name);
        if (found != null) return found;
        Debug.Log("icon not found for: " + type.ToString());
        return DatabaseSystem.instance.iconDatabase.allIcons.Where(x => x.name == "OverworldRandomEvent").FirstOrDefault();
    }

    public static void Invoke(this OverworldEncounterType type)
    {
        switch (type)
        {
            case OverworldEncounterType.CombatNormal:
            case OverworldEncounterType.CombatElite:
            case OverworldEncounterType.CombatBoss:
                WorldStateSystem.SetInCombat(true);
                break;
            case OverworldEncounterType.Shop:
                WorldStateSystem.SetInShop(true);
                break;
            case OverworldEncounterType.RandomEvent:
                WorldSystem.instance.uiManager.encounterUI.encounterData = (EncounterDataRandomEvent)WorldSystem.instance.encounterManager.currentEncounterHex.encounterData;
                WorldStateSystem.SetInEvent(true);
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
    Special,
    Active,
    Completed,
    Animation
}

public enum GridState
{
    Creating,
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
};

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
    Challenge
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

public enum EnemyType
{
    Minion,
    Normal,
    Elite,
    Boss
}
public enum Formation
{

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
    CombatNormal,
    CombatElite,
    CombatBoss
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
    ExhaustDiscard
}
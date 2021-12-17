using System;
using System.Collections.Generic;
using UnityEngine;

/*************************************************
 *************** MANUAL *************************
 * 1. Utöka enum med din nya funktion 
 * 2.(a) Gör en ny static effectTypeinfo med alla relevant text osv
 * 2.(b) Om effekt inte skapad ännu: Använd dummy constructor CardEffectDummmy()
 * 3. Lägg till i dictionary
 */

public enum StatusEffectType
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
    Spiked,
    Barrier,
    Steaming,
    Loaded,
    RockSkin,
    Volatile,
    EN_BombObBlowUp,
    Tyrant,
    Wound
}

public enum StatusEffectCategory
{
    Buff,
    Debuff,
    Oath,
    Hidden
}

public class StatusEffectTypeInfo : IEquatable<StatusEffectTypeInfo>
{
    public StatusEffectType type;
    public string verb;
    public string toolTipCard;
    public string toolTipIcon; 
    public string txtOverride;
    public StatusEffectCategory category = StatusEffectCategory.Buff;
    public StatusEffect Constructor() => Helpers.InstanceObject<StatusEffect>(string.Format("StatusEffect{0}",type.ToString()));

    public static StatusEffectTypeInfo Damage = new StatusEffectTypeInfo { type = StatusEffectType.Damage, verb = "Deal", toolTipCard = String.Empty };
    public static StatusEffectTypeInfo Block = new StatusEffectTypeInfo { type = StatusEffectType.Block, verb = "Gain", toolTipCard = $"<b>Block</b>\nBlock negates incoming attack damage until the end of the round" };
    public static StatusEffectTypeInfo Poison = new StatusEffectTypeInfo { type = StatusEffectType.Poison, category = StatusEffectCategory.Debuff, verb = "Apply", toolTipCard = $"<b>Poison</b>\nLose life at the end of the turn", toolTipIcon = $"<b>Poison</b>\nLose life at the end of the turn"};
    public static StatusEffectTypeInfo Weak = new StatusEffectTypeInfo { type = StatusEffectType.Weak, category = StatusEffectCategory.Debuff, verb = "Apply", toolTipCard = $"<b>Weak</b>\nDeal 25% less damage", toolTipIcon = $"<b>Weak</b>\nDeal 25% less damage" };
    public static StatusEffectTypeInfo Barricade = new StatusEffectTypeInfo { type = StatusEffectType.Barricade, category = StatusEffectCategory.Oath, verb = "Gain", toolTipCard = Block.toolTipCard, toolTipIcon = $"<b>Barricade</b>\nBlock is retained between turns", txtOverride = "Block is retained between turns"};
    public static StatusEffectTypeInfo Vulnerable = new StatusEffectTypeInfo { type = StatusEffectType.Vulnerable, category = StatusEffectCategory.Debuff, verb = "Apply", toolTipCard = $"<b>Vurnerable</b>\nReceive 25% more attack damage", toolTipIcon = $"<b>Vurnerable</b>\nReceive 25% more attack damage" };
    public static StatusEffectTypeInfo Thorns = new StatusEffectTypeInfo { type = StatusEffectType.Thorns, category = StatusEffectCategory.Oath, verb = "Gain", toolTipCard = $"<b>Thorns</b>\nDeal damage back when attacked", toolTipIcon = $"<b>Thorns</b>\nDeal damage back when attacked"};
    public static StatusEffectTypeInfo Splice = new StatusEffectTypeInfo { type = StatusEffectType.Splice, verb = "Gain", toolTipIcon = $"<b>Splice</b>\nThe next splice card you play will meld with the top splice card in discard" };
    public static StatusEffectTypeInfo Challenge = new StatusEffectTypeInfo { type = StatusEffectType.Challenge, verb = "Call", txtOverride = "<b>Challenge</b> an Enemy. You deal double damage against eachother.", toolTipIcon = $"<b>Challenge</b>\nChallenged actors deal double damage against eachother", toolTipCard = $"<b>Challenge</b>\nChallenged actors deal double damage against eachother" };
    public static StatusEffectTypeInfo Strength = new StatusEffectTypeInfo { type = StatusEffectType.Strength, category = StatusEffectCategory.Oath, verb = "Gain", toolTipCard = $"<b>Strength</b>\nIncreases the damage of all attacks", toolTipIcon = $"<b>Strength</b>\nIncreases the damage of all attacks" };
    public static StatusEffectTypeInfo StrengthTemp = new StatusEffectTypeInfo { type = StatusEffectType.StrengthTemp, verb = "Apply", toolTipIcon = $"<b>StrengthTemp</b>\nTemporarily changes strength during your turn", toolTipCard = Strength.toolTipCard };
    public static StatusEffectTypeInfo Confused = new StatusEffectTypeInfo { type = StatusEffectType.Confused, category = StatusEffectCategory.Debuff, verb = "Apply", toolTipIcon = $"<b>Confused</b>\nCards with a single target will land on a random enemy" };
    public static StatusEffectTypeInfo Envenom = new StatusEffectTypeInfo { type = StatusEffectType.Envenom, category = StatusEffectCategory.Oath, verb = "Gain", toolTipCard = $"<b>Envenom</b>\nUnblocked damage inflicts poison.", toolTipIcon = $"<b>Envenom</b>\nUnblocked damage inflicts poison." };
    public static StatusEffectTypeInfo Dazed = new StatusEffectTypeInfo { type = StatusEffectType.Dazed, category = StatusEffectCategory.Debuff, verb = "Apply", toolTipIcon = $"<b>Dazed</b>\nThe next time you would gain block this turn, gain 0 block instead." };
    public static StatusEffectTypeInfo Spiked = new StatusEffectTypeInfo { type = StatusEffectType.Spiked, verb = "Apply", toolTipIcon = $"<b>Spiked</b>\nUntil your next turn, retailate when attacked.", toolTipCard = $"<b>Spiked</b>\nUntil your next turn, retailate when attacked." };
    public static StatusEffectTypeInfo Barrier = new StatusEffectTypeInfo { type = StatusEffectType.Barrier, verb = "Gain", toolTipCard = $"<b>Barrier</b>\nNegate the next time you would loose life.", toolTipIcon = $"<b>Barrier</b>\nNegate the next time you would loose life." };
    public static StatusEffectTypeInfo Steaming = new StatusEffectTypeInfo { type = StatusEffectType.Steaming, verb = "Gain",  toolTipIcon = $"<b>Steaming</b>\nUntil next turn, gain Rage when attacked.", toolTipCard = $"<b>Steaming</b>\nUntil next turn, gain Rage when attacked." };
    public static StatusEffectTypeInfo Loaded = new StatusEffectTypeInfo { type = StatusEffectType.Loaded, toolTipIcon = $"<b>Loaded</b>\nNext attack deals double damage.", txtOverride = "Your next attack deals double damage" };
    public static StatusEffectTypeInfo RockSkin = new StatusEffectTypeInfo { type = StatusEffectType.RockSkin, toolTipIcon = $"<b>Indestructible</b>\nGain block whenever shield is broken."};
    public static StatusEffectTypeInfo Volatile = new StatusEffectTypeInfo { type = StatusEffectType.Volatile, toolTipIcon = $"<b>Volatile</b>\nDeals half of total life to everyone on death."};
    public static StatusEffectTypeInfo EN_BombObBlowUp = new StatusEffectTypeInfo { type = StatusEffectType.EN_BombObBlowUp, toolTipIcon = $"<b>Self Destructor</b>\nMaybe this should be hidden?"};
    public static StatusEffectTypeInfo Tyrant = new StatusEffectTypeInfo { type = StatusEffectType.Tyrant, category = StatusEffectCategory.Oath, toolTipIcon = $"<b>Tyrant</b>\nUnblocked damage will be redirected to a random ally."};
    public static StatusEffectTypeInfo Wound = new StatusEffectTypeInfo { type = StatusEffectType.Wound, category = StatusEffectCategory.Oath, toolTipIcon = $"<b>Wound</b>\nAll attacks deal bonus damage."};

    static Dictionary<StatusEffectType, StatusEffectTypeInfo> TypeToStruct = new Dictionary<StatusEffectType, StatusEffectTypeInfo>
    {
        { StatusEffectType.Damage, Damage },
        { StatusEffectType.Block, Block },
        { StatusEffectType.Poison, Poison },
        { StatusEffectType.Weak, Weak },
        { StatusEffectType.Barricade, Barricade },
        { StatusEffectType.Vulnerable, Vulnerable },
        { StatusEffectType.Thorns, Thorns },
        { StatusEffectType.Splice, Splice },
        { StatusEffectType.Challenge, Challenge },
        { StatusEffectType.Strength, Strength },
        { StatusEffectType.StrengthTemp, StrengthTemp },
        { StatusEffectType.Confused, Confused },
        { StatusEffectType.Envenom, Envenom },
        { StatusEffectType.Spiked, Spiked },
        { StatusEffectType.Dazed, Dazed },
        { StatusEffectType.Barrier, Barrier },
        { StatusEffectType.Steaming, Steaming },
        { StatusEffectType.Loaded, Loaded},
        { StatusEffectType.RockSkin, RockSkin},
        { StatusEffectType.Volatile, Volatile},
        { StatusEffectType.EN_BombObBlowUp, EN_BombObBlowUp},
        { StatusEffectType.Tyrant, Tyrant},
        { StatusEffectType.Wound, Wound},
    };

    #region usually dont touch

    public static StatusEffectTypeInfo GetEffectTypeInfo(StatusEffectType type) => TypeToStruct[type];

    public bool Equals(StatusEffectType other) => type == other;

    public bool Equals(StatusEffectTypeInfo obj) => obj is StatusEffectTypeInfo other && Equals(other);

    public static implicit operator StatusEffectType(StatusEffectTypeInfo e) => e.type;

    public override string ToString() => type.ToString();

    #endregion
}


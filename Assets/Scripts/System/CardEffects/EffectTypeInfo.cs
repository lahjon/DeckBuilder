using System;
using System.Collections.Generic;
using UnityEngine;

public class EffectTypeInfo : IEquatable<EffectTypeInfo>
{
    public EffectType effectType;
    public string verb;
    public string toolTipCard;
    public string toolTipIcon; 
    public string txtOverride;
    public Func<CardEffect> constructor;
    public bool isOath = false;

    public static EffectTypeInfo Damage = new EffectTypeInfo { effectType = EffectType.Damage, verb = "Deal", toolTipCard = String.Empty };
    public static EffectTypeInfo Block = new EffectTypeInfo { effectType = EffectType.Block, verb = "Gain", toolTipCard = $"<b>Block</b>\nBlock negates incoming attack damage until the end of the round" };
    public static EffectTypeInfo Poison = new EffectTypeInfo { effectType = EffectType.Poison, verb = "Apply", constructor = () => { return new CardEffectPoison(); }, toolTipCard = $"<b>Poison</b>\nLose life at the end of the turn", toolTipIcon = $"<b>Poison</b>\nLose life at the end of the turn"};
    public static EffectTypeInfo Weak = new EffectTypeInfo { effectType = EffectType.Weak, verb = "Apply", constructor = () => { return new CardEffectWeak(); }, toolTipCard = $"<b>Weak</b>\nDeal 25% less damage", toolTipIcon = $"<b>Weak</b>\nDeal 25% less damage" };
    public static EffectTypeInfo Barricade = new EffectTypeInfo { effectType = EffectType.Barricade, verb = "Gain", constructor = () => { return new CardEffectBarricade(); }, toolTipCard = Block.toolTipCard, toolTipIcon = $"<b>Barricade</b>\nBlock is retained between turns", txtOverride = "Block is retained between turns", isOath = true };
    public static EffectTypeInfo Vulnerable = new EffectTypeInfo { effectType = EffectType.Vulnerable, verb = "Apply", constructor = () => { return new CardEffectVulnerable(); }, toolTipCard = $"<b>Vurnerable</b>\nReceive 25% more attack damage", toolTipIcon = $"<b>Vurnerable</b>\nReceive 25% more attack damage" };
    public static EffectTypeInfo Thorns = new EffectTypeInfo { effectType = EffectType.Thorns, verb = "Gain", constructor = () => { return new CardEffectThorns(); }, toolTipCard = $"<b>Thorns</b>\nDeal damage back when attacked", toolTipIcon = $"<b>Thorns</b>\nDeal damage back when attacked", isOath = true };
    public static EffectTypeInfo Splice = new EffectTypeInfo { effectType = EffectType.Splice, verb = "Gain", constructor = () => { return new CardEffectSplice(); }, toolTipIcon = $"<b>Splice</b>\nThe next splice card you play will meld with the top splice card in discard" };
    public static EffectTypeInfo Challenge = new EffectTypeInfo { effectType = EffectType.Challenge, verb = "Call", constructor = () => { return new CardEffectChallenge(); }, txtOverride = "<b>Challenge</b> an Enemy. You deal double damage against eachother.", toolTipIcon = $"<b>Challenge</b>\nChallenged actors deal double damage against eachother", toolTipCard = $"<b>Challenge</b>\nChallenged actors deal double damage against eachother", isOath = true };
    public static EffectTypeInfo Strength = new EffectTypeInfo { effectType = EffectType.Strength, verb = "Gain", constructor = () => { return new CardEffectStrength(); }, toolTipCard = $"<b>Strength</b>\nIncreases the damage of all attacks", toolTipIcon = $"<b>Strength</b>\nIncreases the damage of all attacks", isOath = true };
    public static EffectTypeInfo StrengthTemp = new EffectTypeInfo { effectType = EffectType.StrengthTemp, verb = "Apply", constructor = () => { return new CardEffectStrengthTemp(); }, toolTipIcon = $"<b>StrengthTemp</b>\nTemporarily changes strength during your turn", toolTipCard = Strength.toolTipCard };
    public static EffectTypeInfo Confused = new EffectTypeInfo { effectType = EffectType.Confused, verb = "Apply", constructor = () => { return new CardEffectConfused(); }, toolTipIcon = $"<b>Confused</b>\nCards with a single target will land on a random enemy" };
    public static EffectTypeInfo Envenom = new EffectTypeInfo { effectType = EffectType.Envenom, verb = "Gain", constructor = () => { return new CardEffectEnvenom(); }, toolTipCard = $"<b>Envenom</b>\nUnblocked damage inflicts poison.", toolTipIcon = $"<b>Envenom</b>\nUnblocked damage inflicts poison.", isOath = true };
    public static EffectTypeInfo Dazed = new EffectTypeInfo { effectType = EffectType.Dazed, verb = "Apply", constructor = () => { return new CardEffectDazed(); }, toolTipIcon = $"<b>Dazed</b>\nThe next time you would gain block this turn, gain 0 block instead." };
    public static EffectTypeInfo Spiked = new EffectTypeInfo { effectType = EffectType.Spiked, verb = "Apply", constructor = () => { return new CardEffectSpiked(); }, toolTipIcon = $"<b>Spiked</b>\nUntil your next turn, retailate when attacked.", toolTipCard = $"<b>Spiked</b>\nUntil your next turn, retailate when attacked." };
    public static EffectTypeInfo Barrier = new EffectTypeInfo { effectType = EffectType.Barrier, verb = "Gain", constructor = () => { return new CardEffectBarrier(); }, toolTipCard = $"<b>Barrier</b>\nNegate the next time you would loose life.", toolTipIcon = $"<b>Barrier</b>\nNegate the next time you would loose life." };
    public static EffectTypeInfo Steaming = new EffectTypeInfo { effectType = EffectType.Steaming, verb = "Gain", constructor = () => { return new CardEffectSteaming(); }, toolTipIcon = $"<b>Steaming</b>\nUntil next turn, gain Rage when attacked.", toolTipCard = $"<b>Steaming</b>\nUntil next turn, gain Rage when attacked." };
    public static EffectTypeInfo Loaded = new EffectTypeInfo { effectType = EffectType.Loaded, constructor = () => { return new CardEffectLoaded(); }, toolTipIcon = $"<b>Loaded</b>\nNext attack deals double damage.", txtOverride = "Your next attack deals double damage" };

    static Dictionary<EffectType, EffectTypeInfo> TypeToStruct = new Dictionary<EffectType, EffectTypeInfo>
    {
        { EffectType.Damage, Damage },
        { EffectType.Block, Block },
        { EffectType.Poison, Poison },
        { EffectType.Weak, Weak },
        { EffectType.Barricade, Barricade },
        { EffectType.Vulnerable, Vulnerable },
        { EffectType.Thorns, Thorns },
        { EffectType.Splice, Splice },
        { EffectType.Challenge, Challenge },
        { EffectType.Strength, Strength },
        { EffectType.StrengthTemp, StrengthTemp },
        { EffectType.Confused, Confused },
        { EffectType.Envenom, Envenom },
        { EffectType.Spiked, Spiked },
        { EffectType.Dazed, Dazed },
        { EffectType.Barrier, Barrier },
        { EffectType.Steaming, Steaming },
        { EffectType.Loaded, Loaded},
    };

    public static EffectTypeInfo GetEffectTypeInfo(EffectType type){

        Debug.Log(type.ToString());
        Debug.Log(TypeToStruct.ContainsKey(type));
        return TypeToStruct[type];
    }

    public bool Equals(EffectType other) => effectType == other;

    public bool Equals(EffectTypeInfo obj) => obj is EffectTypeInfo other && Equals(other);

    public static implicit operator EffectType(EffectTypeInfo e) => e.effectType;

    public override string ToString() => effectType.ToString(); 
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
    Spiked,
    Barrier,
    Steaming,
    Loaded
}


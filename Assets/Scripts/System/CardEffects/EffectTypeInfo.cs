using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EffectTypeInfo : IEquatable<EffectTypeInfo>
{
    public EffectType effectType;
    public string verb;
    public string toolTip;
    public Func<CardEffect> constructor;

    public static EffectTypeInfo Damage = new EffectTypeInfo { effectType = EffectType.Damage, verb = "Deal", toolTip = String.Empty };
    public static EffectTypeInfo Block = new EffectTypeInfo { effectType = EffectType.Block, verb = "Gain", toolTip = $"<b>Block</b>\nBlock negates incoming attack damage until the end of the round" };
    public static EffectTypeInfo Poison = new EffectTypeInfo { effectType = EffectType.Poison, verb = "Apply", constructor = () => { return new CardEffectPoison(); }, toolTip = $"<b>Poison</b>\nLose life at the end of the turn" };
    public static EffectTypeInfo Weak = new EffectTypeInfo { effectType = EffectType.Weak, verb = "Apply", constructor = () => { return new CardEffectWeak(); }, toolTip = $"<b>Weak</b>\nDeal 25% less damage" };
    public static EffectTypeInfo Barricade = new EffectTypeInfo { effectType = EffectType.Barricade, verb = "Gain", constructor = () => { return new CardEffectBarricade(); }, toolTip = $"<b>Barricade</b>\nBlock is retained between turns" };
    public static EffectTypeInfo Vulnerable = new EffectTypeInfo { effectType = EffectType.Vulnerable, verb = "Apply", constructor = () => { return new CardEffectVulnerable(); }, toolTip = $"<b>Vurnerable</b>\nReceive 25% more attack damage" };
    public static EffectTypeInfo Thorns = new EffectTypeInfo { effectType = EffectType.Thorns, verb = "Gain", constructor = () => { return new CardEffectThorns(); }, toolTip = $"<b>Thorns</b>\nDeal damage back when attacked" };
    public static EffectTypeInfo Splice = new EffectTypeInfo { effectType = EffectType.Splice, verb = "Gain", constructor = () => { return new CardEffectSplice(); }, toolTip = $"<b>Splice</b>\nThe next splice card you play will meld with the top splice card in discard" };
    public static EffectTypeInfo Challenge = new EffectTypeInfo { effectType = EffectType.Challenge, verb = "Call", constructor = () => { return new CardEffectChallenge(); }, toolTip = $"<b>Challenge</b>\nChallenged actors deal double damage against eachother" };
    public static EffectTypeInfo Strength = new EffectTypeInfo { effectType = EffectType.Strength, verb = "Gain", constructor = () => { return new CardEffectStrength(); }, toolTip = $"<b>Strength</b>\nIncreases the value of all attacks" };
    public static EffectTypeInfo StrengthTemp = new EffectTypeInfo { effectType = EffectType.StrengthTemp, verb = "Apply", constructor = () => { return new CardEffectStrengthTemp(); }, toolTip = $"<b>StrengthTemp</b>\nTemporarily changes strength during your turn" };
    public static EffectTypeInfo Confused = new EffectTypeInfo { effectType = EffectType.Confused, verb = "Apply", constructor = () => { return new CardEffectConfused(); }, toolTip = $"<b>Confused</b>\nCards with a single target will land on a random enemy" };
    public static EffectTypeInfo Envenom = new EffectTypeInfo { effectType = EffectType.Envenom, verb = "Gain", constructor = () => { return new CardEffectEnvenom(); }, toolTip = $"<b>Envenom</b>\nUnblocked damage inflicts poison." };
    public static EffectTypeInfo Dazed = new EffectTypeInfo { effectType = EffectType.Dazed, verb = "Apply", constructor = () => { return new CardEffectDazed(); }, toolTip = $"<b>Dazed</b>\nThe next time you would gain block this turn, gain 0 block instead." };
    public static EffectTypeInfo Spiked = new EffectTypeInfo { effectType = EffectType.Spiked, verb = "Apply", constructor = () => { return new CardEffectSpiked(); }, toolTip = $"<b>Spiked</b>\nUntil your next turn, retailate when attacked." };
    public static EffectTypeInfo Barrier = new EffectTypeInfo { effectType = EffectType.Barrier, verb = "Gain", constructor = () => { return new CardEffectBarrier(); }, toolTip = $"<b>Barrier</b>\nNegate the next time you would loose life." };
    public static EffectTypeInfo Steaming = new EffectTypeInfo { effectType = EffectType.Steaming, verb = "Gain", constructor = () => { return new CardEffectSteaming(); }, toolTip = $"<b>Steaming</b>\nUntil next turn, gain Rage when attacked." };

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


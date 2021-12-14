using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class CardCost
{
    Card card;

    public Dictionary<EnergyType, CardInt> energyCosts = new Dictionary<EnergyType, CardInt>();
    public Dictionary<EnergyType, CardInt> energyCostsOptional =
    new Dictionary<EnergyType, CardInt>();
    Dictionary<EnergyType, int> paidEnergy = new Dictionary<EnergyType, int>();
    Dictionary<EnergyType, int> paidEnergyOptional = new Dictionary<EnergyType, int>();

    public bool optionalPaid { get { return paidEnergyOptional.Count > 0; } }

    public CardCost(Card card, List<EnergyData> energyDatas, List<EnergyData> energyOptionalDatas)
    {
        this.card = card;
        foreach (EnergyData d in energyDatas)
        {
            energyCosts[d.type] = CardInt.Factory(d.data, card);
            energyCosts[d.type].limitLower = 0;
        }

        foreach (EnergyData d in energyOptionalDatas)
        {
            energyCostsOptional[d.type] = CardInt.Factory(d.data, card);
            energyCostsOptional[d.type].limitLower = 0;
        }
    }

    public bool Payable()
    {
        if (energyCosts.Count == 0) return false;

        foreach (EnergyType type in energyCosts.Keys)
            if (energyCosts[type] > CombatSystem.instance.GetEnergy(type)) return false;

        return true;
    }

    public bool PayableOptional()
    {
        foreach (EnergyType type in energyCosts.Keys.Union(energyCostsOptional.Keys))
            if (
                    ((energyCosts.ContainsKey(type) ? energyCosts[type] : 0) +
                    (energyCostsOptional.ContainsKey(type) ? energyCostsOptional[type] : 0))       
                > CombatSystem.instance.GetEnergy(type))
                return false;

        return true;
    }

    public void Pay()
    {
        if (!(card.owner == CombatSystem.instance.Hero)) return;

        paidEnergy.Clear();
        paidEnergyOptional.Clear();

        Dictionary<EnergyType, int> frozenCost = new Dictionary<EnergyType, int>();
        foreach (EnergyType type in energyCosts.Keys)
        {
            frozenCost[type] = -energyCosts[type];
            paidEnergy[type] = energyCosts[type];
        }

        if (PayableOptional())
        {
            foreach (EnergyType type in energyCostsOptional.Keys)
            {
                if (energyCostsOptional[type] > CombatSystem.instance.GetEnergy(type) + (frozenCost.ContainsKey(type) ? frozenCost[type] : 0) ) continue;
                frozenCost[type]            = (frozenCost.ContainsKey(type) ? frozenCost[type] : 0) - energyCostsOptional[type];
                paidEnergyOptional[type]    = energyCostsOptional[type];
            }
        }

        CombatSystem.instance.ModifyEnergy(frozenCost);
    }

    public void Refund()
    {
        CombatSystem.instance.ModifyEnergy(paidEnergy);
        CombatSystem.instance.ModifyEnergy(paidEnergyOptional);
        paidEnergy.Clear();
        paidEnergyOptional.Clear();
    }

    public void ModifyCardCost(EnergyType type, int energyModify)
    {
        if (!energyCosts.ContainsKey(type)) return;

        energyCosts[type].modifier += energyModify;

        if (card is CardVisual cv)
            UpdateTextsForCosts();
    }

    public void UpdateTextsForCosts()
    {
        if (card is CardVisual cv) {
            foreach (EnergyType type in energyCosts.Keys)
                cv.energyToCostUI[type].lblEnergy.text = energyCosts[type].GetTextForCost();
            foreach (EnergyType type in energyCostsOptional.Keys)
                cv.energyToCostOptionalUI[type].lblEnergy.text = String.Format("({0})", energyCostsOptional[type].GetTextForCost());
        }
    }

    public void AbsorbModifier(List<EnergyData> energyDatas, List<EnergyData> energyOptionalDatas)
    {
        foreach (EnergyData d in energyDatas)
        {
            if (energyCosts.ContainsKey(d.type))
                energyCosts[d.type].AbsorbModifier(d.data);
            else
            {
                energyCosts[d.type] = CardInt.Factory(d.data, card);
                energyCosts[d.type].limitLower = 0;
            }
        }

        foreach (EnergyData d in energyOptionalDatas)
        {
            if (energyCostsOptional.ContainsKey(d.type))
                energyCostsOptional[d.type].AbsorbModifier(d.data);
            else
            {
                energyCostsOptional[d.type] = CardInt.Factory(d.data, card);
                energyCostsOptional[d.type].limitLower = 0;
            }
        }
    }

    public int GetPaidEnergy(EnergyType type) => paidEnergy.ContainsKey(type) ? paidEnergy[type] : 0;


}


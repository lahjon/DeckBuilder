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
    Dictionary<EnergyType, int> paidEnergy = new Dictionary<EnergyType, int>();

    public CardCost(Card card, List<EnergyData> energyDatas)
    {
        this.card = card;
        foreach (EnergyData d in energyDatas)
        {
            energyCosts[d.type] = CardInt.Factory(d.data, card);
            energyCosts[d.type].limitLower = 0;
        }
    }

    public bool Payable()
    {
        foreach (EnergyType type in energyCosts.Keys)
            if (energyCosts[type] > CombatSystem.instance.GetEnergy(type)) return false;

        return true;
    }

    public void Pay()
    {
        if (!(card.owner == CombatSystem.instance.Hero)) return;
        Dictionary<EnergyType, int> frozenCost = new Dictionary<EnergyType, int>();
        foreach (EnergyType type in energyCosts.Keys)
        {
            frozenCost[type] = -energyCosts[type];
            paidEnergy[type] = energyCosts[type];
        }

        CombatSystem.instance.ModifyEnergy(frozenCost);
    }

    public void Refund()
    {
        CombatSystem.instance.ModifyEnergy(paidEnergy);
        paidEnergy.Clear();
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
        }
    }

    public int GetPaidEnergy(EnergyType type) => paidEnergy.ContainsKey(type) ? paidEnergy[type] : 0;


}


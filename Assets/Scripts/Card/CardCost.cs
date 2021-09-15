using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class CardCost
{
    public CardInt EnergyCost;
    public int PaidEnergy;

    Card card;

    public CardCost(Card card, string energy)
    {
        this.card = card;
        EnergyCost = CardInt.Factory(energy, card);
    }

    public bool Payable
    {
        get { return CombatSystem.instance.cEnergy >= EnergyCost; }
    }

    public void Pay()
    {
        if (!(card.owner is CombatActorHero)) return;

        PaidEnergy = EnergyCost; // ändra till mer avancerat sen när vi har fler energytyper;
        CombatSystem.instance.cEnergy -= EnergyCost;
    }

    public void Refund()
    {
        CombatSystem.instance.cEnergy += PaidEnergy;
        PaidEnergy = 0;
    }

    public string GetTextForCost()
    {
        return EnergyCost.GetTextForCost();
    }

    public void ModifyCardCost(int energyModify)
    {
        EnergyCost.modifier += energyModify;
        if (card is CardVisual cv)
            cv.costText.text = GetTextForCost();
    }
}


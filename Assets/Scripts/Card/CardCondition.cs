using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class CardCondition : IEvents
{
    ConditionStruct conditionStruct;
    public bool value;

    public void CheckCondition()
    {
        ConditionSystem.CheckCondition(conditionStruct);
    }

    public void Subscribe()
    {
        switch (conditionStruct.type)
        {
            case ConditionType.CardsPlayedAbove:
            case ConditionType.CardsPlayedBelow:
                EventManager.OnCardPlayEvent += CheckCondition;
                break;


        }
    }

    public void Unsubscribe()
    {
        throw new NotImplementedException();
    }
}


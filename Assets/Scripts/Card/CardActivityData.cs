using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public struct CardActivityData : ICardUpgradingData
{
    public CombatActivityType type;
    public string strParameter;
    public int val;

    public ConditionData conditionStruct;

    public CardComponentExecType execTime;
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public struct CardActivityData
{
    public CardActivityType type;
    public string parameter;

    public ConditionStruct conditionStruct;

    public CardComponentExecType execTime;
}


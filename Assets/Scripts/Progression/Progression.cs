using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class Progression : MonoBehaviour
{
    public int id;
    public string aName;
    public string description;
    public List<ConditionCounting> countingConditions = new List<ConditionCounting>();
    public bool completed = false;

    public void CreateGoals(ProgressionData aData)
    {
        if (aData == null) return;
        foreach (ConditionData x in aData.conditionStructs)
        {
            ConditionCounting countingCondition = ConditionCounting.Factory(x, ConditionOnUpdate, ConditionOnTrue);
            countingCondition.Subscribe();
            countingConditions.Add(countingCondition);
        }
    }

    void OnDestroy()
    {
        countingConditions.ForEach(x => x.Unsubscribe());
    }

    void ConditionOnUpdate()
    {
        Debug.Log("Updating progression");
    }

    void ConditionOnTrue()
    {
        if (countingConditions.All(x => x.currentAmount >= x.requiredAmount)) Complete();
    }

    protected virtual void Complete()
    {
        Debug.Log("Progression Done");
    }

}
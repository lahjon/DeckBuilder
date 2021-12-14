    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public abstract class Condition : IEventSubscriber
{
    public ConditionData conditionData;
    public ConditionTypeInfo info;
    public bool value;

    public Action OnPreConditionUpdate;
    public Action OnConditionFlipTrue;
    public Action OnConditionFlipFalse;
    public Action OnConditionFlip;

    public static implicit operator bool(Condition c) => c.value;

    public static Condition Factory(ConditionData conditionData, Action OnPreConditionUpdate = null, Action OnConditionFlip = null, Action OnConditionFlipTrue = null, Action OnConditionFlipFalse = null)
    {
        if(conditionData == null) return new ConditionNotConfigured();
        Condition cond = Helpers.InstanceObject<Condition>(string.Format("Condition{0}", conditionData.type));
        if (cond == null) return new ConditionNotConfigured();
        cond.SetParameters(conditionData, OnPreConditionUpdate, OnConditionFlip, OnConditionFlipTrue, OnConditionFlipFalse);
        return cond;
    }

    public void SetParameters(ConditionData conditionData, Action OnPreConditionUpdate = null, Action OnConditionFlip = null, Action OnConditionFlipTrue = null, Action OnConditionFlipFalse = null)
    {
        this.conditionData = conditionData;
        if (conditionData == null || conditionData.type == ConditionType.None)
        {
            value = true;
            return;
        }

        info = ConditionTypeInfo.GetConditionInfo(conditionData.type);
        this.OnPreConditionUpdate = OnPreConditionUpdate;
        this.OnConditionFlip = OnConditionFlip;
        this.OnConditionFlipTrue = OnConditionFlipTrue;
        this.OnConditionFlipFalse = OnConditionFlipFalse;
    }

    public string GetTextCard() => (conditionData == null || conditionData.type == ConditionType.None) ? "" : (info.GetTextInfo(conditionData) + ":\n");

    public virtual void Subscribe()
    {
        if (this is ConditionCounting) return;
        OnEventNotification();
    }
    public abstract void Unsubscribe();
    public abstract bool ConditionEvaluator();

    public virtual void OnEventNotification()
    {
        bool oldVal = value;
        value = ConditionEvaluator();

        OnPreConditionUpdate?.Invoke();

        if (oldVal != value)
        {
            OnConditionFlip?.Invoke();
            if (oldVal)
                OnConditionFlipFalse?.Invoke();
            else
                OnConditionFlipTrue?.Invoke();
        }
    }

}



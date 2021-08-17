using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Condition
{
    int _currentAmount;
    public int requiredAmount;
    public string value;
    public ConditionType conditionType;
    System.Action CompleteCondition;
    System.Action UpdateCondition;
    public virtual int currentAmount
    {
        get => _currentAmount;
        set
        {
            _currentAmount = value;
        }
    }
    public Condition(ConditionStruct conditionStruct, System.Action aUpdateCondition, System.Action aCompleteCondition)
    {
        Debug.Log("Creating new condition! " + this);
        value = conditionStruct.value;
        requiredAmount = conditionStruct.amount;
        conditionType = conditionStruct.type;
        UpdateCondition = aUpdateCondition;
        CompleteCondition = aCompleteCondition;
        Subscribe();
    }
    

    public void Subscribe()
    {
        switch (conditionType)
        {
            case ConditionType.WinCombat:
                EventManager.OnCompleteTileEvent += RefreshCondition;
                break;
            case ConditionType.ClearTile:
                EventManager.OnCompleteTileEvent += RefreshCondition;
                break;
            case ConditionType.KillEnemy:
                EventManager.OnEnemyKilledEvent += RefreshCondition;
                break;
            
            default:
                break;
        }
    }
    public void Unsubscribe()
    {
        switch (conditionType)
        {
            case ConditionType.WinCombat:
                EventManager.OnCompleteTileEvent -= RefreshCondition;
                break;
            case ConditionType.ClearTile:
                EventManager.OnCompleteTileEvent -= RefreshCondition;
                break;
            case ConditionType.KillEnemy:
                EventManager.OnEnemyKilledEvent -= RefreshCondition;
                break;
            
            default:
                break;
        }
    }
    public virtual void AcceptCondition()
    {
        currentAmount = 0;
        CompleteCondition?.Invoke();
    }

    public virtual void CheckCondition()
    {
        Debug.Log("currentAmount: "  + currentAmount);
        Debug.Log("requiredAmount: "  + requiredAmount);
        UpdateCondition?.Invoke();
        if (currentAmount >= requiredAmount) AcceptCondition();
    }

    public static Condition CreateCondition(ConditionStruct type, System.Action refresh, System.Action execute)
    {
        return new Condition(type, refresh, execute);
    }
    public static string GetDescription(ConditionStruct type) => type.type switch
    {
        ConditionType.KillEnemy => string.Format("<b>Kill Enemies (" + type.amount + ")</b>"),
        ConditionType.ClearTile => string.Format("<b>Clear Tiles (" +  type.amount + ")</b>"),
        ConditionType.WinCombat => string.Format("<b>Win Combats (" +  type.amount + ")</b>"),
        _                           => null
    };

    void RefreshCondition(EnemyData enemy)
    {
        Debug.Log("Enemy Killed " + this);
        if (enemy.enemyId == value || string.IsNullOrEmpty(value))
        {
            currentAmount++;
            CheckCondition();
        }
    }

    void RefreshCondition()
    {
        currentAmount++;
        CheckCondition();
    }

}

public interface ICondition
{
    void UpdateCondition();
    void CompleteCondition();
}

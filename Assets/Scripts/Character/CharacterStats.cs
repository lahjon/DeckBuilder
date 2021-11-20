using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CharacterStats : MonoBehaviour
{
    CharacterManager characterManager;
    [SerializeField]
    List<Stat> stats = new List<Stat>();
    public void Init()
    {
        characterManager = WorldSystem.instance.characterManager;
        stats.Clear();
        characterManager.characterData.stats.ForEach(x => stats.Add(new Stat(x.value, x.type)));
    }

    public int GetStat(StatType aStatType)
    {
        if (stats?.Any() == true)
            return stats.Where(x => x.type == aStatType).FirstOrDefault().value;;
        return 0;
    }

    public void ModifyHealth(int aValue)
    {
        Stat health = stats.Where(x => x.type == StatType.Health).FirstOrDefault();
        health.value += aValue;

        characterManager.currentHealth += aValue;

        if (GetStat(StatType.Health) < 1)
        {
            health.value = 1;
        }

        if (characterManager.currentHealth < 1)
        {
            characterManager.currentHealth = 1;
        }

        characterManager.characterVariablesUI.UpdateCharacterHUD();
    }

    public void ModifyStat(StatType aStatType, int aValue)
    {
        stats.Where(x => x.type == aStatType).FirstOrDefault().value += aValue;
        characterManager.characterVariablesUI.UpdateCharacterHUD();
    }
}
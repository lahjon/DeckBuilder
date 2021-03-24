using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CharacterStats : MonoBehaviour
{
    CharacterManager characterManager;
    public List<Stat> stats = new List<Stat>();
    public List<Stat> statsModifer = new List<Stat>();
    public void Init()
    {
        characterManager = WorldSystem.instance.characterManager;
        stats = characterManager.character.characterData.stats;
        //statsModifer.AddRange(stats);
        //stats.ForEach(x => statsModifer.Add(new Stat(0, x.type)));
        // for (int i = 0; i < statsModifer.Count; i++)
        // {
        //     statsModifer[i].value = 0;
        // }
    }

    // public (int baseValue, int modiferValue, int sum) GetStat(StatType aStatType)
    // {
    //     int value = stats.Where(x => x.type == aStatType).FirstOrDefault().value;
    //     int valueMod = statsModifer.Where(x => x.type == aStatType).FirstOrDefault().value;
    //     return (value, valueMod, value + valueMod);
    // }

    public int GetStat(StatType aStatType)
    {
        int value = stats.Where(x => x.type == aStatType).FirstOrDefault().value;
        return value;
    }

    public void AddStat(StatType aStatType, int aValue)
    {
        stats.Where(x => x.type == aStatType).FirstOrDefault().value += aValue;
    }



    public void AddModifier(int aValue, StatType aStatType)
    {

    }
    public void RemoveModifier(int aValue, StatType aStatType)
    {

    }
}
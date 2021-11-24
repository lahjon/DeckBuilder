using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Profession : IEffectAdder
{
    public List<ItemEffect> itemEffect;
    public ProfessionData professionData;
    public Profession(ProfessionData aProfessionData)
    {
        professionData = aProfessionData;
    }
    public void AddEffects()
    {
        itemEffect = new List<ItemEffect>();
        for (int i = 0; i < professionData.itemEffectStructs.Count; i++)
            itemEffect.Add(WorldSystem.instance.itemEffectManager.CreateItemEffect(professionData.itemEffectStructs[i], this, professionData.professionName, true));
    }
    public void RemoveEffects()
    {
        for (int i = 0; i < itemEffect.Count; i++)
            itemEffect[i].RemoveItemEffect();
    }
    public void NotifyUsed()
    {
        
    }

    public static Profession AddProfession(ProfessionData aProfessionData)
    {
        Profession profession = new Profession(aProfessionData);
        profession.AddEffects();
        return profession;
    }
}
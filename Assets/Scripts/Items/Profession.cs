using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Profession : IEffectAdder
{
    public List<ItemEffect> itemEffect;
    public ProfessionData professionData;
    public Profession(ProfessionData aProfessionData)
    {
        professionData = aProfessionData;
    }
    void AddEffects()
    {
        itemEffect = new List<ItemEffect>();
        for (int i = 0; i < professionData.itemEffectStructs.Count; i++)
        {
            ItemEffect newItemEffect = ItemEffect.Factory(professionData.itemEffectStructs[i], this);
            newItemEffect.Register();
            itemEffect.Add(newItemEffect);
        }
    }

    void AddAbilities()
    {
        professionData.abilityDatas.ForEach(x => WorldSystem.instance.abilityManager.EquipAbility(x.itemId));
    }
    public void RemoveEffects()
    {
        for (int i = 0; i < itemEffect.Count; i++)
            itemEffect[i].DeRegister();
    }
    public void NotifyUsed()
    {
        
    }

    public void NotifyDeregister()
    {

    }

    public static Profession AddProfession(ProfessionData aProfessionData)
    {
        Profession profession = new Profession(aProfessionData);
        profession.AddEffects();
        profession.AddAbilities();
        return profession;
    }

    public int GetValue()
    {
        return 0;
    }

    public string GetName()
    {
        return professionData.professionName;
    }

    public void NotifyRegister()
    {
    }
}
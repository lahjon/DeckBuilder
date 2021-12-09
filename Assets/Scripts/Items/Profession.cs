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
            itemEffect.Add(ItemEffect.Factory(professionData.itemEffectStructs[i], this));
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
        return profession;
    }

    public int GetValue()
    {
        return 0;
    }

    public string GetName()
    {
        return null;
    }

    public void NotifyRegister()
    {
    }
}
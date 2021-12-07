using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEffectAddCardDeck : ItemEffect
{
    public int GetValue() => itemEffectStruct.value;
    public string GetName() => effectAdder.GetName();

    public override void Register()
    {
        base.Register();
        string[] ids = itemEffectStruct.parameter.Split(';');
        foreach (string id in ids)
            WorldSystem.instance.characterManager.AddCardToDeck(DatabaseSystem.instance.GetCardByID(id));
    }

    public override void DeRegister()
    {
        base.DeRegister();
        
    }

}

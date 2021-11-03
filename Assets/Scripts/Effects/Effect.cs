using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Effect : MonoBehaviour
{
    public virtual void Init()
    {

    }
    public abstract void AddEffect();
    public abstract void RemoveEffect();

    public static Effect GetEffect(GameObject obj, string name, bool addEffect = false)
    {   
        /// <summary> obj: the game object to attach the effect to
        /// name: the name of the effect excluding "Effect"
        /// create the effects in the format "EffectName" </summary>

        string effectName = string.Format("Effect" + name);
        // Debug.Log("Adding effect with name: " + effectName);
        Effect effect = (Effect)obj.AddComponent(Type.GetType(effectName));
        //effect.Init();

        if (effect != null && addEffect)
            effect.AddEffect();
        else if(addEffect)
            Debug.LogWarning("Create a new effect for " + name + ". Do not include Effect in the name, its added after");
        
        return effect;
    }
}

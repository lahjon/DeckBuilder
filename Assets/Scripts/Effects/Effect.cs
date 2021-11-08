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

        if (!string.IsNullOrEmpty(name) && Type.GetType(string.Format("Effect{0}", name)) is System.Type type)
        {
            Effect effect = (Effect)obj.AddComponent(type);
            if (addEffect)
                effect?.AddEffect();
            return effect;
        }            
        else
        {
            Debug.LogWarning(string.Format("Create a new effect for {0}. Do not include Effect in the name, its added after", name));
            return null;
        }
        
    }
}

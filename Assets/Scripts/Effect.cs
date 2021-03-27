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
        Effect effect = (Effect)obj.AddComponent(Type.GetType(name));

        if (effect != null && addEffect)
        {
            effect.AddEffect();
        }
        
        return effect;
    }
}

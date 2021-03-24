using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Effect
{
    public virtual void Init()
    {

    }
    public abstract void AddEffect();
    public abstract void RemoveEffect();
}

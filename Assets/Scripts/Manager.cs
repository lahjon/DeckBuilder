using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Manager : MonoBehaviour
{
    protected WorldSystem world;
    protected virtual void Start()
    {
        world = WorldSystem.instance;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Manager : MonoBehaviour
{
    protected WorldSystem world;
    protected virtual void Awake()
    {
        world = WorldSystem.instance;
    }
    
    protected virtual void Start()
    {
    }
}

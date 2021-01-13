using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    // stats
    public int strength;
    public int cunning;
    public int speed;
    public int endurance;
    public int wisdom;

    void Awake()
    {
        // unparent to keep in world
        this.gameObject.transform.parent = null;
        DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}

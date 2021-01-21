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
    public CharacterClass characterClass;
    public Encounter currentEncounter;
    

    void Awake()
    {
        // unparent to keep in world
        this.gameObject.transform.parent = null;
        DontDestroyOnLoad(this.gameObject);
    }

    public void MoveToLocation(Vector3 position, Encounter encounter)
    {
        gameObject.transform.Translate(position);
        currentEncounter = encounter;
    }
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}

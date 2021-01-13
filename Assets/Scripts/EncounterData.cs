using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Encounter", menuName = "Encounter")]
public class EncounterData : ScriptableObject
{
public new string name;
public EncounterType type;
public bool isCleared = false;
[TextArea(15,20)]
public string description;
}

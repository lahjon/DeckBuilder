using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Artifact : MonoBehaviour
{
    public Rarity rarity;
    public string displayName;
    [TextArea(5,5)]
    public string tooltip;
    public abstract void AddActivity();

    public abstract void RemoveActivity();

}

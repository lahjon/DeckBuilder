using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IconDatabase", menuName = "Database/Icon Database")]
public class IconDatabase : ScriptableObject
{
    public List<Sprite> allIcons;
}

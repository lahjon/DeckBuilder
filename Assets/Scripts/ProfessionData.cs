using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ProfessionData", menuName = "CardGame/ProfessionData")]
public class ProfessionData : ScriptableObject
{
    public int id;
    public string professionName;
    public string professionDescription;
    public Sprite artwork;
    public ProfessionType professionType;
    public List<AbilityData> abilityDatas;
    public List<ItemEffectStruct> itemEffectStructs;
}

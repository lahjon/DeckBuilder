using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ProfessionData", menuName = "CardGame/ProfessionData")]
public class ProfessionData : ScriptableObject
{
    public int id;
    public string professionName;
    public ProfessionType profession;
    public List<AbilityData> abilityDatas;
    public List<ItemEffectStruct> itemEffectStructs;
}

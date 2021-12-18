using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.UI;
using UnityEditor;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using DG.Tweening;


public static class Helpers
{
    readonly static string colorCodeGood = "#2e590c";
    readonly static string colorCodeBad = "#a16658";

    public static Dictionary<CardClassType,Color> borderColors = new Dictionary<CardClassType, Color>{
        {CardClassType.Berserker,           new Color(.57f, .13f, .13f, 1f) }, 
        {CardClassType.Rogue,               new Color(.32f, .63f, .41f, 1f) }, 
        {CardClassType.Splicer,             new Color(.25f, .52f, .95f, 1f) }, 
        {CardClassType.Beastmaster,         new Color(.95f, .80f, .24f, 1f) }, 
        {CardClassType.Colorless,           Color.white }, 
        {CardClassType.Enemy,               Color.white }, 
        {CardClassType.Burden,              Color.white }, 
        {CardClassType.Torment,             Color.black }
     };

    public static Dictionary<ModifierType, string> ModifierToIconName = new Dictionary<ModifierType, string>
    {
        {ModifierType.Cursed, "<sprite name=\"Cursed\">" },
        {ModifierType.Upgrade, "" },
        {ModifierType.UpgradePermanent, "" },
        {ModifierType.Blessed, "<sprite name=\"Blessed\">" },
    };

     public static Dictionary<Rarity,Color> rarityBorderColors = new Dictionary<Rarity, Color>{
        {Rarity.None,       Color.gray }, 
        {Rarity.Starting,   Color.gray }, 
        {Rarity.Common,     Color.gray }, 
        {Rarity.Uncommon,   new Color(.1f, .6f, .1f, 1f) },
        {Rarity.Rare,       new Color(.67f, .16f, .55f, 1f) }
     };

    public static Dictionary<TileType, Dictionary<ScenarioEncounterType, float>> TileTypeToProportions    = new Dictionary<TileType, Dictionary<ScenarioEncounterType, float>> {
        {TileType.Plains, new Dictionary<ScenarioEncounterType, float>{
                    {ScenarioEncounterType.CombatNormal, 1f},
                    {ScenarioEncounterType.Choice, 1f},
                    {ScenarioEncounterType.CombatElite, 1f},
                    {ScenarioEncounterType.Shop, 1f},
                    {ScenarioEncounterType.Blacksmith, 1f},
                    {ScenarioEncounterType.Bonfire, 1f}
        }},
        {TileType.Forest, new Dictionary<ScenarioEncounterType, float>{
                    {ScenarioEncounterType.CombatNormal, 1f},
                    {ScenarioEncounterType.Choice, 1f},
                    {ScenarioEncounterType.CombatElite, 1f},
                    {ScenarioEncounterType.Shop, 1f},
                    {ScenarioEncounterType.Blacksmith, 1f},
                    {ScenarioEncounterType.Bonfire, 1f}
        }},
        {TileType.Cave, new Dictionary<ScenarioEncounterType, float>{
                    {ScenarioEncounterType.CombatNormal, 1f},
                    {ScenarioEncounterType.Choice, 1f},
                    {ScenarioEncounterType.CombatElite, 1f},
                    {ScenarioEncounterType.Shop, 1f},
                    {ScenarioEncounterType.Blacksmith, 1f},
                    {ScenarioEncounterType.Bonfire, 1f}
        }},
        {TileType.BanditCamp, new Dictionary<ScenarioEncounterType, float>{
                    {ScenarioEncounterType.CombatNormal, 1f},
                    {ScenarioEncounterType.Choice, 1f},
                    {ScenarioEncounterType.CombatElite, 1f},
                    {ScenarioEncounterType.Shop, 1f},
                    {ScenarioEncounterType.Blacksmith, 1f},
                    {ScenarioEncounterType.Bonfire, 1f}
        }},
        {TileType.Caravan, new Dictionary<ScenarioEncounterType, float>{
                    {ScenarioEncounterType.CombatNormal, 1f},
                    {ScenarioEncounterType.Choice, 1f},
                    {ScenarioEncounterType.CombatElite, 1f},
                    {ScenarioEncounterType.Shop, 1f},
                    {ScenarioEncounterType.Blacksmith, 1f},
                    {ScenarioEncounterType.Bonfire, 1f}
        }},
        {TileType.Town, new Dictionary<ScenarioEncounterType, float>{
                    {ScenarioEncounterType.CombatNormal, 0f},
                    {ScenarioEncounterType.Choice, 1f},
                    {ScenarioEncounterType.CombatElite, 0f},
                    {ScenarioEncounterType.Shop, 1f},
                    {ScenarioEncounterType.Blacksmith, 1f},
                    {ScenarioEncounterType.Bonfire, 1f}
        }}
    };
    public static Dictionary<TileType, Dictionary<ScenarioEncounterType, int>> tileTypeToFixedAmounts      = new Dictionary<TileType, Dictionary<ScenarioEncounterType, int>> {
        {TileType.Plains, new Dictionary<ScenarioEncounterType, int>{
                    {ScenarioEncounterType.CombatNormal, 0},
                    {ScenarioEncounterType.Choice, 0},
                    {ScenarioEncounterType.CombatElite, 0},
                    {ScenarioEncounterType.Shop, 0},
                    {ScenarioEncounterType.Blacksmith, 0},
                    {ScenarioEncounterType.Bonfire, 0}
        }},
        {TileType.Forest, new Dictionary<ScenarioEncounterType, int>{
                    {ScenarioEncounterType.CombatNormal, 0},
                    {ScenarioEncounterType.Choice, 0},
                    {ScenarioEncounterType.CombatElite, 0},
                    {ScenarioEncounterType.Shop, 0},
                    {ScenarioEncounterType.Blacksmith, 0},
                    {ScenarioEncounterType.Bonfire, 0}
        }},
        {TileType.Cave, new Dictionary<ScenarioEncounterType, int>{
                    {ScenarioEncounterType.CombatNormal, 0},
                    {ScenarioEncounterType.Choice, 0},
                    {ScenarioEncounterType.CombatElite, 0},
                    {ScenarioEncounterType.Shop, 0},
                    {ScenarioEncounterType.Blacksmith, 0},
                    {ScenarioEncounterType.Bonfire, 0}
        }},
        {TileType.BanditCamp, new Dictionary<ScenarioEncounterType, int>{
                    {ScenarioEncounterType.CombatNormal, 0},
                    {ScenarioEncounterType.Choice, 0},
                    {ScenarioEncounterType.CombatElite, 2},
                    {ScenarioEncounterType.Shop, 0},
                    {ScenarioEncounterType.Blacksmith, 0},
                    {ScenarioEncounterType.Bonfire, 0}
        }},
        {TileType.Caravan, new Dictionary<ScenarioEncounterType, int>{
                    {ScenarioEncounterType.CombatNormal, 0},
                    {ScenarioEncounterType.Choice, 0},
                    {ScenarioEncounterType.CombatElite, 0},
                    {ScenarioEncounterType.Shop, 1},
                    {ScenarioEncounterType.Blacksmith, 1},
                    {ScenarioEncounterType.Bonfire, 1}
        }},
        {TileType.Town, new Dictionary<ScenarioEncounterType, int>{
                    {ScenarioEncounterType.CombatNormal, 0},
                    {ScenarioEncounterType.Choice, 0},
                    {ScenarioEncounterType.CombatElite, 0},
                    {ScenarioEncounterType.Shop, 1},
                    {ScenarioEncounterType.Blacksmith, 1},
                    {ScenarioEncounterType.Bonfire, 1}
        }}
    };


    public static Color attackColor = new Color(190,83,83);
    public static Color neutralColor = Color.white;
    public static Color upgradeCardColor = new Color(1f, 0.9f, 0.7f);
    public static Color normalCardColor = new Color(0.7f, 0.7f, 0.7f);
    public static Color cursedCardColor = new Color(0.6f, 0.5f, 0.85f);
    public static float timeMultiplier = 1.0f;

    public static string ToLowerFirstChar(this string input)
    {
        if(string.IsNullOrEmpty(input))
            return input;

        return char.ToLower(input[0]) + input.Substring(1);
    }
    public static string ToUpperFirstChar(this string input)
    {
        if(string.IsNullOrEmpty(input))
            return input;

        return char.ToUpper(input[0]) + input.Substring(1);
    }

    public static void SetDictionaryFromLists<T1, T2>(this Dictionary<T1, T2> dict, List<T1> keys, List<T2> values)
    {
        if (keys != null)
        {   
            for (int i = 0; i < keys.Count; i++)
            {
                dict.Add(keys[i], values[i]);
            }
        }
    }

    public static void DelayForSeconds(float timeDelay, Action callback = null)
    {
        DOTween.To(() => 0, x => { }, 0, timeDelay).OnComplete(() => callback?.Invoke()); 
    }

    public static string ValueColorWrapper(int originalVal, int currentVal, bool inverse = false)
    {
        if ((!inverse && currentVal < originalVal) || (inverse && currentVal > originalVal))
            return "<color=" + colorCodeBad + ">" + currentVal.ToString() + "</color>";
        else if (originalVal == currentVal)
            return currentVal.ToString();
        else
            return "<color=" + colorCodeGood + ">" + currentVal.ToString() + "</color>";
    }


    public static Rarity DrawRarity(float uncommon, float rare)
    {
        float[] probs = new float[] { 1 - uncommon - rare, uncommon, rare };

        int index = 0;
        float rand = UnityEngine.Random.Range(0f, 1f);
        float sum = 0;

        while(sum < rand && index < probs.Length)
            sum += probs[index++];

        return (Rarity)(index + 10);
    }

    public static void SetListsFromDictionary<T1, T2>(this Dictionary<T1, T2> dict, ref List<T1> keys, ref List<T2> values)
    {
        keys = new List<T1>(dict.Keys);
        values = new List<T2>(dict.Values);
    }
    
    public static IEnumerable<T> FindInterfacesOfType<T>(bool includeInactive = true)
    {
        var objs = SceneManager.GetActiveScene().GetRootGameObjects().SelectMany(go => go.GetComponentsInChildren<T>(includeInactive));

        return SceneManager.GetActiveScene().GetRootGameObjects().SelectMany(go => go.GetComponentsInChildren<T>(includeInactive));
    }

    public static Vector3 AngleLerp(Vector3 StartAngle, Vector3 FinishAngle, float t)
    {
        float xLerp = Mathf.LerpAngle(StartAngle.x, FinishAngle.x, t);
        float yLerp = Mathf.LerpAngle(StartAngle.y, FinishAngle.y, t);
        float zLerp = Mathf.LerpAngle(StartAngle.z, FinishAngle.z, t);
        return new Vector3(xLerp, yLerp, zLerp);
    }

    public static T ToEnum<T>(this string str) where T: Enum
    {
        return (T)Enum.Parse(typeof(T), str);
    }

    public static int ToInt(this string str, int EmptyDefaultsTo = 0) 
    { 
        Debug.Log(str);
        return String.IsNullOrEmpty(str) ? EmptyDefaultsTo : int.Parse(str);
    }

    public static bool ToBool(this string str) 
    { 
        return !String.IsNullOrEmpty(str) && str == "true" ? true : false;
    }

    public static float Remap(this float value, float from1, float to1, float from2, float to2) 
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public static float InverseLerp(float a, float b, float v) 
    {
        return ((v - a) / (b - a));
    }

    public static T InstanceObject<T>(string aName) where T : class
    {
        if (Type.GetType(aName) is Type type && (T)Activator.CreateInstance(type, WorldSystem.instance) is T itemEffect)
            return itemEffect;
        else
            return null;
    }

}

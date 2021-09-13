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

     public static Dictionary<Rarity,Color> rarityBorderColors = new Dictionary<Rarity, Color>{
        {Rarity.None,       Color.gray }, 
        {Rarity.Starting,   Color.gray }, 
        {Rarity.Common,     Color.gray }, 
        {Rarity.Uncommon,   new Color(.1f, .6f, .1f, 1f) },
        {Rarity.Rare,       new Color(.67f, .16f, .55f, 1f) }
     }; 

    public static Color attackColor = new Color(190,83,83);
    public static Color neutralColor = Color.white;
    public static Color upgradeCardColor = new Color(1f, 0.9f, 0.7f);
    public static float timeMultiplier = 1.0f;

    public static string ToLowerFirstChar(this string input)
    {
        if(string.IsNullOrEmpty(input))
            return input;

        return char.ToLower(input[0]) + input.Substring(1);
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

    // public static List<T> FindAssetsByType<T>() where T : UnityEngine.Object
    // {
    //     List<T> assets = new List<T>();
    //     string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));
    //     for( int i = 0; i < guids.Length; i++ )
    //     {
    //         string assetPath = AssetDatabase.GUIDToAssetPath( guids[i] );
    //         T asset = AssetDatabase.LoadAssetAtPath<T>( assetPath );
    //         if( asset != null )
    //         {
    //             assets.Add(asset);
    //         }
    //     }
    //     return assets;
    // }

    // public static List<string> GetAllFilesInDirectory(string relativeAssetPath, string extension = "*.cs")
    // {
    //     string path = Application.dataPath;
    //     path = path.Replace("Assets", "");
    //     path += relativeAssetPath;
    //     List<string> allFiles = Directory.GetFiles(path, extension).Select(file => Path.GetFileNameWithoutExtension(file)).ToList();
    //     return allFiles;
    // }

    public static float Remap(this float value, float from1, float to1, float from2, float to2) 
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public static float InverseLerp(float a, float b, float v) 
    {
        return ((v - a) / (b - a));
    }

}

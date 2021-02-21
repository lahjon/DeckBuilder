using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.UI;
using UnityEditor;
using System;
using System.IO;


public static class Helpers
{
    public static string ToLowerFirstChar(this string input)
    {
        if(string.IsNullOrEmpty(input))
            return input;

        return char.ToLower(input[0]) + input.Substring(1);
    }

   // we can have this or choose to store all references we use
    public static IEnumerable<T> FindInterfacesOfType<T>(bool includeInactive = true)
    {
        return SceneManager.GetActiveScene().GetRootGameObjects().SelectMany(go => go.GetComponentsInChildren<T>(includeInactive));
    }

    public static List<T> FindAssetsByType<T>() where T : UnityEngine.Object
    {
        List<T> assets = new List<T>();
        string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));
        for( int i = 0; i < guids.Length; i++ )
        {
            string assetPath = AssetDatabase.GUIDToAssetPath( guids[i] );
            T asset = AssetDatabase.LoadAssetAtPath<T>( assetPath );
            if( asset != null )
            {
                assets.Add(asset);
            }
        }
        return assets;
    }

    public static List<string> GetAllFilesInDirectory(string relativeAssetPath, string extension = "*.cs")
    {
        string path = Application.dataPath;
        path = path.Replace("Assets", "");
        path += relativeAssetPath;
        List<string> allFiles = Directory.GetFiles(path, extension).Select(file => Path.GetFileNameWithoutExtension(file)).ToList();
        return allFiles;
    }


}

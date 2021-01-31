using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.UI;
using System;


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
}

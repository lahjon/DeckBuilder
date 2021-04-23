using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class DebugWindow : EditorWindow
{
    GameObject foundObject;
    
    [MenuItem("Window/Debug")] public static void ShowWindow()
    {
        GetWindow<DebugWindow>("Debug");
    }

    void OnGUI()
    {
        // toggle town visibility
        if(GUILayout.Button("Toggle Town Visiblity"))
        {
            ToggleActiveObject("Town", "TownMap");
        }

        // toggle grid visibility
        if(GUILayout.Button("Toggle Grid Visibility"))
        {
            ToggleActiveObject("--------------MANAGERS--------------", "GridManager");
        }

        // toggle combat visibility
        if(GUILayout.Button("Toggle Combat Visiblity"))
        {
            ToggleActiveObject("CombatScene", "CombatEnvironment");
        }
    }

    void ToggleActiveObject(string parent, string child)
    {
        foundObject = GameObject.Find(parent);

        if (foundObject != null)
        {
            foundObject = foundObject.transform.Find(child).gameObject;
            if (foundObject.activeSelf)
            {
                foundObject.gameObject.SetActive(false);
            }
            else
            {
                foundObject.gameObject.SetActive(true);
            }
        }
    }
}


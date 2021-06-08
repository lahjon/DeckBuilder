using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;

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
            ToggleActiveObject("--------------MAPS--------------", "GridManager");
        }

        // toggle combat visibility
        if(GUILayout.Button("Toggle Combat Visiblity"))
        {
            ToggleActiveObject("--------------COMBAT--------------", "CombatScene");
        }

        // resets all saved data
        if(GUILayout.Button("Reset Game Data"))
        {
            ResetAllData();
        }
    }

    void ResetAllData()
    {
        Directory.GetFiles(Application.persistentDataPath);
        DirectoryInfo dirInfo = new DirectoryInfo(Application.persistentDataPath);
        foreach (FileInfo file in dirInfo.GetFiles())
        {
            Debug.Log(file + " removed");
            file.Delete(); 
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


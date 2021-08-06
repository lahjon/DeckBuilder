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
        #region visibility

        // toggle town visibility
        if(GUILayout.Button("Toggle Town Visiblity"))
        {
            ToggleActiveObject("--------------MAPS--------------", "TownManager", "TownMap");
        }

        // toggle world map visibility
        if(GUILayout.Button("Toggle WorldMap Visiblity"))
        {
            ToggleActiveObject("--------------MAPS--------------", "WorldMapManager", "WorldMapCanvas");
        }

        // toggle grid visibility
        if(GUILayout.Button("Toggle Grid Visibility"))
        {
            ToggleActiveObject("--------------MAPS--------------", "GridManager", "Content");
        }

        // toggle combat visibility
        if(GUILayout.Button("Toggle Combat Visiblity"))
        {
            ToggleActiveObject("--------------COMBAT--------------", "CombatSystem", "Content");
        }

        DrawLine(Color.black, 2, 10);

        #endregion

        #region data

            // resets all saved data
            if(GUILayout.Button("Reset Game Data"))
            {
                ResetAllData();
            }

            DrawLine(Color.black, 2, 10);

        #endregion

        #region database
        if(GUILayout.Button("Update Database"))
        {
            DatabaseUpdateOnStart.UpdateDatabase();
        }

        if(GUILayout.Button("Download from Google"))
        {
            DatabaseUpdateOnStart.UpdateFromGoogle();
        }


        #endregion




    }
    public void DrawLine(Color color, int thickness = 2, int padding = 10)
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding+thickness));
        r.height = thickness;
        r.y+=padding/2;
        r.x-=2;
        r.width +=6;
        EditorGUI.DrawRect(r, color);
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
    void ToggleActiveObject(string parent, string child1, string child2)
    {
        foundObject = GameObject.Find(parent);

        if (foundObject != null)
        {
            foundObject = foundObject.transform.Find(child1).gameObject;
            if (foundObject != null)
            {
                foundObject = foundObject.transform.Find(child2).gameObject;
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
}


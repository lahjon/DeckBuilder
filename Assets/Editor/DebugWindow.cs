using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;

public class DebugWindow : EditorWindow
{
    GameObject foundObject;
    GameObject myTarget;
    

    [MenuItem("Window/Debug")]
    public static void ShowWindow()
    {
        GetWindow<DebugWindow>("Debug");
    }

    void OnGUI()
    {
        #region visibility

        // toggle town visibility
        if (GUILayout.Button("Toggle Town Visiblity"))
        {
            ToggleActiveObject("--------------MAPS--------------", "TownManager", "TownMap");
        }

        // toggle world map visibility
        if (GUILayout.Button("Toggle WorldMap Visiblity"))
        {
            ToggleActiveObject("--------------MAPS--------------", "WorldMapManager", "WorldMapCanvas");
        }

        // toggle grid visibility
        if (GUILayout.Button("Toggle Grid Visibility"))
        {
            ToggleActiveObject("--------------MAPS--------------", "GridManager", "Content");
        }

        // toggle combat visibility
        if (GUILayout.Button("Toggle Combat Visiblity"))
        {
            ToggleActiveObject("--------------COMBAT--------------", "CombatSystem", "Content");
        }

        DrawLine(Color.black, 2, 10);

        #endregion

        #region data

        // resets all saved data
        if (GUILayout.Button("Reset Game Data"))
        {
            ResetAllData();
        }

        DrawLine(Color.black, 2, 10);

        #endregion

        #region database
        // if (GUILayout.Button("Update Database"))
        // {
        //     DatabaseUpdateOnStart.UpdateDatabase();
        // }

        if (GUILayout.Button("Download and Update Database"))
        {
            DatabaseUpdateOnStart.DownloadDatabase();
            DatabaseUpdateOnStart.UpdateDatabase();
        }


        #endregion

        DrawLine(Color.black, 2, 10);

        DrawDragAndDrop(1);

        if (GUILayout.Button("Toggle GameObject"))
        {
            if (myTarget != null)
            {
                if (myTarget.activeSelf)
                {
                    myTarget.SetActive(false);
                }
                else
                {
                    myTarget.SetActive(true);
                }
            }
        }

        if (GUILayout.Button("Select GameObject"))
        {
            if (myTarget != null)
            {
                Selection.objects = new Object[] { myTarget };
            }
        }


        if (GUILayout.Button("Reset"))
        {
            myTarget = null;
        }


        DrawLine(Color.black, 2, 10);

        if (GUILayout.Button("CreateMap"))
        {
            CreateGridMap();
        }


    }
    public void DrawLine(Color color, int thickness = 2, int padding = 10)
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
        r.height = thickness;
        r.y += padding / 2;
        r.x -= 2;
        r.width += 6;
        EditorGUI.DrawRect(r, color);
    }
    
    public void CreateGridMap()
    {
        int gridWidth = 3;
        
        float hexscale = 0.3765092f;
        for (int q = -gridWidth; q <= gridWidth; q++)
        {
            int r1 = Mathf.Max(-gridWidth, -q - gridWidth);
            int r2 = Mathf.Min(gridWidth, -q + gridWidth);

            for (int r = r1; r <= r2; r++)
            {
                GameObject obj = Instantiate(myTarget, CellPosToWorldPos(new Vector3Int(q, r, -q-r)), myTarget.transform.rotation, myTarget.transform);
                obj.transform.localScale = -Vector3.one * hexscale;
            }
        }
    
    }

    public Vector3 CellPosToWorldPos(Vector3Int coord)
    {
        // get world position of the coord
        float tileSize = 1;
        float tileGap = 0;
        float width = Mathf.Sqrt(3) * (tileSize + tileGap);
        float height = 1.5f * (tileSize + tileGap);
        float x = (width * coord.x * 0.5f) - (width * coord.y * 0.5f);
        float y = height * coord.z * -1;

        return new Vector3(x, y, 0);
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

    void DrawDragAndDrop(int m)
    {

        Event evt = Event.current;

        GUIStyle GuistyleBoxDND = new GUIStyle(GUI.skin.box);
        GuistyleBoxDND.alignment = TextAnchor.MiddleCenter;
        GuistyleBoxDND.fontStyle = FontStyle.Italic;
        GuistyleBoxDND.fontSize = 12;
        GUI.skin.box = GuistyleBoxDND;


        Rect dadRect = new Rect();
        dadRect = GUILayoutUtility.GetRect(0, 20, GUILayout.ExpandWidth(true));
        if (myTarget == null)
        {
            GUI.Box(dadRect, "Drag and Drop gameobjects to this box!", GuistyleBoxDND);
            //GuistyleBoxDND.normal.background = new Texture2D(2, 2);
        }
        else
        {
            GUI.Box(dadRect, myTarget.name, GuistyleBoxDND);
            //GuistyleBoxDND.normal.background = new Texture2D(2, 2);
        }

        if (dadRect.Contains(Event.current.mousePosition))
        {
            if (Event.current.type == EventType.DragUpdated)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                Event.current.Use();
            }
            else if (Event.current.type == EventType.DragPerform)
            {
                for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
                {
                    myTarget = DragAndDrop.objectReferences[i] as GameObject;
                }

                Event.current.Use();
            }
        }
    }
}


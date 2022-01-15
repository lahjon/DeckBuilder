using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class TileCreatorWindow : EditorWindow
{
    string[] options = new string[]{"Encounter Placement", "Road Placement", "Encounter Edit", };
    int selectedOptions = 0;
    int selectedEncOptions = 0;
    int selectedEncDirectionOptions = 0;
    
    string[] encTypeOptions = new string[]
    {
        "None", "Exit", "Start", "Story"
    };
    string[] encDirectionOptions = new string[]
    {
        "East", "NorthEast", "NorthWest", "West", "SouthWest", "SouthEast"
    };

    TileCreator tileCreator;
    bool active = false;

    [MenuItem("Tools/Tile Creator")]
    public static void ShowWindow()
    {
        GetWindow(typeof(TileCreatorWindow));
    }

    public static void OpenWindow(TileCreator aTileCreator)
    {
        GetWindow(typeof(TileCreatorWindow));
    }

    void ResetTile()
    {
        HexTileOverworld tile = tileCreator.hexTile;
        // tile.encounters.ForEach(x => DestroyImmediate(x.gameObject));
            
        // tile.encounters.Clear();
        // tile.encountersExits.Clear();
        tile.neighbours.Clear();
        // tile.encounterEntry = null;
        //tileCreator.hexTile.availableDirections.Clear();
    }

    void ButtonStart()
    {
        if(GUILayout.Button("Start"))
            {
                AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath("Assets/Prefab/TileCreator.prefab", typeof(GameObject)));
                GameObject rootParent = (GameObject)Selection.objects[0];
                tileCreator = rootParent.GetComponent<TileCreator>();
                ActiveEditorTracker.sharedTracker.isLocked = true;
                Selection.objects = new Object[0];
                ActiveEditorTracker.sharedTracker.ForceRebuild();
                selectedOptions = 0;
                Undo.FlushUndoRecordObjects();
                tileCreator?.ResetEncounters();
                SceneVisibilityManager.instance.DisablePicking(tileCreator.gameObject, true);
                SceneView.RepaintAll();
                active = true;
                SetStartEncounters();
            }
    }   

    void SetStartEncounters()
    {
        tileCreator.allEncounters = new List<TileCreatorEncounter>();
        tileCreator.allEncounters.Add(new TileCreatorEncounter(new Vector3(0.6f, 0, 0), ScenarioEncounterType.Exit, new GridDirection(GridDirection.DirectionName.East), 0));
        tileCreator.allEncounters.Add(new TileCreatorEncounter(new Vector3(0.3f, 0.6f, 0), ScenarioEncounterType.Exit, new GridDirection(GridDirection.DirectionName.NorthEast), 1));
        tileCreator.allEncounters.Add(new TileCreatorEncounter(new Vector3(-0.3f, 0.6f, 0), ScenarioEncounterType.Exit, new GridDirection(GridDirection.DirectionName.NorthWest), 2));
        tileCreator.allEncounters.Add(new TileCreatorEncounter(new Vector3(-0.6f, 0, 0), ScenarioEncounterType.Exit, new GridDirection(GridDirection.DirectionName.West), 3));
        tileCreator.allEncounters.Add(new TileCreatorEncounter(new Vector3(-0.3f, -0.6f, 0), ScenarioEncounterType.Exit, new GridDirection(GridDirection.DirectionName.SouthWest), 4));
        tileCreator.allEncounters.Add(new TileCreatorEncounter(new Vector3(0.3f, -0.6f, 0), ScenarioEncounterType.Exit, new GridDirection(GridDirection.DirectionName.SouthEast), 5));
        tileCreator.allEncounters.Add(new TileCreatorEncounter(Vector3.zero, ScenarioEncounterType.Start, new GridDirection(GridDirection.DirectionName.East), 6));
    }

    void ShowTextLabels()
    {
        string tileText = tileCreator.encounterPosition == Vector3.zero ? "None" : tileCreator.GetEncounterIndex().ToString();
        EditorGUILayout.LabelField("Index: " + tileText, EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Position: " + tileCreator.encounterPosition.ToString(), EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Type: " + tileCreator.encounterType, EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Neighbours: " + tileCreator.encounterNeighbours, EditorStyles.boldLabel);
        if (tileCreator.encounterType == "Exit")
            EditorGUILayout.LabelField("Exit Direction: " + tileCreator.encounterDirection, EditorStyles.boldLabel);
    }

    void ButtonCreate()
    {
        if(GUILayout.Button("Create Tile"))
        {
            List<Encounter> newEncs = new List<Encounter>();
            foreach (var enc in tileCreator.allEncounters)
            {
                enc.position.z = 0;
                Encounter newEnc = Instantiate(tileCreator.encounterPrefab, enc.position, Quaternion.identity, tileCreator.encounterParent).GetComponent<Encounter>();
                newEncs.Add(newEnc);
                newEnc.tile = tileCreator.hexTile;
                newEnc.SetEncounterType(enc.overworldEncounterType);
            }
            for (int i = 0; i < tileCreator.allEncounters.Count; i++)
            {
                newEncs[i].neighboors = tileCreator.allEncounters[i].neighbourIndex.Select(x => newEncs[x]).ToList();
            }
            // tileCreator.hexTile.encounters = newEncs;


            bool add;
            List<EncounterEdge> edges = new List<EncounterEdge>();
            for (int i = 0; i < newEncs.Count; i++)
            {
                List<EncounterEdge> tempEdges = new List<EncounterEdge>();
                newEncs[i].neighboors.ForEach(x => tempEdges.Add(new EncounterEdge(newEncs[i], x)));
                foreach (var tEdge in tempEdges)
                {
                    add = true;
                    foreach (var edge in edges)
                        if (edge.Equals(tEdge))
                            add = false;
                    if (add)
                        edges.Add(tEdge);
                }
            }

            int id = System.IO.Directory.GetFiles(Application.dataPath + "/Prefab/HexTiles/").Count() / 2;
            tileCreator.hexTile.id = id;
            EncounterManager encounterManager = GameObject.Find("EncounterManager")?.GetComponent<EncounterManager>();
            DatabaseSystem db = GameObject.Find("DatabaseSystem")?.GetComponent<DatabaseSystem>();

            foreach (var item in edges)
                encounterManager.AddRoad(item.n1, item.n2, false, 1f / 0.392f, true);
            
            // foreach (TileCreatorEncounter exit in tileCreator.allEncounters.Where(x => x.overworldEncounterType  == ScenarioEncounterType.Exit))
            //     tileCreator.hexTile.availableDirections.Add(exit.direction);

            bool success;
            int tileNumber = id;
            string newPath = string.Format("Assets/Prefab/HexTiles/HexTile{0}.prefab", tileNumber);
            GameObject newPrefab = PrefabUtility.SaveAsPrefabAsset(tileCreator.hexTile.gameObject, newPath, out success);

            if (success)
            {
                DatabaseUpdateOnStart.UpdateAllHexTiles();
                Debug.Log("Creating new tile successful!");
            }
            else
                Debug.LogWarning("Creating new tile failed!");



            ResetTile();
        }
    }

    void ButtonReset()
    {
        if(GUILayout.Button("Reset"))
        {
            tileCreator.ResetEncounters();
            SetStartEncounters();
            SceneView.RepaintAll();
        }
    }
    void ButtonEnd()
    {
        if(GUILayout.Button("End"))
        {
            ActiveEditorTracker.sharedTracker.isLocked = false;
            SceneView.RepaintAll();
            if (tileCreator != null) SceneVisibilityManager.instance.EnablePicking(tileCreator.gameObject, true);
            active = false;
        }
    }

    void EncountePopups()
    {
        selectedOptions = EditorGUILayout.Popup("Encounter Options: ", selectedOptions, options); 
        if (tileCreator != null && tileCreator.optionMode != selectedOptions)
            tileCreator.optionMode = selectedOptions;
        if (tileCreator != null && tileCreator.optionExitDirection != selectedEncDirectionOptions)
            tileCreator.optionExitDirection = selectedEncDirectionOptions;

        selectedEncOptions = EditorGUILayout.Popup("Encounter Type: ", selectedEncOptions, encTypeOptions); 
        if (tileCreator != null && tileCreator.optionType != selectedEncOptions)
            tileCreator.optionType = selectedEncOptions;
        
        if (tileCreator.encounterType == "Exit" || tileCreator.optionType == 1)
            selectedEncDirectionOptions = EditorGUILayout.Popup("Exit Direction: ", selectedEncDirectionOptions, encDirectionOptions); 
    }

    void OnGUI()
    {
        if (!active)
            ButtonStart();

        if (active)
        {
            ShowTextLabels();
            GUILayout.Space(20);
            EncountePopups();
            GUILayout.Space(20);
            ButtonCreate();
            ButtonReset();
            ButtonEnd();
            
        }
        Repaint();
    }
}

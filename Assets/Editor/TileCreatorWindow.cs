using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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

    void OnGUI()
    {

        if (!active)
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
            }
        }

        if (active)
        {
            string tileText = tileCreator.encounterPosition == Vector3.zero ? "None" : tileCreator.GetEncounterIndex().ToString();
            EditorGUILayout.LabelField("Index: " + tileText, EditorStyles.boldLabel);
            
            EditorGUILayout.LabelField("Position: " + tileCreator.encounterPosition.ToString(), EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Type: " + tileCreator.encounterType, EditorStyles.boldLabel);
            if (tileCreator.encounterType == "Exit")
                EditorGUILayout.LabelField("Exit Direction: " + tileCreator.encounterDirection, EditorStyles.boldLabel);
            
            GUILayout.Space(20);

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

            GUILayout.Space(20);

            if(GUILayout.Button("Reset"))
            {
                Debug.Log("Reset");
                tileCreator.ResetEncounters();
                SceneView.RepaintAll();
            }
            if(GUILayout.Button("End"))
            {
                ActiveEditorTracker.sharedTracker.isLocked = false;
                SceneView.RepaintAll();
                if (tileCreator != null) SceneVisibilityManager.instance.EnablePicking(tileCreator.gameObject, true);
                active = false;
            }
        }
        

        Repaint();
    }
}

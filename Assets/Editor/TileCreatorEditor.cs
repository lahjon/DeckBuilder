using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Text;

[CustomEditor (typeof(TileCreator))]
public class TileCreatorEditor : Editor
{
    public TileCreator tileCreator;
    void OnSceneGUI()
    {
        if (tileCreator.active)
        {
            Draw();
            InputMouse();
        }
    }

    void InputMouse()
    {
        Event guiEvent = Event.current;

        if (guiEvent.type != EventType.MouseDown)
            return;

        Ray mousePos = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);
        Plane plane = new Plane(Vector3.forward, Vector3.zero);
        Vector3 hitPos = Vector3.zero;
        
        if(plane.Raycast(mousePos, out float distance))
            hitPos = mousePos.GetPoint(distance);

        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0)
        {
            if(GetPoint(hitPos) is TileCreatorEncounter enc)
            {
                tileCreator.encounterPosition = enc.position;
                tileCreator.encounterType = enc.overworldEncounterType.ToString();
                tileCreator.encounterDirection = enc.direction.Name.ToString();
                if ( enc.neighbourIndex.Count > 0)
                {
                    var sb = new StringBuilder();
                    for (int i = 0; i < enc.neighbourIndex.Count; i++)
                    {
                        sb.Append(enc.neighbourIndex[i]);
                        if (i < enc.neighbourIndex.Count - 1)
                            sb.Append(" | ");
                    }
                    tileCreator.encounterNeighbours = sb.ToString();
                }
                else
                    tileCreator.encounterNeighbours = "None";
            }
            else
            {
                tileCreator.encounterPosition = Vector3.zero;
                tileCreator.encounterType = "None";
                tileCreator.encounterNeighbours = "None";
            }
        }

        // encounter placement mode
        if (tileCreator.optionMode == 0)
        {
            // add encounter
            if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.shift)
            {
                Undo.RecordObject(tileCreator, "Add Encounter");
                tileCreator.AddEncounter(hitPos);
                guiEvent.Use();
            }
            // remove encounter
            else if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.control)
            {
                if (GetPoint(hitPos) is TileCreatorEncounter hitEnc)
                {
                    Undo.RecordObject(tileCreator, "Remove point");
                    tileCreator.RemovePoint(hitEnc.position);
                    guiEvent.Use();
                }
            }
        }
        // road placement mode
        else if(tileCreator.optionMode == 1)
        {
            // start new neighbours placement
            if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.shift)
            {
                if (GetPoint(hitPos) is TileCreatorEncounter hitEnc)
                {
                    Undo.RecordObject(tileCreator, "Add Roads");
                    tileCreator.MakeNeighbour(hitEnc);
                    tileCreator.currentEnc = hitEnc;
                    guiEvent.Use();
                }
            }
            // remove neighbours
            else if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0&& guiEvent.control)
            {
                if (GetPoint(hitPos) is TileCreatorEncounter hitEnc)
                {
                    Undo.RecordObject(tileCreator, "Add Roads");
                    tileCreator.RemoveNeighbour(hitEnc);
                    guiEvent.Use();
                }
            }
            // add neighbours
            else if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0)
            {
                if (GetPoint(hitPos) is TileCreatorEncounter hitEnc)
                {
                    Undo.RecordObject(tileCreator, "Start Road Placement");
                    tileCreator.currentEnc = hitEnc;
                    guiEvent.Use();
                }

            }
        }
        // encounter edit mode
        else if(tileCreator.optionMode == 2)
        {
            if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0)
            {
                if (GetPoint(hitPos) is TileCreatorEncounter hitEnc)
                {
                    Undo.RecordObject(tileCreator, "Swap Exit");
                    tileCreator.MakeEncounterType(hitEnc.position);
                    guiEvent.Use();
                }
            }
        }
    }

    

    TileCreatorEncounter GetPoint(Vector3 hitPos)
    {
        for (int i = 0; i < tileCreator.allEncounters.Count; i++)
            if (Vector3.Distance(hitPos, tileCreator.allEncounters[i].position) <= .1f)
                return tileCreator.allEncounters[i];
        return null;
    }

    void Draw()
    {
        for (int i = 0; i < tileCreator.allEncounters.Count; i++)
        {
            // draw roads
            foreach (int idx in tileCreator.allEncounters[i].neighbourIndex)
            {
                Handles.color = Color.green;
                Handles.DrawLine(tileCreator.allEncounters[i].position, tileCreator.allEncounters[idx].position, 5f);
            }
            if (tileCreator.allEncounters[i].overworldEncounterType == OverworldEncounterType.Exit)
                Handles.color = Color.red;
            else if(tileCreator.allEncounters[i].overworldEncounterType == OverworldEncounterType.Start)
                Handles.color = Color.green;
            else if(tileCreator.allEncounters[i].overworldEncounterType == OverworldEncounterType.Story)
                Handles.color = Color.blue;
            else
                Handles.color = Color.white;
            Vector3 newPos = Handles.FreeMoveHandle(tileCreator.allEncounters[i].position, Quaternion.identity, .14f, Vector3.zero, Handles.CylinderHandleCap);
            if (tileCreator.allEncounters[i].position != newPos && tileCreator.optionMode != 1)
            {
                Undo.RecordObject(tileCreator, "Move Point");
                tileCreator.MovePoint(i, newPos);
            }
            Handles.color = Color.green;
            Handles.Label(tileCreator.allEncounters[i].position + new Vector3(0.06f,-0.06f,0), string.Format("{0}", i));
        }
    }

    void OnEnable()
    {
        tileCreator = (TileCreator)target;
    }
}
 
/// <summary>
/// This class contain custom drawer for ReadOnly attribute.
/// </summary>
[CustomPropertyDrawer(typeof(ReadOnly))]
public class ReadOnlyDrawer : PropertyDrawer
{
    /// <summary>
    /// Unity method for drawing GUI in Editor
    /// </summary>
    /// <param name="position">Position.</param>
    /// <param name="property">Property.</param>
    /// <param name="label">Label.</param>
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Saving previous GUI enabled value
        var previousGUIState = GUI.enabled;
        // Disabling edit for property
        GUI.enabled = false;
        // Drawing Property
        EditorGUI.PropertyField(position, property, label);
        // Setting old GUI enabled value
        GUI.enabled = previousGUIState;
    }
}
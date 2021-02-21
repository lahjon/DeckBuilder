// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEditor;
// using System.Linq;

// [CustomEditor (typeof(Mission))]
// public class MissionEditor : Editor
// {
//     string[] eventChoice;
//     string[] strMissionChoice;
//     Mission[] missionChoice;
//     string newEvent;
//     int eventChoiceIndex = 0;
//     int missionchoiceIndex = 0;

//     void OnEnable()
//     {
//         eventChoice = Helpers.GetAllFilesInDirectory("Assets/Scripts/GameEvent/GameEvents").ToArray();
//         strMissionChoice = Helpers.GetAllFilesInDirectory("Assets/Missions", "*.asset").ToArray();
//         missionChoice = Helpers.FindAssetsByType<Mission>().ToArray();
//         Debug.Log(missionchoiceIndex);
//     }

//     private string[] GetAllEvents()
//     {
//         List<string> allEvents = new List<string>(); 

//         foreach (var item in Helpers.FindAssetsByType<Mission>())
//         {
//             allEvents.Add(item.name);
//         }
//         return allEvents.ToArray();
//     }

//     public override void OnInspectorGUI()
//     {
//         serializedObject.Update();
//         DrawDefaultInspector();
        
//         eventChoiceIndex = EditorGUILayout.Popup("StartEvent",eventChoiceIndex, eventChoice);
//         Mission mission = target as Mission;
//         mission.startEvent = eventChoice[eventChoiceIndex];

//         missionchoiceIndex = EditorGUILayout.Popup("NextMission",missionchoiceIndex, strMissionChoice);
//         mission.nextMission = missionChoice[missionchoiceIndex];
        
//         EditorUtility.SetDirty(target);
//     }
// }

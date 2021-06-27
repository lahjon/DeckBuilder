// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEditor;
// using System.Linq;
// using System.IO;
// using UnityEditor.SceneManagement;

// [CustomEditor(typeof(ItemData))]
// public class ItemDataEditor : Editor
// {
//     string[] _choices =  new[] { "UpdateMe"};
//     int _choiceIndex = 0;

//     void UpdateConditions()
//     {
//         _choices = Helpers.GetAllFilesInDirectory("Assets/Scripts/Items").ToArray();
//     }

//     void OnEnable()
//     {
//         //UpdateConditions();
//     }
//     public override void OnInspectorGUI ()
//     {
//         DrawDefaultInspector();
//         _choiceIndex = EditorGUILayout.Popup(_choiceIndex, _choices);
//         ItemData itemData = target as ItemData;
//         itemData.effectName = _choices[_choiceIndex].ToString();

//         if(GUILayout.Button("Update Conditions"))
//         {
//             UpdateConditions();
//         }

//         EditorUtility.SetDirty(target);
//     }

    


// }
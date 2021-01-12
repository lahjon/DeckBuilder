using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(DatabaseSystem))]
public class DatabaseEditor : Editor
{

    private List<CardData> GetAllCards()
    {
        List<CardData> cards = new List<CardData>();
 
         string[] lGuids = AssetDatabase.FindAssets("t:CardData", new string[] { "Assets/Cards" });
 
         for (int i = 0; i < lGuids.Length; i++)
         {
             string lAssetPath = AssetDatabase.GUIDToAssetPath(lGuids[i]);
             cards.Add(AssetDatabase.LoadAssetAtPath<CardData>(lAssetPath));
         }
         return cards;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DatabaseSystem database = (DatabaseSystem)target;
        if(GUILayout.Button("Update Database"))
        {
            database.FetchCards(GetAllCards());
        }
    }
}

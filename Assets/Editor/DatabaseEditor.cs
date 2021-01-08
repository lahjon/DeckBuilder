using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(Database))]
public class DatabaseEditor : Editor
{

    private List<Card> GetAllCards()
    {
        List<Card> cards = new List<Card>();
 
         string[] lGuids = AssetDatabase.FindAssets("t:Card", new string[] { "Assets/Cards" });
 
         for (int i = 0; i < lGuids.Length; i++)
         {
             string lAssetPath = AssetDatabase.GUIDToAssetPath(lGuids[i]);
             cards.Add(AssetDatabase.LoadAssetAtPath<Card>(lAssetPath));
         }
         return cards;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Database database = (Database)target;
        if(GUILayout.Button("Update Database"))
        {
            database.FetchCards(GetAllCards());
        }
    }
}

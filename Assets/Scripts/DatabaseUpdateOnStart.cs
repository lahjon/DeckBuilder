using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public static class DatabaseUpdateOnStart
{
    [MenuItem("Edit/Play-Unplay, Update Database %0")]
    static void UpdateAllCards()
    {
        List<CardData> cards = new List<CardData>();

        string[] lGuids = AssetDatabase.FindAssets("t:CardData", new string[] { "Assets/Cards" });

        for (int i = 0; i < lGuids.Length; i++)
        {
            string lAssetPathCard = AssetDatabase.GUIDToAssetPath(lGuids[i]);
            cards.Add(AssetDatabase.LoadAssetAtPath<CardData>(lAssetPathCard));
        }
        string[] guids1 = AssetDatabase.FindAssets("l:CardDatabase", null);
        CardDatabase cardDatabase = (CardDatabase)AssetDatabase.LoadAssetAtPath("Assets/Database/CardDatabase.asset", typeof(CardDatabase));
        cardDatabase.UpdateDatabase(cards);
        Debug.Log("Database updated!");

        EditorApplication.isPlaying = true;
    }

}

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

        EditorUtility.SetDirty(cardDatabase);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Database updated!");

        EditorApplication.isPlaying = true;
    }

    [MenuItem("Edit/Download GoogleCards %#C")]
    static void UpdateFromGoogle()
    {
        DatabaseGoogle google = new DatabaseGoogle();
        google.DownloadCards(); 

        Debug.Log("Googled the cards bro!");
    }

    [MenuItem("Edit/Download GoogleEnemies %#E")]
    static void UpdateFromGoogleEnemies()
    {
        DatabaseGoogle google = new DatabaseGoogle();
        google.DownloadEnemies();

        Debug.Log("Googled the enemies bro!");
    }

    [MenuItem("Edit/Download GoogleEnemies %#O")]
    static void UpdateFromGoogleEncounters()
    {
        DatabaseGoogle google = new DatabaseGoogle();
        google.DownloadEncounters();

        Debug.Log("Googled the combatEncounters bro!");
    }


    [MenuItem("Edit/Upload GoogleCards %H")]
    static void UploadToGoogle()
    {
        DatabaseGoogle google = new DatabaseGoogle();
        google.PrintCardData();

        Debug.Log("Uploaded the cards bro!");
    }
}

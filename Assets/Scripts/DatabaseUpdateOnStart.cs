using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

[ExecuteInEditMode]
public static class DatabaseUpdateOnStart
{
    [MenuItem("Edit/Play-Unplay, Update Database %0")]
    public static void UpdateDatabase(bool startPlaying = true)
    {
        UpdateAllCards();
        UpdateAllTokens();
        UpdateAllEncounters();
        UpdateAllArtifacts();
        UpdateAllCharacters();
        UpdateUImanager();

        EditorApplication.isPlaying = startPlaying;
        Debug.Log("Updated Database");
    }
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
    }

    [MenuItem("Edit/UpdateAllEncounters")]
    static void UpdateAllEncounters()
    {
        GameObject GO_DatabaseSystem = GameObject.Find("DatabaseSystem");
        DatabaseSystem dbs = GO_DatabaseSystem.GetComponent<DatabaseSystem>();

        dbs.encounterEvent.Clear();
        dbs.encountersCombat.Clear();

        List<string> lGuids = new List<string>(AssetDatabase.FindAssets("t:EncounterData", new string[] { "Assets/Encounters/Overworld/Combat" }));
        lGuids.AddRange(AssetDatabase.FindAssets("t:EncounterData", new string[] { "Assets/Encounters/Overworld/RandomEvent" }));

        for (int i = 0; i < lGuids.Count; i++)
        {
            string lAssetPath = AssetDatabase.GUIDToAssetPath(lGuids[i]);
            EncounterData enc = AssetDatabase.LoadAssetAtPath<EncounterData>(lAssetPath);
            if (enc is EncounterDataCombat encComb)
                dbs.encountersCombat.Add(encComb);
            else if (enc is EncounterDataRandomEvent encEv)
                dbs.encounterEvent.Add(encEv);

        }

        EditorUtility.SetDirty(dbs);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    static void UpdateAllTokens()
    {
        GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefab/TokenManager.prefab", typeof(GameObject));
        List<TokenData> tokens = prefab.GetComponent<TokenManager>().tokenDatas;
        tokens.Clear();

        string[] lGuids = AssetDatabase.FindAssets("t:TokenData", new string[] { "Assets/Tokens" });


        for (int i = 0; i < lGuids.Length; i++)
        {
            string lAssetPath = AssetDatabase.GUIDToAssetPath(lGuids[i]);
            tokens.Add(AssetDatabase.LoadAssetAtPath<TokenData>(lAssetPath));
        }

        EditorUtility.SetDirty(prefab);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    static void UpdateAllArtifacts()
    {
        GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefab/ArtifactManager.prefab", typeof(GameObject));
        List<ArtifactData> artifacts = prefab.GetComponent<ArtifactManager>().allArtifacts;
        artifacts.Clear();

        string[] lGuids = AssetDatabase.FindAssets("t:ArtifactData", new string[] { "Assets/Artifacts" });

        for (int i = 0; i < lGuids.Length; i++)
        {
            string lAssetPath = AssetDatabase.GUIDToAssetPath(lGuids[i]);
            artifacts.Add(AssetDatabase.LoadAssetAtPath<ArtifactData>(lAssetPath));
        }

        EditorUtility.SetDirty(prefab);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    static void UpdateAllCharacters()
    {
        GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefab/CharacterManager.prefab", typeof(GameObject));
        List<PlayableCharacterData> data = prefab.GetComponent<CharacterManager>().allCharacterData;
        data.Clear();

        string[] lGuids = AssetDatabase.FindAssets("t:PlayableCharacterData", new string[] { "Assets/CharacterClass/Heroes" });

        for (int i = 0; i < lGuids.Length; i++)
        {
            string lAssetPath = AssetDatabase.GUIDToAssetPath(lGuids[i]);
            data.Add(AssetDatabase.LoadAssetAtPath<PlayableCharacterData>(lAssetPath));
        }

        EditorUtility.SetDirty(prefab);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    [MenuItem("Edit/DELETE THIS %T")]
    static void UpdateUImanager()
    {
        GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefab/UIManager.prefab", typeof(GameObject));
        UIManager manager = prefab.GetComponent<UIManager>();
        manager.namedEffectIcons.Clear();

        string[] lGuids = AssetDatabase.FindAssets("t:Sprite", new string[] { "Assets/Artwork/Effects" });

        for (int i = 0; i < lGuids.Length; i++)
        {
            string lAssetPath = AssetDatabase.GUIDToAssetPath(lGuids[i]);
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(lAssetPath);
            manager.namedEffectIcons.Add(new NamedSprite { name = sprite.name, sprite = sprite });
        }

        EditorUtility.SetDirty(manager);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
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

    [MenuItem("Edit/Download GoogleEncounters %#O")]
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

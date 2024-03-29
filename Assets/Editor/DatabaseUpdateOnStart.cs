using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

[ExecuteInEditMode]
public static class DatabaseUpdateOnStart
{

    [MenuItem("Edit/PlayUnplay Update Database %0")]
    public static void StartGameAndUpdate()
    {
        UpdateDatabase();
        EditorApplication.isPlaying = true;
    }

    public static void UpdateDatabase()
    {
        UpdateAllCards();
        UpdateAllTokens();
        UpdateAllEncounters();
        UpdateAllArtifacts();
        UpdateAllCharacters();
        UpdateUImanager();
        UpdateAllEncounterIcons();
        UpdateAllScenarios();

        Debug.Log("Updated Database");
    }

    public static void DownloadDatabase()
    {
        DownloadFromGoogleCards();
        DownloadFromGoogleEncounters();
        UpdateFromGoogleArtifacts();
        DownloadFromGoogleScenarios();

        Debug.Log("Updated Database");
    }

    private static void UpdateAllEncounterIcons()
    {
        //DatabaseSystem.instance.allOverworldIcons;
    }

    static void UpdateAllCards()
    {
        GameObject GO_DatabaseSystem = GameObject.Find("DatabaseSystem");
        DatabaseSystem dbs = GO_DatabaseSystem.GetComponent<DatabaseSystem>();
        dbs.cards.Clear();

        string[] lGuids = AssetDatabase.FindAssets("t:CardData", new string[] { "Assets/Cards" });

        for (int i = 0; i < lGuids.Length; i++)
        {
            string lAssetPathCard = AssetDatabase.GUIDToAssetPath(lGuids[i]);
            dbs.cards.Add(AssetDatabase.LoadAssetAtPath<CardData>(lAssetPathCard));
        }

        EditorUtility.SetDirty(dbs);
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

        string[] lGuids = AssetDatabase.FindAssets("t:TokenData", new string[] { "Assets/ItemData/Tokens" });


        for (int i = 0; i < lGuids.Length; i++)
        {
            string lAssetPath = AssetDatabase.GUIDToAssetPath(lGuids[i]);
            tokens.Add(AssetDatabase.LoadAssetAtPath<TokenData>(lAssetPath));
        }

        EditorUtility.SetDirty(prefab);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public static void UpdateAllHexTiles()
    {
        GameObject GO_DatabaseSystem = GameObject.Find("DatabaseSystem");
        DatabaseSystem dbs = GO_DatabaseSystem.GetComponent<DatabaseSystem>();
        dbs.hexTiles.Clear();

        string[] lGuids = AssetDatabase.FindAssets("t:Prefab", new string[] { "Assets/Prefab/HexTiles" });

        for (int i = 0; i < lGuids.Length; i++)
        {
            string lAssetPath = AssetDatabase.GUIDToAssetPath(lGuids[i]);
            dbs.hexTiles.Add(AssetDatabase.LoadAssetAtPath<GameObject>(lAssetPath).GetComponent<HexTile>());
        }

        EditorUtility.SetDirty(dbs);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    static void UpdateAllScenarios()
    {
        GameObject GO_DatabaseSystem = GameObject.Find("DatabaseSystem");
        DatabaseSystem dbs = GO_DatabaseSystem.GetComponent<DatabaseSystem>();
        dbs.scenarios.Clear();

        string[] lGuids = AssetDatabase.FindAssets("t:ScenarioData", new string[] { DatabaseGoogle.ScenarioPath});

        for (int i = 0; i < lGuids.Length; i++)
        {
            string lAssetPath = AssetDatabase.GUIDToAssetPath(lGuids[i]);
            dbs.scenarios.Add(AssetDatabase.LoadAssetAtPath<ScenarioData>(lAssetPath));
        }

        EditorUtility.SetDirty(dbs);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }


    static void UpdateAllArtifacts()
    {
        GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefab/ArtifactManager.prefab", typeof(GameObject));
        List<ArtifactData> artifacts = prefab.GetComponent<ArtifactManager>().allArtifacts;
        artifacts.Clear();

        string[] lGuids = AssetDatabase.FindAssets("t:ArtifactData", new string[] { "Assets/ItemData/Artifacts" });

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
        GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefab/Actors/CharacterManager.prefab", typeof(GameObject));
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

        string[] lGuids = AssetDatabase.FindAssets("t:Sprite", new string[] { "Assets/Art/Artwork/Effects" });

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
    public static void DownloadFromGoogleCards()
    {
        DatabaseGoogle google = new DatabaseGoogle();
        google.DownloadCards(); 

        Debug.Log("Googled the cards bro!");
    }

    [MenuItem("Edit/Download Dialogues %#D")]
    public static void DownloadFromGoogleDialogues()
    {
        DatabaseGoogle google = new DatabaseGoogle();
        google.DownloadDialogues();

        Debug.Log("Googled the dialogues bro!");
    }

    [MenuItem("Edit/Download GoogleEnemies %#E")]
    public static void DownloadFromGoogleEnemies()
    {
        DatabaseGoogle google = new DatabaseGoogle();
        google.DownloadEnemies();

        Debug.Log("Googled the enemies bro!");
    }

    [MenuItem("Edit/Download GoogleEncounters %#O")]
    public static void DownloadFromGoogleEncounters()
    {
        DatabaseGoogle google = new DatabaseGoogle();
        google.DownloadEncounters();

        Debug.Log("Googled the combatEncounters bro!");
    }

    [MenuItem("Edit/Download GoogleItemUsables %#I")]
    public static void DownloadFromGoogleItemUsables()
    {
        DatabaseGoogle google = new DatabaseGoogle();
        google.DownloadItemUsables();

        Debug.Log("Googled the usable items bro!");
    }

    [MenuItem("Edit/Download GoogleItemUsables %#M")]
    public static void DownloadFromGoogleMissions()
    {
        DatabaseGoogle google = new DatabaseGoogle();
        google.DownloadMissions();

        Debug.Log("Googled the missions bro!");
    }

    [MenuItem("Edit/Download GoogleScenarios %#S")]
    public static void DownloadFromGoogleScenarios()
    {
        DatabaseGoogle google = new DatabaseGoogle();
        google.DownloadScenarios();

        Debug.Log("Googled the scenarios bro!");
    }


    [MenuItem("Edit/Upload GoogleCards %H")]
    public static void UploadToGoogle()
    {
        DatabaseGoogle google = new DatabaseGoogle();
        google.PrintCardData();

        Debug.Log("Uploaded the cards bro!");
    }

    [MenuItem("Edit/Download GoogleArtifacts %#A")]
    public static void UpdateFromGoogleArtifacts()
    {
        DatabaseGoogle google = new DatabaseGoogle();
        google.ReadEntriesArtifacts("Artifact","Z");

        Debug.Log("Googled the artifacts bro!");
    }


    [MenuItem("Edit/Download GooglePerks %#R")]
    public static void UpdateFromGooglePerks()
    {
        DatabaseGoogle google = new DatabaseGoogle();
        google.ReadEntriesPerks("Perk", "Z");

        Debug.Log("Googled the perks bro!");
    }
}

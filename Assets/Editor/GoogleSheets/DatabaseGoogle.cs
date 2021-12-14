using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Linq;


public class DatabaseGoogle
{

    readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
    readonly string ApplicationName = "Dieathlon";
    Dictionary<string, string> SpreadSheetIDs = new Dictionary<string, string>();

    readonly public static string CardPath = @"Assets\Cards";
    readonly public static string EnemyPath = @"Assets\Enemies";
    readonly public static string EncounterPath = @"Assets\Encounters\Overworld\Combat";
    readonly public static string ArtifactPath = @"Assets\ItemData\Artifacts";
    readonly public static string PerkPath = @"Assets\ItemData\Perks";
    readonly public static string ModifierPath = @"Assets\CardModifier";
    readonly public static string ScenarioPath = @"Assets\Scenarios";
    readonly public static string DialoguePath = @"Assets\Dialogues";
    readonly public static string ItemPath = @"Assets\ItemData\ItemUsable";
    readonly public static string ItemArtPath = @"Assets\Art\Artwork\Items";
    readonly public static string MissionPath = @"Assets\Progression\Missions";

    SheetsService service;

    public DatabaseGoogle()
    {
        SpreadSheetIDs["Main"] = "17GflJ9aZYsEpgOmopd5H1e92KdqSRa22m0zvHU4gxvc";


        GoogleCredential credential;
        using (FileStream stream = new FileStream("GoogleSheets.json", FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);

            service = new SheetsService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName
            });
        }
    }

    public void DownloadCards()
    {
        ReadEntriesCard("Card", "Z");
        ReadEntriesCardEffects("CardEffects", "Z");
        ReadEntriesCardActivites("CardActivities", "K");
        ReadEntriesCardFields("CardSingleFields", "F");
        ReadEntriesCardCosts("CardCosts", "Z");
        ReadEntriesCardStarting("CardsStarting", "E");
    }

    public void DownloadEnemies()
    {
        ReadEntriesEnemy("Enemy", "Z", "Main");
        ReadEntriesEnemyCards("EnemyDecks", "Z", "Main");
        ReadEntriesEnemyEffects("EnemyEffects", "Z", "Main");
    }

    public void DownloadEncounters()
    {
        ReadEntriesEncounter("CombatEncounter", "Z", "Main");
        ReadEntriesEncounterEnemies("CombatEncounterEnemies", "Z", "Main");
        ReadEntriesEncounterEffects("CombatEncounterEffects", "Z", "Main");
    }

    public void DownloadScenarios()
    {
        ReadEntriesScenarios("Scenario", "Z");
        ReadEntriesScenariosSegments("ScenarioSegments", "Z");
    }

    public void DownloadDialogues()
    {
        ReadEntriesDialogue("Dialogue", "Z");
        ReadEntriesDialogueSentences("DialogueSentences", "Z");
        ReadEntriesDialoguEvents("DialogueEvents", "Z");
    }

    public void DownloadItemUsables()
    {
        ReadEntriesItemUsable("ItemUsable","Z");
    }

    public void DownloadMissions()
    {
        ReadEntriesMissions("Mission", "Z");
        ReadEntriesMissionsEvents("MissionEvents", "Z");
        ReadEntriesMissionsConditions("MissionConditions", "Z");
    }

    #region CardDownload

    public static void CreateCardData()
    {
        CardData asset = ScriptableObject.CreateInstance<CardData>();

        AssetDatabase.CreateAsset(asset, "Assets/NewScripableObject.asset");
        AssetDatabase.SaveAssets();
    }


    public void ReadEntriesCard(string sheetName, string lastCol)
    {
        GameObject GO_DatabaseSystem = GameObject.Find("DatabaseSystem");
        DatabaseSystem dbs = GO_DatabaseSystem.GetComponent<DatabaseSystem>();
        dbs.cards.Clear();
        dbs.cardModifiers.Clear();

        GoogleTable gt = getGoogleTable(sheetName, lastCol);
        for (int i = 1; i < gt.values.Count; i++)
        {
            AssetDatabase.SaveAssets();
            string databaseName = (string)gt[i, "DatabaseName"];
            if (databaseName.Equals(""))
                break;

            CardData data = TDataNameToAsset<CardData>(databaseName, new string[] { CardPath});
            if (data is null)
            {
                data = ScriptableObject.CreateInstance<CardData>();
                AssetDatabase.SaveAssets();
                AssetDatabase.CreateAsset(data, CardPath + @"\" + databaseName + ".asset");
            }

            data.ResetFunctionality();

            Enum.TryParse((string)gt[i, "CardType"], out data.cardType);
            Enum.TryParse((string)gt[i, "Class"], out data.cardClass);
            Enum.TryParse((string)gt[i, "Rarity"], out data.rarity);

            data.id = (string)gt[i, "ID"];
            data.name = (string)gt[i, "DatabaseName"];
            data.cardName = (string)gt[i, "Name"];
            data.goldValue = int.Parse((string)gt[i, "GoldValue"]);

            data.maxUpgrades = int.Parse((string)gt[i, "Upgrades"]);
            data.upgradeCostFullEmber = (string)gt[i, "UpgradeCost"];
            data.upgrades.Clear();

            for (int j = 1; j <= data.maxUpgrades; j++)
            {
                CardFunctionalityData mod = FunctionalityAt(databaseName, j);
                mod.ResetFunctionality();
                data.upgrades.Add(mod);
                mod.id = data.id + "_" + j;
                EditorUtility.SetDirty(mod);
            }

            BindArt(data, databaseName);

            dbs.cards.Add(data);
            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    public void ReadEntriesCardFields(string sheetName, string lastCol)
    {
        GameObject GO_DatabaseSystem = GameObject.Find("DatabaseSystem");
        DatabaseSystem dbs = GO_DatabaseSystem.GetComponent<DatabaseSystem>();
        GoogleTable gt = getGoogleTable(sheetName, lastCol);

        for (int i = 1; i < gt.values.Count; i++)
        {
            string databaseName = (string)gt[i, "DatabaseName"];
            if (databaseName.Equals(""))
                break;


            CardSingleFieldPropertyType type;
            Enum.TryParse((string)gt[i, "Property"], out type);


            string id = (string)gt[i, "CardId"];
            int level = Int32.Parse((string)gt[i, "UpgradeLevel"]);
            CardFunctionalityData data = level == 0 ?
                                            dbs.cards.Where(x=> x.id == id).FirstOrDefault() :
                                            FunctionalityAt(databaseName, level);
            data.singleFieldProperties.Add(new CardSingleFieldPropertyTypeWrapper(type, bool.Parse((string)gt[i, "Add"])));

            if (level != 0 && !dbs.cardModifiers.Contains(data))
                dbs.cardModifiers.Add(data);

            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

    }

    public void ReadEntriesCardCosts(string sheetName, string lastCol)
    {
        GoogleTable gt = getGoogleTable(sheetName, lastCol);
        GameObject GO_DatabaseSystem = GameObject.Find("DatabaseSystem");
        DatabaseSystem dbs = GO_DatabaseSystem.GetComponent<DatabaseSystem>();

        for (int i = 1; i < gt.values.Count; i++)
        {
            string databaseName = (string)gt[i, "DatabaseName"];
            if (databaseName.Equals(""))
                break;

            string id = (string)gt[i, "CardId"];
            int level = Int32.Parse((string)gt[i, "UpgradeLevel"]);
            CardFunctionalityData data = level == 0 ?
                                            dbs.cards.Where(x => x.id == id).FirstOrDefault() :
                                            FunctionalityAt(databaseName, level);

            data.costDatas.Clear();
            data.costOptionalDatas.Clear();
            if (!string.IsNullOrEmpty((string)gt[i, "CostStandard"]))
                data.costDatas.Add(new EnergyData() { type = EnergyType.Standard, data = CardInt.ParseInput((string)gt[i, "CostStandard"]) });
            if (!string.IsNullOrEmpty((string)gt[i, "CostRage"]))
                data.costDatas.Add(new EnergyData() { type = EnergyType.Rage, data = CardInt.ParseInput((string)gt[i, "CostRage"]) });
            if (!string.IsNullOrEmpty((string)gt[i, "CostOptionalStandard"]))
                data.costOptionalDatas.Add(new EnergyData() { type = EnergyType.Standard, data = CardInt.ParseInput((string)gt[i, "CostOptionalStandard"]) });
            if (!string.IsNullOrEmpty((string)gt[i, "CostOptionalRage"]))
                data.costOptionalDatas.Add(new EnergyData() { type = EnergyType.Rage, data = CardInt.ParseInput((string)gt[i, "CostOptionalRage"]) });

            if (level != 0 && !dbs.cardModifiers.Contains(data))
                dbs.cardModifiers.Add(data);

            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

    }

    public void ReadEntriesCardEffects(string sheetName, string lastCol)
    {
        GoogleTable gt = getGoogleTable(sheetName, lastCol);
        GameObject GO_DatabaseSystem = GameObject.Find("DatabaseSystem");
        DatabaseSystem dbs = GO_DatabaseSystem.GetComponent<DatabaseSystem>();

        for (int i = 1; i < gt.values.Count; i++)
        {
            string databaseName = (string)gt[i,"DatabaseName"];
            if (databaseName.Equals(""))
                break;
            

            CardEffectCarrierData cardEffect = new CardEffectCarrierData();
            Enum.TryParse((string)gt[i, "EffectType"], out cardEffect.Type);
            cardEffect.Value = CardInt.ParseInput((string)gt[i, "Value"]);
            cardEffect.Times = CardInt.ParseInput((string)gt[i, "Times"]);
            Enum.TryParse((string)gt[i, "TargetType"], out cardEffect.Target);

            cardEffect.ConditionData = new ConditionData();
            Enum.TryParse((string)gt[i, "ConditionType"], out cardEffect.ConditionData.type);
            cardEffect.ConditionData.strParameter = (string)gt[i, "ConditionStrParam"];
            cardEffect.ConditionData.numValue = (string)gt[i, "ConditionValue"] == "" ? 0 : Int32.Parse((string)gt[i, "ConditionValue"]);

            Enum.TryParse((string)gt[i, "ExecutionTime"], out cardEffect.execTime);

            string id = (string)gt[i, "CardId"];
            int level = Int32.Parse((string)gt[i, "UpgradeLevel"]);
            CardFunctionalityData data = level == 0 ?
                                            dbs.cards.Where(x => x.id == id).FirstOrDefault() :
                                            FunctionalityAt(databaseName, level);

            data.effects.Add(cardEffect);

            if (level != 0 && !dbs.cardModifiers.Contains(data))
                dbs.cardModifiers.Add(data);

            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

    }

    public void ReadEntriesCardActivites(string sheetName, string lastCol)
    {
        GoogleTable gt = getGoogleTable(sheetName, lastCol);
        GameObject GO_DatabaseSystem = GameObject.Find("DatabaseSystem");
        DatabaseSystem dbs = GO_DatabaseSystem.GetComponent<DatabaseSystem>();

        for (int i = 1; i < gt.values.Count; i++)
        {
            string databaseName = (string)gt[i, "DatabaseName"];
            if (databaseName.Equals(""))
                break;

            CardActivityData activity = new CardActivityData();
            Enum.TryParse((string)gt[i, "Activity"], out activity.type);
            Enum.TryParse((string)gt[i, "ExecutionTime"], out activity.execTime);

            activity.strParameter = (string)gt[i, "strParameter"];
            activity.val = Int32.Parse((string)gt[i, "val"]);

            string id = (string)gt[i, "CardId"];
            int level = Int32.Parse((string)gt[i, "UpgradeLevel"]);
            CardFunctionalityData data = level == 0 ?
                                            dbs.cards.Where(x => x.id == id).FirstOrDefault() :
                                            FunctionalityAt(databaseName, level);
            data.activities.Add(activity);

            if (level != 0 && !dbs.cardModifiers.Contains(data))
                dbs.cardModifiers.Add(data);

            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

    }


    public void ReadEntriesCardStarting(string sheetName, string lastCol)
    {
        GameObject GO_DatabaseSystem = GameObject.Find("DatabaseSystem");
        DatabaseSystem dbs = GO_DatabaseSystem.GetComponent<DatabaseSystem>();
        GoogleTable gt = getGoogleTable(sheetName, lastCol);

        dbs.StartingCards.Clear(); 

        for (int i = 1; i < gt.values.Count; i++)
        {
            AssetDatabase.SaveAssets();
            string databaseName = (string)gt[i, "DatabaseName"];
            if (databaseName.Equals(""))
                break;

            CharacterClassType classType;
            Enum.TryParse((string)gt[i, "CharacterClass"], out classType);
            ProfessionType profession;
            Enum.TryParse((string)gt[i, "Profession"], out profession);

            StartingCardSet scs = dbs.StartingCards.Where(x => x.characterClass == classType && x.profession == profession).FirstOrDefault();
            if (scs is null)
            {
                scs = new StartingCardSet() { profession = profession, characterClass = classType, name = classType.ToString() + ", " + profession.ToString() };
                dbs.StartingCards.Add(scs);
            }
            scs.startingCards.AddRange(dbs.GetCardsByName(new List<string>() { databaseName }));

            EditorUtility.SetDirty(dbs);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

    }

    public void BindArt(CardData cardData, string name)
    {
        string folder = @"Assets/Art/Artwork/Cards/";
        Sprite artWork = (Sprite)AssetDatabase.LoadAssetAtPath<Sprite>(folder + name + ".png");

        if (artWork is null)
        {
            Debug.Log("GoogleImport: Art not found for: " + name);
            AssetDatabase.CopyAsset(folder + "Placeholder.png", folder + name + ".png");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            artWork = (Sprite)AssetDatabase.LoadAssetAtPath<Sprite>(folder + name + ".png");
        }

        cardData.artwork = artWork;
    }

    public void BindArt(ItemData data)
    {
        string folder;
        if(data is ArtifactData aData)
        {
            folder = @"Assets/Art/Artwork/Artifacts/";
            Sprite artWork = AssetDatabase.LoadAssetAtPath<Sprite>(folder + data.itemId.ToString() + ".png");
            if(artWork == null)
            {
                Debug.LogWarning("No such artwork for Artifact " + data.itemId.ToString());
                return;
            }
            data.artwork = artWork;
        }
        else
        {
            folder = @"Assets/Art/Artwork/Perks/";
            Sprite artWorkActive = AssetDatabase.LoadAssetAtPath<Sprite>(folder + data.itemId.ToString() + "Active.png");
            Sprite artWorkInactive = AssetDatabase.LoadAssetAtPath<Sprite>(folder + data.itemId.ToString() + "Inactive.png");
            if (artWorkActive == null || artWorkInactive== null)
            {
                Debug.LogWarning("No such artwork for Perk " + data.itemId.ToString());
                return;
            }

            data.artwork = artWorkActive;
            ((PerkData)data).inactiveArtwork = artWorkInactive;
        }
    }

    public void BindCharacterModel(EnemyData enemyData)
    {
        string folder = @"Assets/Art/Artwork/Enemies/";
        string name = enemyData.name;
        GameObject artWork = AssetDatabase.LoadAssetAtPath<GameObject>(folder + name + @"/" + name + "_Complete.prefab");

        if (artWork is null)
        {
            Debug.Log("GoogleImport: CharacterArt not found for: " + name);
            artWork = AssetDatabase.LoadAssetAtPath<GameObject>(folder + @"Placeholder/Placeholder_Complete.prefab");
        }

        enemyData.characterArt = artWork;

    }


    public void PrintCardData()
    {
        string[] strCardIDs = AssetDatabase.FindAssets("t:CardData", new string[] {CardPath });
        List<IList<object>> InputDataCard = new List<IList<object>>();
        List<IList<object>> InputDataCardEffects = new List<IList<object>>();
        List<IList<object>> InputDataCardActivities = new List<IList<object>>();

        foreach (string ID in strCardIDs)
        {
            List<object> cDataCard = new List<object>();

            string lAssetPathCard = AssetDatabase.GUIDToAssetPath(ID);
            CardData cardData = (CardData)AssetDatabase.LoadAssetAtPath<CardData>(lAssetPathCard);

            cDataCard.Add(cardData.cardClass.ToString());
            cDataCard.Add(cardData.name);
            cDataCard.Add(cardData.cardName);
            cDataCard.Add(cardData.costDatas);

            InputDataCard.Add(cDataCard);

            if (cardData.effects.Count != 0)
            {
                foreach (CardEffectCarrierData effect in cardData.effects)
                {
                    List<object> cDataCardEffect = new List<object>();
                    cDataCardEffect.Add(cardData.name);
                    cDataCardEffect.Add(cardData.cardName);
                    cDataCardEffect.Add(effect.Type.ToString());
                    cDataCardEffect.Add(effect.Times);
                    cDataCardEffect.Add(effect.Value);
                    cDataCardEffect.Add(effect.Target.ToString());
                    InputDataCardEffects.Add(cDataCardEffect);
                }
            }

            if (cardData.activities.Count != 0)
            {
                foreach (CardActivityData caSetting in cardData.activities)
                {
                    List<object> cDataCardActivity = new List<object>();
                    cDataCardActivity.Add(cardData.name);
                    cDataCardActivity.Add(cardData.cardName);
                    cDataCardActivity.Add(caSetting.type.ToString());
                    cDataCardActivity.Add(caSetting.strParameter.ToString());
                    InputDataCardActivities.Add(cDataCardActivity);
                }
            }
        }

        GoogleUpdateData(InputDataCard, "Upl_Card", "A2:Z" + (InputDataCard.Count + 100).ToString());
        GoogleUpdateData(InputDataCardEffects, "Upl_CardEffects", "A2:Z" + (InputDataCardEffects.Count + 100).ToString());
        GoogleUpdateData(InputDataCardActivities, "Upl_CardActivities", "A2:Z" + (InputDataCardEffects.Count + 100).ToString());
    }

    #endregion

    #region EnemyDownload

    public void ReadEntriesEnemy(string sheetName, string lastCol, string sheet)
    {
        GoogleTable gt = getGoogleTable(sheetName, lastCol, sheet);
        GameObject GO_DatabaseSystem = GameObject.Find("DatabaseSystem");
        DatabaseSystem dbs = GO_DatabaseSystem.GetComponent<DatabaseSystem>();
        dbs.enemies.Clear();

        for (int i = 1; i < gt.values.Count; i++)
        {
            string databaseName = (string)gt[i, "DatabaseName"];
            if (databaseName.Equals(""))
                break;

            EnemyData data = TDataNameToAsset<EnemyData>(databaseName, new string[]{EnemyPath});
            if(data is null)
            {
                data = ScriptableObject.CreateInstance<EnemyData>();
                AssetDatabase.CreateAsset(data, EnemyPath + @"\" + databaseName + ".asset");
            }


            data.enemyName      = (string)gt[i, "Name"];
            data.enemyId        = int.Parse((string)gt[i, "EnemyID"]);
            data.StartingHP     = int.Parse((string)gt[i, "HP"]);
            data.tier           = int.Parse((string)gt[i, "Tier"]);
            data.experience     = int.Parse((string)gt[i, "Experience"]);

            data.shuffleInit    = bool.Parse((string)gt[i, "InitialShuffle"]);
            data.stochasticReshuffle = bool.Parse((string)gt[i, "StochasticReshuffle"]);
            
            data.deck.Clear();
            data.startingEffects.Clear();

            BindCharacterModel(data);
            dbs.enemies.Add(data);
            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    public void ReadEntriesEnemyCards(string sheetName, string lastCol, string sheet)
    {
        GoogleTable gt = getGoogleTable(sheetName, lastCol, sheet);
        GameObject GO_DatabaseSystem = GameObject.Find("DatabaseSystem");
        DatabaseSystem dbs = GO_DatabaseSystem.GetComponent<DatabaseSystem>();

        for (int i = 1; i < gt.values.Count; i++)
        {
            string databaseName = (string)gt[i, "DatabaseName"];
            if (databaseName.Equals(""))
                break;


            EnemyData enemy = dbs.enemies.Where(x => x.name == databaseName).FirstOrDefault();
            if (enemy == null)
            {
                Debug.Log("No such enemy named" + databaseName);
                continue;
            }

            string id = (string)gt[i, "CardId"];
            CardData card = dbs.cards.Where(x => x.id == id).FirstOrDefault();
            if (card == null)
            {
                Debug.LogError("SKIPPED: Enemy assigned non-existant cardId: " + id);
                continue;
            }


            enemy.deck.Add(card);
            
            EditorUtility.SetDirty(enemy);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

    }


    public void ReadEntriesEnemyEffects(string sheetName, string lastCol, string sheet)
    {
        /*
        GoogleTable gt = getGoogleTable(sheetName, lastCol, sheet);

        for (int i = 1; i < gt.values.Count; i++)
        {
            string databaseName = (string)gt[i, "DatabaseName"];
            if (databaseName.Equals(""))
                break;

            EnemyData data = TDataNameToAsset<EnemyData>(databaseName, new string[] { EnemyPath });
            if (data == null)
            {
                Debug.LogError("SKIPPED: Effect assigned to non-existant enemy named: " + databaseName);
                continue;
            }

            CardEffectCarrier cardEffect = new CardEffectCarrier();
            cardEffect.Target = CardTargetType.Self;
            Enum.TryParse((string)gt[i, "EffectType"], out cardEffect.Type);
            cardEffect.Value = int.Parse((string)gt[i, "Value"]);
            cardEffect.Times = int.Parse((string)gt[i, "Times"]);

            data.startingEffects.Add(cardEffect);

            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        */
    }

    #endregion

    #region Encounters
    public void ReadEntriesEncounter(string sheetName, string lastCol, string sheet)
    {
        GoogleTable gt = getGoogleTable(sheetName, lastCol, sheet);
        GameObject GO_DatabaseSystem = GameObject.Find("DatabaseSystem");
        DatabaseSystem dbs = GO_DatabaseSystem.GetComponent<DatabaseSystem>();

        dbs.encountersCombat.Clear();


        for (int i = 1; i < gt.values.Count; i++)
        {
            string databaseName = (string)gt[i, "DatabaseName"];
            if (databaseName.Equals(""))
                break;

            EncounterDataCombat data = TDataNameToAsset<EncounterDataCombat>(databaseName, new string[] { EncounterPath});
            if (data is null)
            {
                data = ScriptableObject.CreateInstance<EncounterDataCombat>();
                AssetDatabase.CreateAsset(data, EncounterPath + @"\" + databaseName + ".asset");
            }


            data.name = (string)gt[i, "Name"];
            Enum.TryParse((string)gt[i, "Type"], out data.type);
            Enum.TryParse((string)gt[i, "Formation"], out data.formation);
            data.tier = int.Parse((string)gt[i, "Tier"]);

            data.enemyData.Clear();
            data.startEffectsTargets.Clear();
            data.startingEffects.Clear();


            dbs.encountersCombat.Add(data);
            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    public void ReadEntriesEncounterEnemies(string sheetName, string lastCol, string sheet)
    {
        GoogleTable gt = getGoogleTable(sheetName, lastCol, sheet);



        for (int i = 1; i < gt.values.Count; i++)
        {
            string databaseName = (string)gt[i, "DatabaseName"];
            if (databaseName.Equals(""))
                break;

            EncounterDataCombat data = TDataNameToAsset<EncounterDataCombat>(databaseName, new string[] { EncounterPath });
            if (data == null)
            {
                Debug.LogError("SKIPPED: Enemy assigned to non-existant encounter : " + databaseName);
                continue;
            }

            string EnemyName = (string)gt[i, "EnemyDatabaseName"];

            EnemyData enemyData  = TDataNameToAsset<EnemyData>(EnemyName, new string[] { EnemyPath });
            if (enemyData == null)
            {
                Debug.LogError("SKIPPED: Encounter assigned non-existant enemy: " + EnemyName);
                continue;
            }

            data.enemyData.Add(enemyData);

            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

    }

    private void ReadEntriesEncounterEffects(string sheetName, string lastCol, string sheet)
    {
        GoogleTable gt = getGoogleTable(sheetName, lastCol, sheet);

        for (int i = 1; i < gt.values.Count; i++)
        {
            string databaseName = (string)gt[i, "DatabaseName"];
            if (databaseName.Equals(""))
                break;

            EncounterDataCombat data = TDataNameToAsset<EncounterDataCombat>(databaseName, new string[] { EncounterPath });
            if (data == null)
            {
                Debug.LogError("SKIPPED: Effects assigned to non-existant encounter : " + databaseName);
                continue;
            }

            CardEffectCarrierData cardEffect = new CardEffectCarrierData();
            Enum.TryParse((string)gt[i, "EffectType"], out cardEffect.Type);
            cardEffect.Value = CardInt.ParseInput((string)gt[i, "Value"]);
            cardEffect.Times = CardInt.ParseInput((string)gt[i, "Times"]);
            Enum.TryParse((string)gt[i, "TargetType"], out cardEffect.Target);

            data.startingEffects.Add(cardEffect);
            data.startEffectsTargets.Add(int.Parse((string)gt[i, "EnemyNr"]));

            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    #endregion

    #region Dialogue
    public void ReadEntriesDialogue(string sheetName, string lastCol)
    {
        GameObject GO_DatabaseSystem = GameObject.Find("DatabaseSystem");
        DatabaseSystem dbs = GO_DatabaseSystem.GetComponent<DatabaseSystem>();
        dbs.dialogues.Clear();

        GoogleTable gt = getGoogleTable(sheetName, lastCol);
        for (int i = 1; i < gt.values.Count; i++)
        {
            AssetDatabase.SaveAssets();
            string databaseName = (string)gt[i, "Name"];
            if (databaseName.Equals(""))
                break;

            DialogueData data = TDataNameToAsset<DialogueData>(databaseName, new string[] { DialoguePath });
            if (data is null)
            {
                data = ScriptableObject.CreateInstance<DialogueData>();
                AssetDatabase.SaveAssets();
                AssetDatabase.CreateAsset(data, DialoguePath + @"\" + databaseName + ".asset");
            }

            data.index = int.Parse((string)gt[i, "Id"]);
            data.stateToTriggerIn = ((string)gt[i, "StateToTriggerIn"]).ToEnum<WorldState>();

            data.sentences.Clear();
            data.endEvent.Clear();
            

            dbs.dialogues.Add(data);
            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    public void ReadEntriesDialogueSentences(string sheetName, string lastCol)
    {
        GoogleTable gt = getGoogleTable(sheetName, lastCol);

        GameObject GO_DatabaseSystem = GameObject.Find("DatabaseSystem");
        DatabaseSystem dbs = GO_DatabaseSystem.GetComponent<DatabaseSystem>();

        for (int i = 1; i < gt.values.Count; i++)
        {
            int id = int.Parse((string)gt[i, "DialogueId"]);
            DialogueData data = dbs.dialogues.Where(x => x.index == id).FirstOrDefault();
            if (data  == null)
            {
                Debug.Log("No Such DialogueID:" + id);
                break;
            }

            Sentence sentence = new Sentence();
            Enum.TryParse((string)gt[i, "Participant"], out sentence.dialogueParticipant);
            sentence.sentence = (string)gt[i, "Sentence"];
            data.sentences.Add(sentence);

            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

    }

    public void ReadEntriesDialoguEvents(string sheetName, string lastCol)
    {
        GoogleTable gt = getGoogleTable(sheetName, lastCol);

        GameObject GO_DatabaseSystem = GameObject.Find("DatabaseSystem");
        DatabaseSystem dbs = GO_DatabaseSystem.GetComponent<DatabaseSystem>();

        for (int i = 1; i < gt.values.Count; i++)
        {
            int id = int.Parse((string)gt[i, "DialogueId"]);
            DialogueData data = dbs.dialogues.Where(x => x.index == id).FirstOrDefault();
            if (data == null)
            {
                Debug.Log("No Such DialogueID:" + id);
                break;
            }

            GameEventStruct eventInfo = new GameEventStruct();
            Enum.TryParse((string)gt[i, "Type"], out eventInfo.type);
            eventInfo.parameter = (string)gt[i, "parameter"];
            eventInfo.value = (string)gt[i, "value"];

            data.endEvent.Add(eventInfo);

            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

    }

    #endregion

    public void ReadEntriesItemUsable(string sheetName, string lastCol)
    {
        GoogleTable gt = getGoogleTable(sheetName, lastCol);
        GameObject GO_DatabaseSystem = GameObject.Find("DatabaseSystem");
        DatabaseSystem dbs = GO_DatabaseSystem.GetComponent<DatabaseSystem>();

        dbs.abilityDatas.Clear();

        for (int i = 1; i < gt.values.Count; i++)
        {
            AssetDatabase.SaveAssets();
            int id = ((string)gt[i, "Id"]).ToInt();

            AbilityData data = TDataNameToAsset<AbilityData>("Ability" + id, new string[] { ItemPath });
            if (data is null)
            {
                data = ScriptableObject.CreateInstance<AbilityData>();
                AssetDatabase.SaveAssets();
                AssetDatabase.CreateAsset(data, ItemPath + @"\" + "Ability" + id.ToString() + ".asset");
            }

            data.itemId = id;
            data.itemName = (string)gt[i, "ItemName"];
            data.description = (string)gt[i, "Description"];
            data.rarity = ((string)gt[i, "Rarity"]).ToEnum<Rarity>();
            data.itemEffectStruct = new ItemEffectStruct();
            data.itemEffectStruct.type = ((string)gt[i, "EffectType"]).ToEnum<ItemEffectType>();
            data.itemEffectStruct.parameter= (string)gt[i, "EffectParameter"];
            data.itemEffectStruct.value= ((string)gt[i, "EffectValue"]).ToInt();
            
            data.itemCondition = new ConditionData();
            data.itemCondition.type = ((string)gt[i, "ConditionType"]).ToEnum<ConditionType>();
            data.itemCondition.strParameter = (string)gt[i, "ConditionStrParam"];
            data.itemCondition.numValue = ((string)gt[i, "ConditionValue"]).ToInt();

            data.statesUsable.Clear();
            string[] worldStates = ((string)gt[i, "StatesUsable"]).Split(';');
            if (!String.IsNullOrEmpty(worldStates[0]))
                for (int s = 0; s < worldStates.Length; s++)
                    data.statesUsable.Add(worldStates[s].ToEnum<WorldState>());

            Sprite sprite = TDataNameToAsset<Sprite>("Item" + id.ToString() + "Art", new string[] { ItemArtPath });
            if (sprite != null)
                data.artwork = sprite;
            else
                Debug.Log("Art: " + "Ability" + id.ToString() + "Art" + " not found");

            dbs.abilityDatas.Add(data);
            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

    }

    public void ReadEntriesScenarios(string sheetName, string lastCol)
    {
        GameObject GO_DatabaseSystem = GameObject.Find("DatabaseSystem");
        DatabaseSystem dbs = GO_DatabaseSystem.GetComponent<DatabaseSystem>();
        dbs.scenarios.Clear();

        GoogleTable gt = getGoogleTable(sheetName, lastCol);
        for (int i = 1; i < gt.values.Count; i++)
        {
            AssetDatabase.SaveAssets();
            string ScenarioName = (string)gt[i, "ScenarioName"];
            if (ScenarioName.Equals(""))
                break;

            ScenarioData data = TDataNameToAsset<ScenarioData>(ScenarioName, new string[] { ScenarioPath });
            if (data is null)
            {
                data = ScriptableObject.CreateInstance<ScenarioData>();
                AssetDatabase.SaveAssets();
                AssetDatabase.CreateAsset(data, ScenarioPath + @"\" + ScenarioName + ".asset");
            }

            data.id = int.Parse((string)gt[i, "ID"]);
            data.ScenarioName = ScenarioName;
            Enum.TryParse((string)gt[i, "Difficulty"], out data.difficulty);
            Enum.TryParse((string)gt[i, "RewardType"], out data.rewardStruct.type);
            Enum.TryParse((string)gt[i, "ScenarioType"], out data.type);
            
            data.rewardStruct.value = ((string)gt[i, "RewardValues"]);
            data.unlocksScenarios.Clear(); 
            string[] unlockableIDs = ((string)gt[i, "UnlockableScenarioIDs"]).Split(';');
            for(int s = 0; s < unlockableIDs.Length; s++)
                if (!string.IsNullOrEmpty(unlockableIDs[s])) data.unlocksScenarios.Add(Int32.Parse(unlockableIDs[s]));

            data.Description = (string)gt[i, "Description"];
            data.DescriptionShort = (string)gt[i, "DescriptionShort"];
            data.linkedScenarioId = ((string)gt[i, "ScenarioType"]).ToInt(-1);

            data.SegmentDatas.Clear();
            dbs.scenarios.Add(data);

            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    public void ReadEntriesScenariosSegments(string sheetName, string lastCol)
    {
        GameObject GO_DatabaseSystem = GameObject.Find("DatabaseSystem");
        DatabaseSystem dbs = GO_DatabaseSystem.GetComponent<DatabaseSystem>();

        GoogleTable gt = getGoogleTable(sheetName, lastCol);

        for (int i = 1; i < gt.values.Count; i++)
        {
            int scenarioID  = int.Parse((string)gt[i, "ScenarioID"]);
            string segmentName = (string)gt[i, "SegmentName"];
            ScenarioData scenarioData = dbs.scenarios.Where(x => x.id == scenarioID).FirstOrDefault();
            if (scenarioData == null || segmentName.Equals(string.Empty))
            {
                if(scenarioData == null) Debug.Log("No Such ScenarioID:" + scenarioID);
                break;
            }

            ScenarioSegmentData data = new ScenarioSegmentData();

            Enum.TryParse((string)gt[i, "Type"], out data.segmentType);

            string[] colorVec = ((string)gt[i, "Color"]).Replace("(","").Replace(")","").Split(',');
            data.color = new UnityEngine.Color(
                float.Parse(colorVec[0], System.Globalization.CultureInfo.InvariantCulture), 
                float.Parse(colorVec[1], System.Globalization.CultureInfo.InvariantCulture), 
                float.Parse(colorVec[2], System.Globalization.CultureInfo.InvariantCulture));

            ((string)gt[i, "GridCoordinates"]).Split(';').ToList().ForEach(x => data.gridCoordinates.Add(Vector3IntFromString(x)));

            string[] encounters = ((string)gt[i, "Encounters"]).Split(';');
            foreach(string s in encounters)
            {
                if (s.Equals(string.Empty)) break;
                EncounterData encData = dbs.encountersCombat.Where(x => x.name == s).FirstOrDefault();
                if(encData == null) encData = dbs.encounterEvent.Where(x => x.name == s).FirstOrDefault();
                if (encData != null) data.encounters.Add(encData);
            }

            string[] encountersMiss = ((string)gt[i, "EncountersMiss"]).Split(';');
            foreach (string s in encountersMiss)
            {
                if (s.Equals(string.Empty)) break;
                EncounterData encData = dbs.encountersCombat.Where(x => x.name == s).FirstOrDefault();
                if (encData == null) encData = dbs.encounterEvent.Where(x => x.name == s).FirstOrDefault();
                if (encData != null) data.missEncounters.Add(encData);
            }

            data.nrSkippableTiles = ((string)gt[i, "nrSkippableTiles"]).Equals(string.Empty) ? 0 : int.Parse((string)gt[i, "nrSkippableTiles"]);
            data.nrDecoys = ((string)gt[i, "nrDecoys"]).Equals(string.Empty) ? 0 :  int.Parse((string)gt[i, "nrDecoys"]);

            data.SegmentName = segmentName;
            data.description = (string)gt[i, "Description"];

            ((string)gt[i, "RequiredSegmentsOR"]).Split(';').ToList().ForEach(x => { if (!x.Equals(string.Empty)) data.requiredSegmentsOR.Add(x); });
            ((string)gt[i, "RequiredSegmentsAND"]).Split(';').ToList().ForEach(x => { if (!x.Equals(string.Empty)) data.requiredSegmentsAND.Add(x); });
            ((string)gt[i, "CancelSegmentsOnStart"]).Split(';').ToList().ForEach(x => { if (!x.Equals(string.Empty)) data.cancelSegmentsOnStart.Add(x); });


            scenarioData.SegmentDatas.Add(data);

            EditorUtility.SetDirty(scenarioData);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    public void ReadEntriesMissions(string sheetName, string lastCol)
    {
        GameObject GO_DatabaseSystem = GameObject.Find("DatabaseSystem");
        DatabaseSystem dbs = GO_DatabaseSystem.GetComponent<DatabaseSystem>();
        dbs.missions.Clear();

        GoogleTable gt = getGoogleTable(sheetName, lastCol);
        for (int i = 1; i < gt.values.Count; i++)
        {
            AssetDatabase.SaveAssets();
            string name = (string)gt[i, "Name"];
            if (name.Equals(""))
                break;

            int id = int.Parse((string)gt[i, "Id"]);
            MissionData data = TDataNameToAsset<MissionData>("Mission" + id.ToString(), new string[] { MissionPath });
            if (data is null)
            {
                data = ScriptableObject.CreateInstance<MissionData>();
                AssetDatabase.SaveAssets();
                AssetDatabase.CreateAsset(data, MissionPath + @"\" + "Mission" + id.ToString() + ".asset");
            }

            data.id = id;
            data.aName = (string)gt[i, "Name"];
            data.mainMission = (string)gt[i, "Name"] == "TRUE";

            data.addDialogueIdx.Clear();

            string[] unlockableIDs = ((string)gt[i, "DialogueIds"]).Split(';');
            for (int s = 0; s < unlockableIDs.Length; s++)
                if (!string.IsNullOrEmpty(unlockableIDs[s])) data.addDialogueIdx.Add(unlockableIDs[s].ToInt());


            data.description = (string)gt[i, "Description"];

            if (string.IsNullOrEmpty((string)gt[i, "NextMissionId"]))
                data.nextMissionId = -1;
            else
                data.nextMissionId = ((string)gt[i, "NextMissionId"]).ToInt();

            data.gameEventsOnEnd.Clear();
            data.gameEventsOnEnd.Clear();
            data.conditionStructs.Clear();
            
            dbs.missions.Add(data);

            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }


    public void ReadEntriesMissionsEvents(string sheetName, string lastCol)
    {
        GameObject GO_DatabaseSystem = GameObject.Find("DatabaseSystem");
        DatabaseSystem dbs = GO_DatabaseSystem.GetComponent<DatabaseSystem>();

        GoogleTable gt = getGoogleTable(sheetName, lastCol);

        for (int i = 1; i < gt.values.Count; i++)
        {
            int id = int.Parse((string)gt[i, "MissionId"]);
            MissionData missionData = dbs.missions.Where(x => x.id == id).FirstOrDefault();
            if (missionData == null)
            {
                if (missionData == null) Debug.Log("No Such MissionId:" + id);
                break;
            }

            GameEventStruct data = new GameEventStruct();

            data.type = ((string)gt[i, "EventType"]).ToEnum<GameEventType>();
            data.parameter = ((string)gt[i, "EventParameter"]);
            data.value = ((string)gt[i, "EventValue"]);

            if ((string)gt[i, "OnStart"] == "TRUE")
                missionData.gameEventsOnStart.Add(data);
            else
                missionData.gameEventsOnEnd.Add(data);

            EditorUtility.SetDirty(missionData);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    public void ReadEntriesMissionsConditions(string sheetName, string lastCol)
    {
        GameObject GO_DatabaseSystem = GameObject.Find("DatabaseSystem");
        DatabaseSystem dbs = GO_DatabaseSystem.GetComponent<DatabaseSystem>();

        GoogleTable gt = getGoogleTable(sheetName, lastCol);

        for (int i = 1; i < gt.values.Count; i++)
        {
            int id = int.Parse((string)gt[i, "MissionId"]);
            MissionData missionData = dbs.missions.Where(x => x.id == id).FirstOrDefault();
            if (missionData == null)
            {
                if (missionData == null) Debug.Log("No Such MissionId:" + id);
                break;
            }

            ConditionData data = new ConditionData();

            data.type = ((string)gt[i, "ConditionType"]).ToEnum<ConditionType>();
            data.strParameter = ((string)gt[i, "StrParameter"]);
            data.numValue = ((string)gt[i, "NumValue"]).ToInt();

            missionData.conditionStructs.Add(data);

            EditorUtility.SetDirty(missionData);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }


    public void ReadEntriesArtifacts(string sheetName, string lastCol)
    {
        GoogleTable gt = getGoogleTable(sheetName, lastCol);
        GameObject GO_DatabaseSystem = GameObject.Find("DatabaseSystem");
        DatabaseSystem dbs = GO_DatabaseSystem.GetComponent<DatabaseSystem>();
        dbs.arifactDatas.Clear();

        for (int i = 1; i < gt.values.Count; i++)
        {
            AssetDatabase.SaveAssets();
            string databaseName = (string)gt[i, "DatabaseName"];
            if (databaseName.Equals(""))
                break;

            int id = ((string)gt[i, "Id"]).ToInt();
            ArtifactData data = dbs.arifactDatas.Where(a => a.itemId == id).FirstOrDefault();
            if (data is null)
            {
                data = ScriptableObject.CreateInstance<ArtifactData>();
                AssetDatabase.SaveAssets();
                AssetDatabase.CreateAsset(data, ArtifactPath + @"\" + databaseName + ".asset");
            }

            data.itemId = id;
            data.name = (string)gt[i, "DatabaseName"];
            data.itemName = (string)gt[i, "ItemName"];
            data.rarity = ((string)gt[i, "Rarity"]).ToEnum<Rarity>();
            data.description = (string)gt[i, "Description"];
            data.itemEffectStruct.type = ((string)gt[i, "EffectType"]).ToEnum<ItemEffectType>();
            data.itemEffectStruct.parameter = (string)gt[i, "EffectParameter"];
            data.itemEffectStruct.value = ((string)gt[i, "EffectValue"]).ToInt();
            data.type = ((string)gt[i, "Type"]).ToEnum<ArtifactType>();

            data.condition = new ConditionData();
            data.condition.type = ((string)gt[i, "ConditionType"]).ToEnum<ConditionType>();
            data.condition.strParameter = ((string)gt[i, "ConditionStrParam"]);
            data.condition.numValue = ((string)gt[i, "ConditionValue"]).ToInt();
            data.conditionResetEvent = ((string)gt[i, "ConditionReset"]).ToEnum<ConditionType>();
            data.conditionCountingOnTrueType = ((string)gt[i, "OnConditionTrueType"]).ToEnum<ConditionCountingOnTrueType>();


            data.goldValue = ((string)gt[i, "GoldValue"]).ToInt();
            data.characterClassType = ((string)gt[i, "CharacterClassType"]).ToEnum<CharacterClassType>();
            BindArt(data);
            //BindArt(data, databaseName);
            dbs.arifactDatas.Add(data);
            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

    }

    public void ReadEntriesPerks(string sheetName, string lastCol)
    {
        GoogleTable gt = getGoogleTable(sheetName, lastCol);
        GameObject GO_DatabaseSystem = GameObject.Find("DatabaseSystem");
        DatabaseSystem dbs = GO_DatabaseSystem.GetComponent<DatabaseSystem>();
        dbs.perkDatas.Clear();

        for (int i = 1; i < gt.values.Count; i++)
        {
            AssetDatabase.SaveAssets();
            string databaseName = (string)gt[i, "DatabaseName"];
            if (databaseName.Equals(""))
                break;

            int id = ((string)gt[i, "Id"]).ToInt();
            PerkData data = dbs.perkDatas.Where(a => a.itemId == id).FirstOrDefault();
            if (data is null)
            {
                data = ScriptableObject.CreateInstance<PerkData>();
                AssetDatabase.SaveAssets();
                AssetDatabase.CreateAsset(data, PerkPath + @"\" + databaseName + ".asset");
            }

            data.itemId = id;
            data.name = (string)gt[i, "DatabaseName"];
            data.itemName = (string)gt[i, "ItemName"];
            data.rarity = ((string)gt[i, "Rarity"]).ToEnum<Rarity>();
            data.description = (string)gt[i, "Description"];
            data.itemEffectStruct.type = ((string)gt[i, "EffectType"]).ToEnum<ItemEffectType>();
            data.itemEffectStruct.parameter = (string)gt[i, "EffectParameter"];
            data.itemEffectStruct.value = ((string)gt[i, "EffectValue"]).ToInt();
            data.itemEffectStruct.addImmediately = (string)gt[i, "EffectAddOnStart"] == "TRUE";
            data.level = ((string)gt[i, "Level"]).ToInt();
            data.characterClassType = ((string)gt[i, "CharacterClassType"]).ToEnum<CharacterClassType>();
            BindArt(data);
            //BindArt(data, databaseName);
            dbs.perkDatas.Add(data);
            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

    }

    GoogleTable getGoogleTable(string sheetName, string lastCol, string sheet = "Main")
    {
        GoogleTable googleTable = new GoogleTable();
        Dictionary<string, int> colNames = new Dictionary<string, int>();

        string range = sheetName + "!A1:" + lastCol + "10000";
        var request = service.Spreadsheets.Values.Get(SpreadSheetIDs[sheet], range);
        var response = request.Execute();
        var values = response.Values;

        for (int i = 0; i < values[0].Count; i++)
        {
            if ((string)values[0][i] != "")
            {
                colNames[(string)values[0][i]] = i;
            }
            else
                break;
        }

        googleTable.ColNames = colNames;
        googleTable.values = values;

        return googleTable;
    }


    void GoogleUpdateData(List<IList<object>> data, string sheetName, string RangeSpace)
    {
        string range = sheetName + "!" + RangeSpace;
        string valueInputOption = "USER_ENTERED";

        // The new values to apply to the spreadsheet.
        List<ValueRange> updateData = new List<ValueRange>();
        var dataValueRange = new ValueRange();
        dataValueRange.Range = range;
        dataValueRange.Values = data; 
        updateData.Add(dataValueRange);

        BatchUpdateValuesRequest requestBody = new BatchUpdateValuesRequest
        {
            ValueInputOption = valueInputOption,
            Data = updateData
        };

        BatchClearValuesRequest clearingRequestBody = new BatchClearValuesRequest();
        clearingRequestBody.Ranges =  new string[]{ range };

        var clearRequest = service.Spreadsheets.Values.BatchClear(clearingRequestBody, SpreadSheetIDs["Main"]);
        var request = service.Spreadsheets.Values.BatchUpdate(requestBody, SpreadSheetIDs["Main"]);

        clearRequest.Execute();
        request.Execute();
        // Data.BatchUpdateValuesResponse response = await request.ExecuteAsync(); // For async 

    }

    public T TDataNameToAsset<T>(string name, string[] foldersToLookIn)
    {
        T data;
        string[] result = AssetDatabase.FindAssets(name + " t:" + typeof(T).Name, foldersToLookIn);
  
        if (result.Length > 1)
            Debug.LogError("More than 1 Asset found for name:" + name);

        if (result.Length == 0)
            return default(T);
        else
        {
            string path = AssetDatabase.GUIDToAssetPath(result[0]);
            object obj = AssetDatabase.LoadAssetAtPath(path,typeof(T));
            data = (T)obj;
        }

        return data;
    }

    public CardFunctionalityData FunctionalityAt(string cardName, int upgradeLevel)
    {
        string databaseName = cardName + "_" + upgradeLevel.ToString();
        CardFunctionalityData data = TDataNameToAsset<CardFunctionalityData>(databaseName, new string[] { ModifierPath });
        if (data is null)
        {
            data = ScriptableObject.CreateInstance<CardFunctionalityData>();
            AssetDatabase.SaveAssets();
            AssetDatabase.CreateAsset(data, ModifierPath + @"\" + databaseName + ".asset");
        }

        return data;
    }

    public Vector3Int Vector3IntFromString(string s)
    {
        List<int> vals = new List<int>();
        string cleaned = s.Replace("(", "").Replace(")", "");
        cleaned.Split(',').ToList().ForEach(x => vals.Add(int.Parse(x)));

        return new Vector3Int(vals[0], vals[1],vals[2]);
    }
}

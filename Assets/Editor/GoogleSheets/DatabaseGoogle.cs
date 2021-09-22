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

    readonly string CardPath = @"Assets\Cards";
    readonly string EnemyPath = @"Assets\Enemies";
    readonly string EncounterPath = @"Assets\Encounters\Overworld\Combat";
    readonly string ArtifactPath = @"Assets\Artifacts";
    readonly string ModifierPath = @"Assets\CardModifier";

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


    #region CardDownload

    public static void CreateCardData()
    {
        CardData asset = ScriptableObject.CreateInstance<CardData>();

        AssetDatabase.CreateAsset(asset, "Assets/NewScripableObject.asset");
        AssetDatabase.SaveAssets();
    }


    public void ReadEntriesCard(string sheetName, string lastCol)
    {
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
            data.cost = CardInt.ParseInput((string)gt[i, "Cost"]);

            data.visibleCost = (string)gt[i, "VisibleCost"] == "TRUE";
            data.goldValue = Int32.Parse((string)gt[i, "GoldValue"]);

            data.maxUpgrades = Int32.Parse((string)gt[i, "Upgrades"]);
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

            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }


    public void ReadEntriesCardFields(string sheetName, string lastCol)
    {
        GoogleTable gt = getGoogleTable(sheetName, lastCol);

        for (int i = 1; i < gt.values.Count; i++)
        {
            string databaseName = (string)gt[i, "DatabaseName"];
            if (databaseName.Equals(""))
                break;


            CardSingleFieldPropertyType type;
            Enum.TryParse((string)gt[i, "Property"], out type);


            int level = Int32.Parse((string)gt[i, "UpgradeLevel"]);
            CardFunctionalityData data = level == 0 ?
                                            TDataNameToAsset<CardFunctionalityData>(databaseName, new string[] { CardPath }) :
                                            FunctionalityAt(databaseName, level);
            data.singleFieldProperties.Add(new CardSingleFieldPropertyTypeWrapper(type, bool.Parse((string)gt[i, "Add"])));

            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

    }

    public void ReadEntriesCardEffects(string sheetName, string lastCol)
    {
        GoogleTable gt = getGoogleTable(sheetName, lastCol);  

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

            cardEffect.conditionStruct = new ConditionStruct();
            Enum.TryParse((string)gt[i, "ConditionType"], out cardEffect.conditionStruct.type);
            cardEffect.conditionStruct.strParameter = (string)gt[i, "ConditionStrParam"];
            cardEffect.conditionStruct.numValue = (string)gt[i, "ConditionValue"] == "" ? 0 : Int32.Parse((string)gt[i, "ConditionValue"]);

            Enum.TryParse((string)gt[i, "ExecutionTime"], out cardEffect.execTime);


            int level = Int32.Parse((string)gt[i, "UpgradeLevel"]);
            CardFunctionalityData data = level == 0 ? 
                                            TDataNameToAsset<CardFunctionalityData>(databaseName, new string[] { CardPath }) :
                                            FunctionalityAt(databaseName,level);

            data.effects.Add(cardEffect);
            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

    }

    public void ReadEntriesCardActivites(string sheetName, string lastCol)
    {
        GoogleTable gt = getGoogleTable(sheetName, lastCol);

        for (int i = 1; i < gt.values.Count; i++)
        {
            string databaseName = (string)gt[i, "DatabaseName"];
            if (databaseName.Equals(""))
                break;

            CardActivityData activity = new CardActivityData();
            Enum.TryParse((string)gt[i, "Activity"], out activity.type);
            Enum.TryParse((string)gt[i, "ExecutionTime"], out activity.execTime);

            activity.parameter = (string)gt[i, "Parameter"];

            int level = Int32.Parse((string)gt[i, "UpgradeLevel"]);
            
            CardFunctionalityData data = level == 0 ?
                                            TDataNameToAsset<CardFunctionalityData>(databaseName, new string[] { CardPath }) :
                                            FunctionalityAt(databaseName, level);
            data.activities.Add(activity);

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
            Profession profession;
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
        string folder = @"Assets/Artwork/Cards/";
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

    public void BindCharacterModel(EnemyData enemyData)
    {
        string folder = @"Assets/Artwork/Enemies/";
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
            cDataCard.Add(cardData.cost);

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
                    cDataCardActivity.Add(caSetting.parameter.ToString());
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
            data.enemyId        = (string)gt[i, "EnemyID"];
            data.StartingHP     = int.Parse((string)gt[i, "HP"]);
            data.tier           = int.Parse((string)gt[i, "Tier"]);
            data.experience     = int.Parse((string)gt[i, "Experience"]);

            data.shuffleInit    = bool.Parse((string)gt[i, "InitialShuffle"]);
            data.stochasticReshuffle = bool.Parse((string)gt[i, "StochasticReshuffle"]);
            
            data.deck.Clear();
            data.startingEffects.Clear();

            BindCharacterModel(data);

            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    public void ReadEntriesEnemyCards(string sheetName, string lastCol, string sheet)
    {
        GoogleTable gt = getGoogleTable(sheetName, lastCol, sheet);

        for (int i = 1; i < gt.values.Count; i++)
        {
            string databaseName = (string)gt[i, "DatabaseName"];
            if (databaseName.Equals(""))
                break;

            EnemyData data = TDataNameToAsset<EnemyData>(databaseName, new string[] { EnemyPath });
            if(data == null)
            {
                Debug.LogError("SKIPPED: Card assigned to non-existant enemy named: " + databaseName);
                continue;
            }

            string CardName = (string)gt[i, "CardName"];

            CardData cardData = TDataNameToAsset<CardData>(CardName, new string[] { CardPath });
            if(cardData == null)
            {
                Debug.LogError("SKIPPED: Enemy assigned non-existant card: " + CardName);
                continue;
            }

            data.deck.Add(cardData);
            
            EditorUtility.SetDirty(data);
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


    public void ReadEntriesArtifacts()
    {
        string sheetName = "Artifact";
        string lastCol = "H";
        string sheet = "Main";

        GoogleTable gt = getGoogleTable(sheetName, lastCol, sheet);
        for (int i = 1; i < gt.values.Count; i++)
        {
            AssetDatabase.SaveAssets();
            string databaseName = (string)gt[i, "DatabaseName"];
            if (databaseName.Equals(""))
                break;

            ArtifactData data = TDataNameToAsset<ArtifactData>(databaseName, new string[] { ArtifactPath });
            if (data is null)
            {
                data = ScriptableObject.CreateInstance<ArtifactData>();
                AssetDatabase.SaveAssets();
                AssetDatabase.CreateAsset(data, ArtifactPath + @"\" + databaseName + ".asset");
            }

            data.name = (string)gt[i, "DatabaseName"];
            data.itemName = (string)gt[i, "ItemName"];
            Enum.TryParse((string)gt[i, "Rarity"], out data.rarity);
            data.description = (string)gt[i, "Description"];

            //BindArt(data, databaseName);

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
}

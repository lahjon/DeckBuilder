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
    readonly string strFilePathBase = @"Assets\";
    static char[] separators = { '+', '-' };

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

    public void DownloadAll()
    {
        ReadEntriesCard("Card", "Z", "Main");
        ReadEntriesCardEffects("CardEffects", "Z", "Main");
    }



    public static void CreateCardData()
    {
        CardData asset = ScriptableObject.CreateInstance<CardData>();

        AssetDatabase.CreateAsset(asset, "Assets/NewScripableObject.asset");
        AssetDatabase.SaveAssets();
    }


    public void ReadEntriesCard(string sheetName, string lastCol, string sheet)
    {
        GoogleTable gt = getGoogleTable(sheetName, lastCol, sheet);

        for (int i = 1; i < gt.values.Count; i++)
        {
            string databaseName = (string)gt[i, "DatabaseName"];
            if (databaseName.Equals(""))
                break;

            CardData cardData = CardDataNameToAsset(databaseName);

            cardData.name = (string)gt[i, "Name"];
            cardData.cost = Int32.Parse((string)gt[i, "Cost"]);
            cardData.Damage.Type = EffectType.Damage;
            cardData.Damage.Value = Int32.Parse((string)gt[i, "DamageValue"]);
            cardData.Damage.Times = Int32.Parse((string)gt[i,"DamageTimes"]);
            Enum.TryParse((string)gt[i,"DamageTarget"], out cardData.Damage.Target);

            cardData.Block.Type = EffectType.Block;
            cardData.Block.Value = Int32.Parse((string)gt[i, "BlockValue"]);
            cardData.Block.Times = Int32.Parse((string)gt[i, "BlockTimes"]);

            cardData.Effects.Clear();


            BindArt(cardData, databaseName);

            EditorUtility.SetDirty(cardData);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

    }

    public void ReadEntriesCardEffects(string sheetName, string lastCol, string sheet)
    {
        GoogleTable gt = getGoogleTable(sheetName, lastCol, sheet);  

        for (int i = 1; i < gt.values.Count; i++)
        {
            string databaseName = (string)gt[i,"DatabaseName"];
            if (databaseName.Equals(""))
                break;

            CardData cardData = CardDataNameToAsset(databaseName);
            CardEffect cardEffect = new CardEffect();
            Enum.TryParse((string)gt[i, "EffectType"], out EffectType effectType);

            cardEffect.Type = effectType;
            cardData.Effects.Add(cardEffect);

            cardEffect.Value = Int32.Parse((string)gt[i, "Value"]);
            cardEffect.Times = Int32.Parse((string)gt[i, "Times"]);
            Enum.TryParse((string)gt[i, "TargetType"], out cardEffect.Target);

            EditorUtility.SetDirty(cardData);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

    }

    public CardData CardDataNameToAsset(string name)
    {
        string[] foldersToLookIn = { @"Assets\Cards" };
        CardData cardData;
        string[] result = AssetDatabase.FindAssets(name, foldersToLookIn);

        if (result.Length > 1)
            Debug.LogError("More than 1 Asset found for name:" + name);

        if (result.Length == 0)
        {
            cardData = ScriptableObject.CreateInstance<CardData>();
            AssetDatabase.CreateAsset(cardData, @"Assets\Cards\" + name + ".asset");
           
        }
        else
        {
            string path = AssetDatabase.GUIDToAssetPath(result[0]);
            Debug.Log("Paths of form: " + path);
            cardData = (CardData)AssetDatabase.LoadAssetAtPath<CardData>(path);
        }

        return cardData;
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


    GoogleTable getGoogleTable(string sheetName, string lastCol, string sheet)
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


    public void PrintCardData()
    {
        string[] strCardIDs = AssetDatabase.FindAssets("t:CardData", new string[] { "Assets/Cards" });
        List<IList<object>> InputDataCard = new List<IList<object>>();
        List<IList<object>> InputDataCardEffects = new List<IList<object>>();
        foreach (string ID in strCardIDs)
        {
            List<object> cDataCard = new List<object>();

            string lAssetPathCard = AssetDatabase.GUIDToAssetPath(ID);
            CardData cardData = (CardData)AssetDatabase.LoadAssetAtPath<CardData>(lAssetPathCard);

            cDataCard.Add(cardData.characterClass.ToString());
            cDataCard.Add(cardData.name);
            cDataCard.Add(cardData.name);
            cDataCard.Add(cardData.cost);
            cDataCard.Add(cardData.Damage.Value);
            cDataCard.Add(cardData.Damage.Times);
            cDataCard.Add(cardData.Damage.Target.ToString());
            cDataCard.Add(cardData.Block.Value);
            cDataCard.Add(cardData.Block.Times);

            InputDataCard.Add(cDataCard);

            if (cardData.Effects.Count != 0)
            {
                foreach (CardEffect effect in cardData.Effects)
                {
                    List<object> cDataCardEffect = new List<object>();
                    cDataCardEffect.Add(cardData.name);
                    cDataCardEffect.Add(cardData.name);
                    cDataCardEffect.Add(cardData.Effects.Contains(effect) ? "Enemy" : "Self");
                    cDataCardEffect.Add(effect.Type.ToString());
                    cDataCardEffect.Add(effect.Times);
                    cDataCardEffect.Add(effect.Value);
                    cDataCardEffect.Add(effect.Target.ToString());
                    InputDataCardEffects.Add(cDataCardEffect);
                }
            }
        }

        GoogleUpdateData(InputDataCard, "Upl_Card", "A2:Z" + (InputDataCard.Count+100).ToString());
        GoogleUpdateData(InputDataCardEffects, "Upl_CardEffects", "A2:Z" + (InputDataCardEffects.Count + 100).ToString());
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
}

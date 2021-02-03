using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
        ReadEntries("Card.csv", "Card", "H", "Main");
    }



    public void ReadEntries(string TargetFileName, string sheetName, string lastCol, string sheet)
    {
        string FilePath = strFilePathBase + TargetFileName;
        if (File.Exists(FilePath))
            File.Delete(FilePath);

        int nrColumns = 0;
        string range = sheetName + "!A1:" + lastCol + "10000";
        var request = service.Spreadsheets.Values.Get(SpreadSheetIDs[sheet], range);
        var response = request.Execute();
        var values = response.Values;

        if (values != null & values.Count > 0)
        {
            for (int i = 0; i < values[0].Count; i++)
            {
                if ((string)values[0][i] != "")
                    nrColumns++;
                else
                    break;
            }

            //Start looping up in hea
            using (StreamWriter sw = new StreamWriter(FilePath, true, Encoding.UTF8, 65536))
            {
                for (int row = 0; row < values.Count; row++)
                {
                    string writeMe = "";

                    for (int i = 0; i < Math.Min(nrColumns, values[row].Count); i++)
                        writeMe += values[row][i] + ";";

                    sw.WriteLine(writeMe);
                }
            }
        }
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Apis.Sheets.v4.Data;

public class GoogleTable
{
    public Dictionary<string,int> ColNames = new Dictionary<string, int>();
    public IList<IList<System.Object>> values;

    public System.Object this[int row, string colName] => values[row][ColNames[colName]];
}

using System.Collections.Generic;
using UnityEngine;

public static class SaveDataManager
{
    // use saveSlot_0 for progression save

    public static void SaveJsonData(IEnumerable<ISaveable> a_Saveables, int saveSlot = 0)
    {
        string fileName = string.Format("saveFile_{0}", saveSlot);

        SaveData sd = new SaveData();
        foreach (var saveable in a_Saveables)
        {
            saveable.PopulateSaveData(sd);
        }

        if (FileManager.WriteToFile(fileName, sd.ToJson()))
        {
            Debug.Log("Save successful");
        }
    }
    
    public static void LoadJsonData(IEnumerable<ISaveable> a_Saveables, int saveSlot = 0)
    {
        string fileName = string.Format("saveFile_{0}", saveSlot);

        if (FileManager.LoadFromFile(fileName, out var json))
        {
            SaveData sd = new SaveData();
            sd.LoadFromJson(json);

            foreach (var saveable in a_Saveables)
            {
                saveable.LoadFromSaveData(sd);
            }
            
            Debug.Log("Load complete");
        }
    }
}
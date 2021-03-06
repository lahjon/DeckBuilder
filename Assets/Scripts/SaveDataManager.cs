using System.Collections.Generic;
using UnityEngine;

public static class SaveDataManager
{
    public static string saveFileName = "progression_data";
    public static string saveFileNameTemp = "run_data";
    public static string saveFileNameStart = "start_data";


    public static void SaveJsonData(IEnumerable<ISaveableWorld> a_Saveables)
    {
        string fileName = saveFileName;

        SaveDataWorld sd = new SaveDataWorld();
        foreach (var saveable in a_Saveables)
        {
            saveable.PopulateSaveDataWorld(sd);
        }

        if (FileManager.WriteToFile(fileName, sd.ToJson()))
        {
            Debug.Log("Save successful");
        }
    }
    
    public static void LoadJsonData(IEnumerable<ISaveableWorld> a_Saveables)
    {
        string fileName = saveFileName;

        if (FileManager.LoadFromFile(fileName, out var json))
        {
            SaveDataWorld sd = new SaveDataWorld();
            sd.LoadFromJson(json);

            foreach (var saveable in a_Saveables)
            {
                saveable.LoadFromSaveDataWorld(sd);
            }
            
            Debug.Log("Load complete");
        }
    }

    // character_01 character_02 character_03 character_04

    public static void SaveJsonData(IEnumerable<ISaveableCharacter> a_Saveables, int saveSlot = 0)
    {
        string fileName = string.Format("character_{0}", saveSlot);

        SaveDataCharacter sd = new SaveDataCharacter();
        foreach (var saveable in a_Saveables)
        {
            saveable.PopulateSaveDataCharacter(sd);
        }

        if (FileManager.WriteToFile(fileName, sd.ToJson()))
        {
            Debug.Log("Save successful");
        }
    }
    public static void LoadJsonData(IEnumerable<ISaveableCharacter> a_Saveables, int saveSlot = 0)
    {
        string fileName = string.Format("character_{0}", saveSlot);

        if (FileManager.LoadFromFile(fileName, out var json))
        {
            SaveDataCharacter sd = new SaveDataCharacter();
            sd.LoadFromJson(json);

            foreach (var saveable in a_Saveables)
            {
                saveable.LoadFromSaveDataCharacter(sd);
            }
            
            Debug.Log("Load complete");
        }
    }

        // character_01 character_02 character_03 character_04

    public static void SaveJsonData(IEnumerable<ISaveableTemp> a_Saveables)
    {
        string fileName = saveFileNameTemp;

        SaveDataTemp sd = new SaveDataTemp();
        foreach (var saveable in a_Saveables)
        {
            saveable.PopulateSaveDataTemp(sd);
        }

        if (FileManager.WriteToFile(fileName, sd.ToJson()))
        {
            Debug.Log("Save successful");
        }
    }
    public static void LoadJsonData(IEnumerable<ISaveableTemp> a_Saveables)
    {
        string fileName = saveFileNameTemp;

        if (FileManager.LoadFromFile(fileName, out var json))
        {
            SaveDataTemp sd = new SaveDataTemp();
            sd.LoadFromJson(json);

            foreach (var saveable in a_Saveables)
            {
                saveable.LoadFromSaveDataTemp(sd);
            }
            
            Debug.Log("Load complete");
        }
    }

    public static void SaveJsonData(IEnumerable<ISaveableStart> a_Saveables)
    {
        string fileName = saveFileNameStart;

        SaveDataStart sd = new SaveDataStart();
        foreach (var saveable in a_Saveables)
        {
            saveable.PopulateSaveDataStart(sd);
        }

        if (FileManager.WriteToFile(fileName, sd.ToJson()))
        {
            Debug.Log("Save successful");
        }
    }
    public static void LoadJsonData(IEnumerable<ISaveableStart> a_Saveables)
    {
        string fileName = saveFileNameStart;

        if (FileManager.LoadFromFile(fileName, out var json))
        {
            SaveDataStart sd = new SaveDataStart();
            sd.LoadFromJson(json);

            foreach (var saveable in a_Saveables)
            {
                saveable.LoadFromSaveDataStart(sd);
            }
            
            Debug.Log("Load complete");
        }
    }
}
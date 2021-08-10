using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public static class FileManager
{
    public static bool WriteToFile(string a_FileName, string a_FileContents)
    {
        string fullPath = Path.Combine(Application.persistentDataPath, a_FileName);
        Debug.Log(fullPath); // keep this debug line, its useful for finding the save data file

        try
        {
            File.WriteAllText(fullPath, a_FileContents);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to write to {fullPath} with exception {e}");
            return false;
        }
    }
    public static bool LoadFromFile(string a_FileName, out string result)
    {
        string fullPath = Path.Combine(Application.persistentDataPath, a_FileName);

        try
        {
            result = File.ReadAllText(fullPath);
            return true;
        }
        catch (Exception e1)
        {
            Debug.Log($"Failed to read from {fullPath} with exception {e1}. Writing new file and trying to reload");
            result = "";
            WriteToFile(a_FileName, result);
            try
            {
                result = File.ReadAllText(fullPath);
                return true;
            }
            catch (Exception e2)
            {
                Debug.Log($"Failed to read from {fullPath} with exception {e2}. Cancel operation");
                return false;
            }
        }
    }

    public static bool ResetTempData()
    {
        string fullPath = Path.Combine(Application.persistentDataPath, SaveDataManager.saveFileNameTemp);

        try
        {
            File.WriteAllText(fullPath, "");
            return true;
        }
        catch (Exception e)
        {
            Debug.Log($"Failed to read from {fullPath} with exception {e}. Writing new file");
            return false;
        }
    }
}
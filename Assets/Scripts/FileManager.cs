using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public static class FileManager
{
    public static bool WriteToFile(string a_FileName, string a_FileContents)
    {
        var fullPath = Path.Combine(Application.persistentDataPath, a_FileName);
        Debug.Log(fullPath);

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
        var fullPath = Path.Combine(Application.persistentDataPath, a_FileName);

        try
        {
            result = File.ReadAllText(fullPath);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to read from {fullPath} with exception {e}");
            result = "";
            return false;
        }
    }

    public static bool RemoveFile(string a_FileName)
    {
        var fullPath = Path.Combine(Application.persistentDataPath, a_FileName);

        try
        {
            File.Delete(fullPath);
            Debug.Log("Delete path!");
            Debug.Log(fullPath);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to delete {a_FileName} from {fullPath} with exception {e}");
            return false;
        }
    }
}
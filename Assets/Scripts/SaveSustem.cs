using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static string GetPath(int slot)
    {
        return Application.persistentDataPath + $"/SaveSlot{slot}.json";
    }

    // Simpan ke slot tertentu
    public static void Save(PlayerData data, int slot)
    {
        data.saveTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetPath(slot), json);
        Debug.Log($"Saved to slot {slot} at {GetPath(slot)}");
    }

    // Load dari slot tertentu
    public static PlayerData Load(int slot)
    {
        string path = GetPath(slot);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);
            Debug.Log($"Loaded from slot {slot}");
            return data;
        }

        Debug.LogWarning($"No save found in slot {slot}");
        return null;
    }

    // Cek apakah slot punya data
    public static bool HasSave(int slot)
    {
        return File.Exists(GetPath(slot));
    }

    // Hapus slot
    public static void Delete(int slot)
    {
        string path = GetPath(slot);
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"Deleted save slot {slot}");
        }
    }
}

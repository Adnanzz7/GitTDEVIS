using System.IO;
using UnityEngine;

public class SaveManager
{
    const string FILE_NAME = "save_data.json";

    bool dirty;
    bool hasSlept;

    string FullPath => Path.Combine(Application.persistentDataPath, FILE_NAME);

    public void MarkDirty() => dirty = true;
    public void MarkSlept() { dirty = false; hasSlept = true; SavePlayerImmediate(); }

    public bool IsUnsaved() => !hasSlept || dirty;

    public void SavePlayerImmediate()
    {
        var pdata = GameManager.Instance.PlayerManager.CreateSave();
        File.WriteAllText(FullPath, JsonUtility.ToJson(pdata, true));
        Debug.Log($"[Save] data tersimpan ke {FullPath}");
        dirty = false; hasSlept = true;
    }

    public bool LoadPlayer()
    {
        if (!File.Exists(FullPath))
        {
            Debug.Log("[Save] belum ada file save.");
            return false;
        }

        string json = File.ReadAllText(FullPath);
        var pdata = JsonUtility.FromJson<PlayerManager.SaveData>(json);
        GameManager.Instance.PlayerManager.LoadSave(pdata);
        hasSlept = true;
        Debug.Log("[Save] data berhasil dimuat.");
        return true;
    }
}

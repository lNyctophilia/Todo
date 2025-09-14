using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class StreakSaveManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform streakScrollRectContent;
    public string SavePath => Application.persistentDataPath + "/streaksave.json";

    [Header("Data")]
    [SerializeField] private List<Streak> streaks = new List<Streak>();

    public static StreakSaveManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void Save(bool allowEmpty = false)
    {
        StreakListWrapper wrapper = new StreakListWrapper();

        for (int i = 0; i < streakScrollRectContent.childCount; i++)
        {
            Transform child = streakScrollRectContent.GetChild(i);
            if (child.TryGetComponent<StreakContent>(out StreakContent streakContent))
                wrapper.streaks.Add(streakContent.streak);
        }

        if (wrapper.streaks.Count == 0 && !allowEmpty)
        {
            Debug.LogWarning("Boş streak kaydı alınmadı, mevcut dosya korunuyor.");
            return;
        }

        string json = JsonUtility.ToJson(wrapper, true);
        string tempPath = SavePath + ".tmp";
        File.WriteAllText(tempPath, json);
        File.Copy(tempPath, SavePath, true);
        File.Delete(tempPath);

        Debug.Log("Streak kaydedildi: " + SavePath);
        Backup();
    }

    public void Backup()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("Backup alınamadı, kayıt dosyası yok.");
            return;
        }

        string backupDir = Application.persistentDataPath + "/Backups/Streak";
        if (!Directory.Exists(backupDir))
            Directory.CreateDirectory(backupDir);

        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string backupPath = Path.Combine(backupDir, $"streaksave_{timestamp}.json");

        File.Copy(SavePath, backupPath, true);
        Debug.Log("Streak backup kaydedildi: " + backupPath);
    }

    public List<Streak> Load()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("Kayıt dosyası bulunamadı.");
            return new List<Streak>();
        }

        string json = File.ReadAllText(SavePath);
        if (string.IsNullOrEmpty(json))
        {
            Debug.LogError("Streak dosyası boş.");
            return new List<Streak>();
        }

        StreakListWrapper wrapper = JsonUtility.FromJson<StreakListWrapper>(json);
        if (wrapper == null || wrapper.streaks == null)
        {
            Debug.LogError("Streak parse edilemedi.");
            return new List<Streak>();
        }

        streaks = wrapper.streaks;
        Debug.Log("Kayıt yüklendi, " + streaks.Count + " adet streak bulundu.");
        return streaks;
    }
}

[System.Serializable]
public class StreakListWrapper
{
    public List<Streak> streaks = new List<Streak>();
}
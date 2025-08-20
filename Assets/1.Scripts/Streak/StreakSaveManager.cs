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

    public void Save()
    {
        StreakListWrapper wrapper = new StreakListWrapper();
        wrapper.streaks.Clear();

        for (int i = 0; i < streakScrollRectContent.childCount; i++)
        {
            Transform child = streakScrollRectContent.GetChild(i);
            if (child.TryGetComponent<StreakContent>(out StreakContent streakContent))
                wrapper.streaks.Add(streakContent.streak);
        }

        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(SavePath, json);
        Debug.Log("Kayıt yapıldı: " + SavePath);
    }

    public List<Streak> Load()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("Kayıt dosyası bulunamadı.");
            return null;
        }

        string json = File.ReadAllText(SavePath);
        StreakListWrapper wrapper = JsonUtility.FromJson<StreakListWrapper>(json);
        streaks = wrapper.streaks;
        Debug.Log("Kayıt yüklendi, " + streaks.Count + " adet streak bulundu.");
        return streaks;
    }

    private void OnApplicationQuit()
    {
        Save();
    }
    private void OnApplicationPause(bool pause)
    {
        if (pause) Save();
    }
}
[System.Serializable]
public class StreakListWrapper
{
    public List<Streak> streaks = new List<Streak>();
}

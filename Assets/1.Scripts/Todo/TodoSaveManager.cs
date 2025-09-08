using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class TodoSaveManager : MonoBehaviour
{
    public static TodoSaveManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    public string SavePath => Application.persistentDataPath + "/todosave.json";

    public void Save(List<Category> todoCategories)
    {
        TodoSaveFile saveFile = new TodoSaveFile();

        foreach (Category cat in todoCategories)
        {
            SerializableCategory sCat = new SerializableCategory();
            sCat.Id = cat.Id;
            sCat.ScrollbarId = cat.ScrollBarId;
            sCat.Title = cat.Title;

            foreach (Todo t in cat.Todos)
            {
                SerializableTodo sTodo = new SerializableTodo
                {
                    Title = t.Title,
                    Id = t.Id,
                    IsCompleted = t.IsCompleted
                };
                sCat.Todos.Add(sTodo);
            }

            saveFile.categories.Add(sCat);
        }

        string json = JsonUtility.ToJson(saveFile, true);
        File.WriteAllText(SavePath, json);
        Debug.Log("Kayıt yapıldı: " + SavePath);
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
    public List<SerializableCategory> Load()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("Kayıt dosyası bulunamadı.");
            return null;
        }

        string json = File.ReadAllText(SavePath);
        TodoSaveFile saveFile = JsonUtility.FromJson<TodoSaveFile>(json);
        Debug.Log("Kayıt yüklendi.");
        return saveFile.categories;
    }
}

[System.Serializable]
public class SerializableTodo
{
    public string Title;
    public int Id;
    public bool IsCompleted;
}

[System.Serializable]
public class SerializableCategory
{
    public int Id;
    public int ScrollbarId;
    public string Title;
    public List<SerializableTodo> Todos = new List<SerializableTodo>();
}

[System.Serializable]
public class TodoSaveFile
{
    public List<SerializableCategory> categories = new List<SerializableCategory>();
}

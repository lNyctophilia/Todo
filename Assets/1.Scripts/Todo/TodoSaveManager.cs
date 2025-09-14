using System;
using System.Collections.Generic;
using System.IO;
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
            SerializableCategory sCat = new SerializableCategory
            {
                Id = cat.Id,
                ScrollbarId = cat.ScrollBarId,
                Title = cat.Title
            };

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

        if (saveFile.categories.Count == 0)
        {
            Debug.LogWarning("Boş kayıt alınmadı, mevcut dosya korunuyor.");
            return;
        }

        string json = JsonUtility.ToJson(saveFile, true);

        // Geçici dosya yöntemi
        string tempPath = SavePath + ".tmp";
        File.WriteAllText(tempPath, json);
        File.Copy(tempPath, SavePath, true);
        File.Delete(tempPath);

        Debug.Log("Kayıt yapıldı: " + SavePath);

        Backup();
    }

    public void Backup()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("Backup alınamadı, kayıt dosyası yok.");
            return;
        }

        string backupDir = Application.persistentDataPath + "/Backups/Todo";
        if (!Directory.Exists(backupDir))
            Directory.CreateDirectory(backupDir);

        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string backupPath = Path.Combine(backupDir, $"todosave_{timestamp}.json");

        File.Copy(SavePath, backupPath, true);
        Debug.Log("Todo backup kaydedildi: " + backupPath);
    }

    public List<SerializableCategory> Load()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("Kayıt dosyası bulunamadı.");
            return new List<SerializableCategory>();
        }

        string json = File.ReadAllText(SavePath);
        if (string.IsNullOrEmpty(json))
        {
            Debug.LogError("Kayıt dosyası boş.");
            return new List<SerializableCategory>();
        }

        TodoSaveFile saveFile = JsonUtility.FromJson<TodoSaveFile>(json);
        if (saveFile == null || saveFile.categories == null)
        {
            Debug.LogError("Kayıt parse edilemedi.");
            return new List<SerializableCategory>();
        }

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
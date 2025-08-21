using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class StreakManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform streakScrollRectContent;
    [SerializeField] private TMP_InputField titleInputField;
    [SerializeField] private TMP_InputField dateInputField;
    [SerializeField] private GameObject streakContentPrefab;
    [SerializeField] private Text emptyText;

    public static StreakManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        LoadStreaks();
    }

    public void AddStreak()
    {
        if (titleInputField == null || titleInputField.text.Length <= 0) return;
        if (dateInputField == null || GetDate() == DateTime.MinValue || dateInputField.text.Length <= 0) return;

        GameObject newStreak = Instantiate(streakContentPrefab, streakScrollRectContent);

        if (newStreak.TryGetComponent<StreakContent>(out StreakContent streakContent))
        {
            streakContent.streak.Id = GetAvailableStreakId();
            streakContent.streak.Title = titleInputField.text;
            streakContent.streak.StartDateTicks = GetDate().Ticks;
        }

        titleInputField.text = null;
        dateInputField.text = null;

        SetEmptyText();
    }
    public void DeleteStreak(Streak streak)
    {
        Destroy(GetStreakGameObject(streak.Id));

        Invoke(nameof(SetEmptyText), 0.01f);
    }
    public void SetEmptyText()
    {
        if (streakScrollRectContent.childCount <= 0)
        {
            emptyText.gameObject.SetActive(true);
            emptyText.text = "Streaks is Empty";
        }
        else
        {
            emptyText.gameObject.SetActive(false);
        }
    }
    private GameObject GetStreakGameObject(int id)
    {
        foreach (Transform child in streakScrollRectContent)
        {
            if (child.TryGetComponent<StreakContent>(out StreakContent streakContent))
            {
                if (streakContent.streak.Id == id)
                {
                    return child.gameObject;
                }
            }
        }
        return null;
    }
    public DateTime GetDate()
    {
        if (DateTime.TryParse(dateInputField.text, out DateTime result))
        {
            return result; // başarıyla parse edildi
        }
        else
        {
            Debug.LogWarning("Geçersiz tarih formatı!");
            return DateTime.MinValue;
        }
    }
    private int GetAvailableStreakId()
    {
        while (true)
        {
            int id = UnityEngine.Random.Range(0, 100000);
            foreach (Transform child in streakScrollRectContent)
            {
                if (child.TryGetComponent<StreakContent>(out StreakContent streakContent))
                {
                    if (streakContent.streak.Id != id)
                    {
                        return id;
                    }
                }
            }
            return -1;
        }
    }
    private void LoadStreaks()
    {
        var streaks = StreakSaveManager.Instance?.Load();
        if (streaks == null) return;

        foreach (Streak streak in streaks)
        {
            GameObject newStreak = Instantiate(streakContentPrefab, streakScrollRectContent);
            if (newStreak.TryGetComponent<StreakContent>(out StreakContent streakContent))
            {
                streakContent.streak = streak;
            }
        }
    }
}

[Serializable]
public class Streak {
    public int Id;
    public string Title;
    public long StartDateTicks; // uzun sayı
}
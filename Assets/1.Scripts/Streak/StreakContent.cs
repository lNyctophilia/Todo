using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class StreakContent : MonoBehaviour
{
    [Header("References")]
    private TextMeshProUGUI titleTMP;
    private TextMeshProUGUI startDateTMP;
    private TextMeshProUGUI totalDayCountTMP;
    private Button trashButton;

    [Header("Data")]
    private DateTime StartDate;
    public Streak streak = new Streak();

    private void Awake()
    {
        titleTMP = transform.Find("Title").GetComponent<TextMeshProUGUI>();
        startDateTMP = transform.Find("StartDate").GetComponent<TextMeshProUGUI>();
        totalDayCountTMP = transform.Find("TotalDayCount").GetComponent<TextMeshProUGUI>();
        trashButton = transform.Find("TrashButton").GetComponent<Button>();
    }

    private void Start()
    {
        // Kaydedilen total day count'tan başlangıç tarihi hesapla
        StartDate = DateTime.Now.AddDays(-streak.TotalDayCount);

        RefreshUI();

        trashButton.onClick.AddListener(() => StreakManager.Instance.DeleteStreak(streak));
    }

    private void OnEnable()
    {
        // Tekrar enable olduğunda UI'yi güncelle
        RefreshUI();
    }

    private void RefreshUI()
    {
        titleTMP.text = streak.Title;
        startDateTMP.text = StartDate.ToString("dd.MM.yyyy") + " :";
        totalDayCountTMP.text = "Day " + ((DateTime.Now - StartDate).Days + 1).ToString();
    }
}

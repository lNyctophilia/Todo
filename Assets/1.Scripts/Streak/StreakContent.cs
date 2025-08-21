using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Globalization;

public class StreakContent : MonoBehaviour
{
    [Header("References")]
    private TextMeshProUGUI titleTMP;
    private TextMeshProUGUI startDateTMP;
    private TextMeshProUGUI totalDayCountTMP;
    private Button trashButton;

    [Header("Data")]
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
        RefreshUI();
        
        trashButton.onClick.AddListener(() => StreakManager.Instance?.DeleteStreak(streak));
    }
    private void OnEnable()
    {
        RefreshUI();
    }
    private void RefreshUI()
    {
        DateTime startDate = new DateTime(streak.StartDateTicks);

        titleTMP.text = streak.Title;
        startDateTMP.text = startDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        totalDayCountTMP.text = "Day " + ((DateTime.Now - startDate).Days + 1).ToString();
    }
}

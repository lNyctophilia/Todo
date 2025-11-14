using System;
using UnityEngine;
using UnityEngine.UI;

public class Warning : MonoBehaviour
{
    public static Warning Instance { get; private set; }

    public Button DeleteButton;
    public GameObject CloseButton;
    public GameObject WarningPanel;

    private void Awake()
    {
        Instance = this;
    }

    public void SetWarningScreen(bool active, Action act)
    {
        CloseButton.SetActive(active);
        WarningPanel.SetActive(active);

        if (active)
            DeleteButton.onClick.AddListener(() => act());
        else
            DeleteButton.onClick.RemoveAllListeners();
    }

    public void CloseWarningScreen() => SetWarningScreen(false, null);
}

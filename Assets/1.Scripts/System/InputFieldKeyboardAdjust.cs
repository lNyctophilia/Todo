using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputFieldKeyboardAdjust : MonoBehaviour, IDeselectHandler
{
    public RectTransform panelToMove;
    public Toggle panelToggle;
    private Vector2 originalPos;

    void Start()
    {
        originalPos = panelToMove.anchoredPosition;
    }

    void Update()
    {
    #if UNITY_ANDROID || UNITY_IOS
        // Android geri tuşu ile kapatma
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            panelToMove.gameObject.SetActive(false);
            if (panelToggle) panelToggle.isOn = false;
            EventSystem.current.SetSelectedGameObject(null);
        }

        // Klavye yüksekliği (UI birimi)
        int kbd = MobileUtilities.GetKeyboardHeightUI(panelToMove);

        // Ufak oynama/jitter engellemek için bir eşik koyduk
        if (kbd > 10)
            panelToMove.anchoredPosition = originalPos + new Vector2(0, kbd);
        else
            panelToMove.anchoredPosition = originalPos;
    #endif
    }

    public void OnDeselect(BaseEventData eventData)
    {
        panelToMove.anchoredPosition = originalPos;
    }
}

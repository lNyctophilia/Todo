using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputFieldKeyboardAdjust : MonoBehaviour, ISelectHandler, IDeselectHandler
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
        if (Input.GetKeyDown(KeyCode.Escape)) // Android geri tuşu
        {
            panelToMove.gameObject.SetActive(false);
            panelToggle.isOn = false;
            EventSystem.current.SetSelectedGameObject(null); // focusu temizle
        }
#endif
    }

    public void OnSelect(BaseEventData eventData)
    {
#if UNITY_ANDROID || UNITY_IOS
        float height = TouchScreenKeyboard.area.height;
        panelToMove.anchoredPosition = originalPos + new Vector2(0, height);
        panelToMove.gameObject.SetActive(true);
#endif
    }

    public void OnDeselect(BaseEventData eventData)
    {
        // artık burada paneli kapatma yok!
        panelToMove.anchoredPosition = originalPos;
    }
}

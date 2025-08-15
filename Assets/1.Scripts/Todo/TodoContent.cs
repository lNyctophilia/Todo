using UnityEngine;
using UnityEngine.UI;

public class TodoContent : MonoBehaviour
{
    public Todo todo;
    private TMPro.TextMeshProUGUI title;
    private Toggle toggle;
    private Button trashButton;
    private Image contentImage;
    
    private void Start()
    {
        contentImage = GetComponent<Image>();
        title = GetComponentInChildren<TMPro.TextMeshProUGUI>();
        toggle = GetComponentInChildren<Toggle>();
        trashButton = GetComponentInChildren<Button>();


        toggle.isOn = todo.IsCompleted;
        title.text = todo.Title;
        contentImage.color = toggle.isOn ? new Color(0.1803922f, 0.1803922f, 0.1803922f, 0.7f) :  new Color(0.1803922f, 0.1803922f, 0.1803922f, 1f);
        title.fontStyle = toggle.isOn ? TMPro.FontStyles.Strikethrough : TMPro.FontStyles.Normal;


        toggle.onValueChanged.AddListener(value =>
        {
            todo.IsCompleted = value;
            toggle.isOn = value;
            contentImage.color = value ? new Color(0.1803922f, 0.1803922f, 0.1803922f, 0.7f) :  new Color(0.1803922f, 0.1803922f, 0.1803922f, 1f);
            title.fontStyle = value ? TMPro.FontStyles.Strikethrough : TMPro.FontStyles.Normal;
            //if (value) TodoManager.Instance.ClickTodo(value, todo);
        });
        trashButton.onClick.AddListener(() => TodoManager.Instance.DeleteTodo(todo));
    }
}
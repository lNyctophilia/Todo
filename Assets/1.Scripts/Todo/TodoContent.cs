using UnityEngine;
using UnityEngine.UI;

public class TodoContent : MonoBehaviour
{
    public Todo todo;
    private Text title;
    private Button button;
    private Toggle toggle;
    private void Start()
    {
        todo.Item = gameObject;
        title = GetComponentInChildren<Text>();
        button = GetComponent<Button>();
        toggle = GetComponentInChildren<Toggle>();

        title.text = $"- {todo.Title}";
        button.onClick.AddListener(() =>
        {
            TodoManager.Instance.ClickTodo(todo, toggle, button);
        });
    }
}
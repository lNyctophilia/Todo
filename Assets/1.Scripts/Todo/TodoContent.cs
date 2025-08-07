using UnityEngine;
using UnityEngine.UI;

public class TodoContent : MonoBehaviour
{
    public Todo todo;
    private Text title;
    private Button button;
    private void Start()
    {
        todo.Item = gameObject;
        title = GetComponentInChildren<Text>();
        button = GetComponent<Button>();

        title.text = $"- {todo.Title}";
        button.onClick.AddListener(() =>
        {
            TodoManager.Instance.ClickTodo(todo);
        });
    }
}
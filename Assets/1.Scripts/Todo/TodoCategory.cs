using UnityEngine;
using UnityEngine.UI;

public class TodoCategory : MonoBehaviour
{
    public Category category;
    private Text text;
    private Button button;
    void Start()
    {
        text = GetComponentInChildren<Text>();
        button = GetComponent<Button>();
        text.text = category.Name;
        button.onClick.AddListener(() => TodoManager.Instance.ClickCategory(category));
    }
}

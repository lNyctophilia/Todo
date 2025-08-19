using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TodoCategory : MonoBehaviour
{
    public Category category;
    private Text text;
    private Button clickButton;
    private Button trashButton;

    public void Setup(Category cat)
    {
        category = cat;
        text = transform.GetChild(2).GetComponent<Text>();
        clickButton = transform.GetChild(0).GetComponent<Button>();
        trashButton = transform.GetChild(1).GetComponent<Button>();

        if (category != null)
            text.text = category.Title;

        clickButton.onClick.AddListener(() => TodoManager.Instance.ClickCategory(category));

        trashButton.onClick.AddListener(() => TodoManager.Instance.DeleteCategory(category));
    }
}

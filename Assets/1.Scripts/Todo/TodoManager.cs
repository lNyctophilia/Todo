using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TodoManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject TodoContentPrefab;
    [SerializeField] private Transform ContentParent;
    [SerializeField] private InputField TodoInputField;

    [Space(10)]

    [SerializeField] private GameObject TodoTextGameObject;
    [SerializeField] private GameObject TodoNowTextGameObject;
    [SerializeField] private GameObject DoneTextGameObject;

    [Header("Data")]
    private List<GameObject> todoContents = new List<GameObject>();

    [Header("Settings")]
    [SerializeField] private TodoProcess currentProcess = TodoProcess.Add;

    public static TodoManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    public void AddTodo()
    {
        string todo = TodoInputField.text;

        if (todo == null || todo.Length == 0) return;

        GameObject todoContent = InstantiateBelow(TodoContentPrefab, TodoTextGameObject);

        TodoContent content = todoContent.GetComponent<TodoContent>();
        content.todo.Title = todo;
        content.todo.Id = GetAvailableId();
        content.todo.IsCompleted = false;

        Toggle toggle = todoContent.GetComponent<Toggle>();
        if (toggle != null) toggle.isOn = false;

        TodoInputField.text = null;
    }
    public void ClickTodo(Todo todo, Toggle toggle, Button button)
    {
        if (currentProcess == TodoProcess.Add)
        {
            if (todo.IsReceived == false)
            {
                todo.IsReceived = true;
                MoveTodo(todo, TodoNowTextGameObject);
            }
            else if (todo.IsCompleted == false && todo.IsReceived == true)
            {
                todo.IsCompleted = true;
                MoveTodo(todo, DoneTextGameObject);
            }
            toggle.isOn = todo.IsCompleted;
        }
        else if (currentProcess == TodoProcess.Delete)
        {
            DeleteTodo(todo);
        }
        else if (currentProcess == TodoProcess.Reset)
        {
            ResetTodo(todo);
        }
        ChangeProcess("Add");
    }
    public void ChangeProcess(string process)
    {
        switch (process)
        {
            case "Add":
                currentProcess = TodoProcess.Add;
                break;
            case "Delete":
                currentProcess = TodoProcess.Delete;
                break;
            case "Reset":
                currentProcess = TodoProcess.Reset;
                break;
        }
    }
    public void ResetTodo(Todo todo)
    {
        todo.IsCompleted = false;
        todo.IsReceived = false;
        todo.SetToggle(false);
        MoveTodo(todo, TodoTextGameObject);
    }
    public void DeleteTodo(Todo todo)
    {
        Destroy(todo.Item);
    }
    private void MoveTodo(Todo todo, GameObject target)
    {
        GameObject obj = todo.Item;
        int targetIndex = target.transform.GetSiblingIndex();
        obj.transform.SetSiblingIndex(targetIndex + 1);
    }
    private GameObject InstantiateBelow(GameObject prefab, GameObject target)
    {
        GameObject newObj = Instantiate(prefab, target.transform.parent);
        int targetIndex = target.transform.GetSiblingIndex();
        newObj.transform.SetSiblingIndex(targetIndex + 1);
        todoContents.Add(newObj);
        return newObj;
    }
    private int GetAvailableId()
    {
        HashSet<int> existingIds = new HashSet<int>();
        for (int i = 0; i < ContentParent.childCount; i++)
        {
            TodoContent content = ContentParent.GetChild(i).GetComponent<TodoContent>();
            if (content != null)
            {
                existingIds.Add(content.todo.Id);
            }
        }
        while (true)
        {
            int id = Random.Range(0, 10000);
            if (!existingIds.Contains(id))
                return id;
        }
    }
}
[System.Serializable]
public class Todo
{
    public int Id;
    public string Title;
    public GameObject Item;
    public bool IsReceived;
    public bool IsCompleted;
    public void SetToggle(bool state)
    {
        Toggle toggle = Item.GetComponentInChildren<Toggle>();
        if (toggle != null) toggle.isOn = state;
    }
}
public enum TodoProcess
{
    Add,
    Delete,
    Reset
}
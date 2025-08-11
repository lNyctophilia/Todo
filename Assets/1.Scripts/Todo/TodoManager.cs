using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.IO;

public class TodoManager : MonoBehaviour
{
    [Header("Category References")]
    [SerializeField] private GameObject CategoryPrefab;
    [SerializeField] private GameObject CategoryScrollBarPrefab;
    [SerializeField] private Transform CategoryParent;
    [SerializeField] private Transform CategoryScrollBarParent;


    [Header("Todo References")]
    [SerializeField] private GameObject TodoContentPrefab;
    [SerializeField] private InputField TodoInputField;
    [SerializeField] private Toggle TodoToggle;

    [Space(10)]

    [SerializeField] private GameObject TodoTextGameObject;
    [SerializeField] private GameObject TodoNowTextGameObject;
    [SerializeField] private GameObject DoneTextGameObject;

    [Header("Data")]
    private List<Category> todoCategories = new List<Category>();

    [Header("Settings")]
    [SerializeField] private TodoProcess currentProcess = TodoProcess.Add;
    [SerializeField] private int activeCategoryId;

    public static TodoManager Instance;
    private void Awake()
    {
        Instance = this;

        TodoInputField.text = "Default";
        AddCategory();
        TodoInputField.text = null;
    }
    private void Start()
    {
        LoadTodos();

        ClickCategory(todoCategories[0]);
    }
    public void AddButton()
    {
        if (TodoToggle.isOn == true) AddTodo();
        else AddCategory();
        TodoSaveManager.Instance.Save(todoCategories);
    }

    #region Todo
    public void AddTodo()
    {
        string todo = TodoInputField.text;

        if (todo == null || todo.Length == 0) return;

        GameObject todoContent = InstantiateBelow(TodoContentPrefab, TodoTextGameObject);

        TodoContent content = todoContent.GetComponent<TodoContent>();
        content.todo.Title = todo;
        content.todo.IsCompleted = false;
        content.todo.Item = todoContent;
        content.todo.SetToggle(false);

        TodoInputField.text = null;
    }
    public void ClickTodo(Todo todo)
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
            todo.SetToggle(todo.IsCompleted);
        }
        else if (currentProcess == TodoProcess.Delete)
        {
            DeleteTodo(todo);
        }
        else if (currentProcess == TodoProcess.Reset)
        {
            Debug.Log(currentProcess);
            ResetTodo(todo);
        }
        ChangeProcess("Add");
        TodoSaveManager.Instance.Save(todoCategories);
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
        var categoryIndex = GetCategoryIndex(activeCategoryId);
        if (categoryIndex >= 0)
        {
            todoCategories[categoryIndex].Todos.Remove(todo);
        }

        Destroy(todo.Item);
    }
    private void MoveTodo(Todo todo, GameObject target)
    {
        Debug.Log(target);
        GameObject obj = todo.Item;
        int targetIndex = target.transform.GetSiblingIndex();
        Debug.Log(targetIndex + 1);
        if (obj.transform.GetSiblingIndex() > targetIndex + 1)
            obj.transform.SetSiblingIndex(targetIndex + 1);
        else if (obj.transform.GetSiblingIndex() < targetIndex + 1)
            obj.transform.SetSiblingIndex(targetIndex);
    }
    private GameObject InstantiateBelow(GameObject prefab, GameObject target)
    {
        GameObject newObj = Instantiate(prefab, target.transform.parent);
        int targetIndex = target.transform.GetSiblingIndex();
        newObj.transform.SetSiblingIndex(targetIndex + 1);
        todoCategories[GetCategoryIndex(activeCategoryId)].Todos.Add(newObj.GetComponent<TodoContent>().todo);
        return newObj;
    }
    #endregion

    #region Category
    public void AddCategory()
    {
        Category newCategory = new Category();
        newCategory.Id = GetAvailableId();
        newCategory.Name = TodoInputField.text; ;
        newCategory.CategoryItem = Instantiate(CategoryPrefab, CategoryParent);
        newCategory.CategoryItem.GetComponent<TodoCategory>().category = newCategory;
        newCategory.CategoryScrollBar = Instantiate(CategoryScrollBarPrefab, CategoryScrollBarParent);
        newCategory.CategoryScrollBar.transform.SetSiblingIndex(1);
        todoCategories.Add(newCategory);
        TodoInputField.text = null;
    }
    public void ClickCategory(Category category)
    {
        if (currentProcess == TodoProcess.Add)
        {
            activeCategoryId = category.Id;
            ChangeScrollBar(category.Id);
            GameObject obj = todoCategories[GetCategoryIndex(activeCategoryId)].CategoryScrollBar;

            if (obj.transform.GetChild(0).GetChild(0).gameObject.name.Contains("Content"))
            {
                TodoTextGameObject = obj.transform.GetChild(0).GetChild(0).Find("TodoText").gameObject;
                TodoNowTextGameObject = obj.transform.GetChild(0).GetChild(0).Find("TodoNowText").gameObject;
                DoneTextGameObject = obj.transform.GetChild(0).GetChild(0).Find("DoneText").gameObject;
            }
            else Debug.Log("Bulamadı Content adında bişi");
        }
        else if (currentProcess == TodoProcess.Delete)
        {
            DeleteCategory(category);
        }
        TodoSaveManager.Instance.Save(todoCategories);
    }
    public void DeleteCategory(Category category)
    {
        if(todoCategories.Count == 1) return;

        todoCategories.Remove(category);
        Destroy(category.CategoryItem);
        Destroy(category.CategoryScrollBar);

        ChangeProcess("Add");

        ClickCategory(todoCategories[0]);
    }
    public void ChangeScrollBar(int Id)
    {
        for (int i = 0; i < todoCategories.Count; i++)
        {
            if (todoCategories[i].Id == Id)
            {
                todoCategories[i].CategoryScrollBar.SetActive(true);
            }
            else
            {
                todoCategories[i].CategoryScrollBar.SetActive(false);
            }
        }
    }
    private int GetCategoryIndex(int id)
    {
        for (int i = 0; i < todoCategories.Count; i++)
        {
            if (todoCategories[i].Id == id)
                return i;
        }
        return -1;
    }
    private int GetAvailableId()
    {
        HashSet<int> existingIds = new HashSet<int>();
        for (int i = 0; i < CategoryParent.childCount; i++)
        {
            TodoCategory content = CategoryParent.GetChild(i).GetComponent<TodoCategory>();
            if (content != null)
            {
                existingIds.Add(content.category.Id);
            }
        }
        while (true)
        {
            int id = Random.Range(0, 10000);
            if (!existingIds.Contains(id))
                return id;
        }
    }
    #endregion
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            TodoSaveManager.Instance?.Save(todoCategories);
        }
    }
    private void OnApplicationQuit()
    {
        TodoSaveManager.Instance?.Save(todoCategories);
    }
    public void LoadTodos()
    {
        var loadedCategories = TodoSaveManager.Instance.Load();
        if (loadedCategories == null) return;

        // Eski verileri temizle
        foreach (Category cat in todoCategories)
        {
            Destroy(cat.CategoryItem);
            Destroy(cat.CategoryScrollBar);
        }
        todoCategories.Clear();

        // Yeniden oluştur
        foreach (var sCat in loadedCategories)
        {
            TodoInputField.text = sCat.Name;
            AddCategory();
            int index = todoCategories.Count - 1;
            Category createdCat = todoCategories[index];
            createdCat.Id = sCat.Id;
            createdCat.Name = sCat.Name;

            ClickCategory(createdCat);

            TodoTextGameObject = createdCat.CategoryScrollBar.transform.GetChild(0).GetChild(0).transform.Find("TodoText").gameObject;
            TodoNowTextGameObject = createdCat.CategoryScrollBar.transform.GetChild(0).GetChild(0).transform.Find("TodoNowText").gameObject;
            DoneTextGameObject = createdCat.CategoryScrollBar.transform.GetChild(0).GetChild(0).transform.Find("DoneText").gameObject;

            foreach (var sTodo in sCat.Todos)
            {
                TodoInputField.text = sTodo.Title;
                AddTodo();
                Todo lastTodo = createdCat.Todos.Last();

                lastTodo.IsReceived = sTodo.IsReceived;
                lastTodo.IsCompleted = sTodo.IsCompleted;
                lastTodo.Item = lastTodo.Item ?? lastTodo.Item; // garanti

                // Doğru pozisyona taşı
                if (lastTodo.IsCompleted)
                {
                    MoveTodo(lastTodo, DoneTextGameObject);
                }
                else if (lastTodo.IsReceived)
                {
                    MoveTodo(lastTodo, TodoNowTextGameObject);
                }
                else
                {
                    MoveTodo(lastTodo, TodoTextGameObject);
                }
                lastTodo.SetToggle(lastTodo.IsCompleted);
            }
        }

        TodoInputField.text = null;
    }

}
[System.Serializable]
public class Todo
{
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
[System.Serializable]
public class Category
{
    public int Id;
    public string Name;
    public GameObject CategoryItem;
    public GameObject CategoryScrollBar;
    public List<Todo> Todos = new List<Todo>();
}
public enum TodoProcess
{
    Add,
    Delete,
    Reset
}
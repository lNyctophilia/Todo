using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class TodoManager : MonoBehaviour
{
    [Header("Category References")]
    [SerializeField] private GameObject CategoryPrefab;
    [SerializeField] private GameObject CategoryScrollBarPrefab;
    [SerializeField] private Transform CategoryParent;
    [SerializeField] private Transform CategoryScrollBarParent;
    [SerializeField] private InputField CategoryInputField;

    [Header("Todo References")]
    [SerializeField] private GameObject TodoContentPrefab;
    [SerializeField] private InputField TodoInputField;

    [Header("Data")]
    private List<Category> todoCategories = new List<Category>();
    [SerializeField] private int activeCategoryId;

    public static TodoManager Instance;

    private void Awake()
    {
        Instance = this;

        // Varsayılan kategori
        CategoryInputField.text = "Default";
        AddCategory();
        CategoryInputField.text = null;
    }

    private void Start()
    {
        LoadTodos();
        if (todoCategories.Count > 0)
            ClickCategory(todoCategories[0]);
    }

    public void AddButton()
    {
        AddTodo();
    }

    public void AddTodo()
    {
        TodoInputField.DeactivateInputField();
        string todoText = TodoInputField.text;
        if (todoText == null || todoText.Length <= 0) return;

        int categoryIndex = GetCategoryIndex(activeCategoryId);
        if (categoryIndex == -1) return;

        int scrollbarId = todoCategories[categoryIndex].ScrollBarId;
        GameObject scrollbarObj = GetScrollbarGameObject(scrollbarId);
        if (scrollbarObj == null) return;

        Transform contentParent = scrollbarObj.transform.GetChild(0).GetChild(0);

        GameObject newTodo = Instantiate(TodoContentPrefab, contentParent);
        TodoContent content = newTodo.GetComponent<TodoContent>();

        content.todo.Title = todoText;
        content.todo.IsCompleted = false;
        content.todo.Id = GetAvailableTodoId();

        todoCategories[categoryIndex].Todos.Add(content.todo);

        TodoInputField.text = null;
    }
/*
    public void ClickTodo(bool state, Todo todo)
    {
        //MoveTodo(state, todo);
    }
*/
    public void DeleteTodo(Todo todo)
    {
        var categoryIndex = GetCategoryIndex(activeCategoryId);
        if (categoryIndex >= 0)
        {
            todoCategories[categoryIndex].Todos.Remove(todo);
        }

        var obj = GetTodoGameObject(todo.Id);
        if (obj != null)
            Destroy(obj);
    }
/*
    private void MoveTodo(bool state, Todo todo)
    {
        var todoObj = GetTodoGameObject(todo.Id);
        if (todoObj == null) return;

        if (state)
            todoObj.transform.SetSiblingIndex(todoObj.transform.parent.childCount - 1);
        else
            todoObj.transform.SetSiblingIndex(0);
    }
*/
    private GameObject GetTodoGameObject(int Id)
    {
        int categoryIndex = GetCategoryIndex(activeCategoryId);
        if (categoryIndex == -1) return null;

        int scrollbarId = todoCategories[categoryIndex].ScrollBarId;
        GameObject scrollbarObj = GetScrollbarGameObject(scrollbarId);
        if (scrollbarObj == null) return null;

        Transform contentParent = scrollbarObj.transform.GetChild(0).GetChild(0);
        foreach (Transform child in contentParent)
        {
            TodoContent todoContent = child.GetComponent<TodoContent>();
            if (todoContent != null && todoContent.todo.Id == Id)
                return child.gameObject;
        }
        return null;
    }

    public void AddCategory()
    {
        TodoInputField.DeactivateInputField();
        string categoryText = CategoryInputField.text;
        if (categoryText == null || categoryText.Length <= 0) return;

        int availableId = GetAvailableCategoryId();
        Category newCategory = new Category
        {
            Id = availableId,
            ScrollBarId = availableId,
            Title = categoryText
        };

        GameObject CategoryItem = Instantiate(CategoryPrefab, CategoryParent);
        CategoryItem.GetComponent<TodoCategory>().category = newCategory;
        CategoryItem.GetComponent<TodoCategory>().Setup(newCategory);

        GameObject CategoryScrollBar = Instantiate(CategoryScrollBarPrefab, CategoryScrollBarParent);
        CategoryScrollBar.transform.SetSiblingIndex(1);
        CategoryScrollBar.GetComponent<TodoScrollbar>().scrollbar = new Scrollbar { Id = newCategory.ScrollBarId };

        todoCategories.Add(newCategory);

        CategoryItem.transform.SetSiblingIndex(0);
        CategoryInputField.text = null;

        ClickCategory(newCategory);
    }

    public void ClickCategory(Category category)
    {
        activeCategoryId = category.Id;
        ChangeScrollBar(category.ScrollBarId);
    }

    public void DeleteCategory(Category category)
    {
        if (todoCategories.Count == 1) return;

        todoCategories.Remove(category);
        Destroy(GetCategoryGameObject(category.Id));
        Destroy(GetScrollbarGameObject(category.ScrollBarId));

        ClickCategory(todoCategories[0]);
    }

    public void ChangeScrollBar(int Id)
    {
        for (int i = 0; i < CategoryScrollBarParent.transform.childCount; i++)
        {
            var obj = CategoryScrollBarParent.transform.GetChild(i).gameObject;
            var scrollBar = obj.GetComponent<TodoScrollbar>();
            if (scrollBar != null && scrollBar.scrollbar.Id == Id)
            {
                // Sadece seçilen açık olsun
                obj.SetActive(true);
            }
            else if (scrollBar != null && scrollBar.scrollbar.Id != Id)
            {
                obj.SetActive(false);
            }
        }
    }

    private int GetCategoryIndex(int id)
    {
        for (int i = 0; i < todoCategories.Count; i++)
            if (todoCategories[i].Id == id)
                return i;
        return -1;
    }

    private int GetAvailableCategoryId()
    {
        HashSet<int> existingIds = new HashSet<int>(todoCategories.Select(c => c.Id).Concat(todoCategories.Select(c => c.ScrollBarId)));
        while (true)
        {
            int id = Random.Range(0, 10000);
            if (!existingIds.Contains(id))
                return id;
        }
    }

    private int GetAvailableTodoId()
    {
        HashSet<int> existingIds = new HashSet<int>(todoCategories.SelectMany(c => c.Todos).Select(t => t.Id));
        while (true)
        {
            int id = Random.Range(0, 100000);
            if (!existingIds.Contains(id))
                return id;
        }
    }

    private GameObject GetCategoryGameObject(int id)
    {
        for (int i = 0; i < CategoryParent.childCount; i++)
        {
            TodoCategory content = CategoryParent.GetChild(i).GetComponent<TodoCategory>();
            if (content != null && content.category.Id == id)
                return content.gameObject;
        }
        return null;
    }

    private GameObject GetScrollbarGameObject(int id)
    {
        for (int i = 0; i < CategoryScrollBarParent.childCount; i++)
        {
            TodoScrollbar content = CategoryScrollBarParent.GetChild(i).GetComponent<TodoScrollbar>();
            if (content != null && content.scrollbar.Id == id)
                return CategoryScrollBarParent.GetChild(i).gameObject;
        }
        return null;
    }
    public void AddCategoryFromSave(SerializableCategory sCat)
    {
        Category newCategory = new Category
        {
            Id = sCat.Id,
            ScrollBarId = sCat.ScrollbarId,
            Title = sCat.Title
        };

        GameObject CategoryItem = Instantiate(CategoryPrefab, CategoryParent);
        CategoryItem.GetComponent<TodoCategory>().category = newCategory;
        CategoryItem.GetComponent<TodoCategory>().Setup(newCategory);

        GameObject CategoryScrollBar = Instantiate(CategoryScrollBarPrefab, CategoryScrollBarParent);
        CategoryScrollBar.transform.SetSiblingIndex(1);
        CategoryScrollBar.GetComponent<TodoScrollbar>().scrollbar = new Scrollbar { Id = newCategory.ScrollBarId };

        todoCategories.Add(newCategory);
        CategoryItem.transform.SetSiblingIndex(0);

        // Bu kategoriye ait todo'ları ekle
        activeCategoryId = newCategory.Id;
        foreach (var sTodo in sCat.Todos)
        {
            GameObject scrollbarObj = GetScrollbarGameObject(newCategory.ScrollBarId);
            if (scrollbarObj == null) continue;

            Transform contentParent = scrollbarObj.transform.GetChild(0).GetChild(0);
            GameObject newTodo = Instantiate(TodoContentPrefab, contentParent);
            TodoContent content = newTodo.GetComponent<TodoContent>();

            content.todo.Title = sTodo.Title;
            content.todo.IsCompleted = sTodo.IsCompleted;
            content.todo.Id = sTodo.Id;

            newCategory.Todos.Add(content.todo);
        }
    }
    public void LoadTodos()
    {
        var loadedCategories = TodoSaveManager.Instance.Load();
        if (loadedCategories == null) return;

        // Eski verileri temizle
        foreach (Category cat in todoCategories)
        {
            Destroy(GetCategoryGameObject(cat.Id));
            Destroy(GetScrollbarGameObject(cat.ScrollBarId));
        }
        todoCategories.Clear();

        // Yeniden oluştur
        foreach (var sCat in loadedCategories)
        {
            AddCategoryFromSave(sCat);
        }

        // İlk kategoriyi aktif yap
        if (todoCategories.Count > 0)
        {
            ClickCategory(todoCategories[0]);
        }

        TodoInputField.text = null;
    }
    private void OnApplicationPause(bool pause)
    {
        if (pause)
            TodoSaveManager.Instance?.Save(todoCategories);
    }

    private void OnApplicationQuit()
    {
        TodoSaveManager.Instance?.Save(todoCategories);
    }
}

[System.Serializable]
public class Todo
{
    public string Title;
    public int Id;
    public bool IsCompleted;
}

[System.Serializable]
public class Category
{
    public string Title;
    public int Id;
    public int ScrollBarId;
    public List<Todo> Todos = new List<Todo>();
}

[System.Serializable]
public class Scrollbar
{
    public int Id;
}

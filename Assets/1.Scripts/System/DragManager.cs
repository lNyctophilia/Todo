using UnityEngine;
using UnityEngine.UI;

public class DragManager : MonoBehaviour
{
    [Header("Hold Settings")]
    public float holdDuration = 1f; // 1 saniye bekleme
    public float dragThreshold = 10f; // Drag başlaması için max hareket (piksel)
    public float draggingOffset = 0.15f;

    public GameObject AddCategoryButton;

    private float holdTimer = 0f;
    private bool isHolding = false;
    private bool isDragging = false;

    private GameObject currentItem;
    private RectTransform itemRect;
    private Canvas parentCanvas;
    private ScrollRect parentScrollRect;
    private Transform originalParent;

    private bool verticalOnly = false;
    private bool horizontalOnly = false;

    private Vector2 dragOffset; 
    private Vector2 limitedTarget;

    private Vector2 pointerDownPos; // Basıldığı andaki mouse pozisyonu

    void Update()
    {
        // Mouse tıklandı
        if (Input.GetMouseButtonDown(0))
        {
            var results = new System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult>();
            var pointerData = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);
            pointerData.position = Input.mousePosition;
            UnityEngine.EventSystems.EventSystem.current.RaycastAll(pointerData, results);

            foreach (var result in results)
            {
                if (result.gameObject.GetComponent<TodoContent>() != null || result.gameObject.GetComponent<TodoCategory>() != null || result.gameObject.GetComponent<StreakContent>() != null)
                {
                    currentItem = result.gameObject;
                    itemRect = currentItem.GetComponent<RectTransform>();
                    parentCanvas = currentItem.GetComponentInParent<Canvas>();
                    parentScrollRect = currentItem.GetComponentInParent<ScrollRect>();
                    originalParent = currentItem.transform.parent;

                    holdTimer = 0f;
                    isHolding = true;
                    pointerDownPos = Input.mousePosition; // basıldığı anda pozisyonu al

                    break;
                }
            }
        }

        // Basılı tutma kontrolü
        if (Input.GetMouseButton(0) && isHolding && !isDragging)
        {
            holdTimer += Time.deltaTime;

            float distance = Vector2.Distance(Input.mousePosition, pointerDownPos);

            if (holdTimer >= holdDuration && distance <= dragThreshold)
            {
                StartDrag();
            }
        }

        // Mouse bırakıldı
        if (Input.GetMouseButtonUp(0))
        {
            if (isDragging)
                EndDrag();

            isHolding = false;
            holdTimer = 0f;
        }

        if (isDragging && currentItem != null)
        {
            Vector3 worldMousePos = parentCanvas.worldCamera != null
                ? parentCanvas.worldCamera.ScreenToWorldPoint(Input.mousePosition)
                : (Vector3)Input.mousePosition;
            worldMousePos.z = itemRect.position.z;

            Vector3 targetPos = worldMousePos + (Vector3)dragOffset;

            // Kısıtlama
            if (verticalOnly) targetPos.x = limitedTarget.x;
            if (horizontalOnly) targetPos.y = limitedTarget.y;

            itemRect.position = Vector3.Lerp(itemRect.position, targetPos, 0.3f);
        }
    }

    private void StartDrag()
    {
        if (currentItem == null) return;

        if (currentItem.GetComponent<TodoContent>() != null || currentItem.GetComponent<StreakContent>() != null)
        {
            verticalOnly = true;
            horizontalOnly = false;
        }
        else if (currentItem.GetComponent<TodoCategory>() != null)
        {
            verticalOnly = false;
            horizontalOnly = true;
        }
        else
        {
            isHolding = false;
            return;
        }

        isDragging = true;
        isHolding = false;

        if (parentScrollRect != null)
            parentScrollRect.enabled = false;

        // Drag sırasında ani sıçramayı önlemek için
        Vector3 worldMousePos = parentCanvas.worldCamera != null
            ? parentCanvas.worldCamera.ScreenToWorldPoint(Input.mousePosition)
            : (Vector3)Input.mousePosition;
        worldMousePos.z = itemRect.position.z; 

        dragOffset = itemRect.position - worldMousePos;
        limitedTarget = new Vector2(itemRect.position.x - draggingOffset, itemRect.position.y + draggingOffset);

        // Canvas üstüne taşı
        itemRect.SetParent(parentCanvas.transform, true);
    }
private void EndDrag()
{
    if (currentItem == null) return;

    // Orijinal parent’a geri dön
    itemRect.SetParent(originalParent, false);
    RectTransform parentRT = originalParent as RectTransform;

    // Mouse pozisyonunu parent (content) lokal uzayına çevir
    Vector2 localMouse;
    RectTransformUtility.ScreenPointToLocalPointInRectangle(
        parentRT,
        Input.mousePosition,
        parentCanvas ? parentCanvas.worldCamera : null,
        out localMouse
    );

    int closestIndex = -1;
    float closestDist = float.MaxValue;

    for (int i = 0; i < originalParent.childCount; i++)
    {
        Transform childT = originalParent.GetChild(i);

        // Kendini ve "Add" butonunu atla
        if (childT == itemRect) continue;
        if (AddCategoryButton != null && childT == AddCategoryButton.transform) continue;

        RectTransform childRT = childT as RectTransform;
        Vector2 childLocal = childRT.anchoredPosition;

        float distance = verticalOnly
            ? Mathf.Abs(localMouse.y - childLocal.y)   // dikey
            : Mathf.Abs(localMouse.x - childLocal.x); // yatay

        if (distance < closestDist)
        {
            closestDist = distance;
            closestIndex = i;
        }
    }

    int newIndex;
    if (closestIndex >= 0)
    {
        RectTransform closestChildRT = originalParent.GetChild(closestIndex) as RectTransform;
        float closestPos = verticalOnly ? closestChildRT.anchoredPosition.y : closestChildRT.anchoredPosition.x;
        float mousePosValue = verticalOnly ? localMouse.y : localMouse.x;

        if (verticalOnly)
        {
            // Mouse üstteyse üstüne, alttaysa altına
            newIndex = mousePosValue > closestPos ? closestIndex : closestIndex + 1;
        }
        else
        {
            // Horizontal: soldaysa önüne, sağdaysa sonrasına
            newIndex = mousePosValue < closestPos ? closestIndex : closestIndex + 1;
        }

        // childCount aşımını önle
        newIndex = Mathf.Clamp(newIndex, 0, originalParent.childCount - 1);
    }
    else
    {
        newIndex = originalParent.childCount - 1; // Bulunamadıysa en sona
    }

    itemRect.SetSiblingIndex(newIndex);

    // Order güncelle
    var todoContent = currentItem.GetComponent<TodoContent>();
    var todoCategory = currentItem.GetComponent<TodoCategory>();
    var streakContent = currentItem.GetComponent<StreakContent>();

    if (todoContent != null)
        TodoManager.Instance.UpdateTodoOrder(itemRect.parent);
    else if (todoCategory != null)
        TodoManager.Instance.UpdateCategoryOrder(itemRect.parent);
    else if (streakContent != null)
        StreakSaveManager.Instance?.Save();

    if (parentScrollRect != null)
            parentScrollRect.enabled = true;

    // Temizlik
    isDragging = false;
    currentItem = null;
    itemRect = null;
    originalParent = null;

    if (AddCategoryButton != null)
        AddCategoryButton.transform.SetAsLastSibling();
}

}
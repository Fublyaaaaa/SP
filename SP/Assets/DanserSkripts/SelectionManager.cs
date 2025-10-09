using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class SelectionManager : MonoBehaviour
{
    [Header("Selection Settings")]
    [SerializeField] private LayerMask dancerLayerMask = 1;
    
    [Header("Grid Settings")]
    [SerializeField] private bool snapToGrid = true;
    [SerializeField] private float gridSize = 1.0f;
    
    private List<DancerController> selectedDancers = new List<DancerController>();
    private Camera mainCamera;
    private Vector3 dragStartWorldPos;
    private bool isDragging = false;
    private bool multiSelectPressed = false;

    // Input Actions
    private PlayerInput playerInput;
    private InputAction leftClickAction;
    private InputAction multiSelectAction;
    private InputAction deleteAction;
    private InputAction spawnAction;

    public IReadOnlyList<DancerController> SelectedDancers => selectedDancers;

    private void Awake()
    {
        mainCamera = Camera.main;
        SetupInputSystem();
        Debug.Log("SelectionManager initialized with Input System");
    }

    private void SetupInputSystem()
    {
        // Создаем Input Actions
        playerInput = GetComponent<PlayerInput>();
        if (playerInput == null)
        {
            playerInput = gameObject.AddComponent<PlayerInput>();
        }

        // Настраиваем действия
        leftClickAction = new InputAction("LeftClick", InputActionType.Button, "<Mouse>/leftButton");
        multiSelectAction = new InputAction("MultiSelect", InputActionType.Button, "<Keyboard>/leftShift");
        deleteAction = new InputAction("Delete", InputActionType.Button, "<Keyboard>/delete");
        spawnAction = new InputAction("Spawn", InputActionType.Button, "<Keyboard>/space");

        // Подписываемся на события
        leftClickAction.started += OnLeftClickStarted;
        leftClickAction.canceled += OnLeftClickCanceled;
        multiSelectAction.started += OnMultiSelectStarted;
        multiSelectAction.canceled += OnMultiSelectCanceled;
        deleteAction.started += OnDeleteStarted;
        spawnAction.started += OnSpawnStarted;

        // Включаем действия
        leftClickAction.Enable();
        multiSelectAction.Enable();
        deleteAction.Enable();
        spawnAction.Enable();
    }

    private void OnDestroy()
    {
        // Отписываемся от событий
        leftClickAction?.Dispose();
        multiSelectAction?.Dispose();
        deleteAction?.Dispose();
        spawnAction?.Dispose();
    }

    private void OnLeftClickStarted(InputAction.CallbackContext context)
    {
        if (!IsPointerOverUI())
        {
            StartSelection();
        }
    }

    private void OnLeftClickCanceled(InputAction.CallbackContext context)
    {
        EndSelection();
    }

    private void OnMultiSelectStarted(InputAction.CallbackContext context)
    {
        multiSelectPressed = true;
    }

    private void OnMultiSelectCanceled(InputAction.CallbackContext context)
    {
        multiSelectPressed = false;
    }

    private void OnDeleteStarted(InputAction.CallbackContext context)
    {
        DeleteSelectedDancers();
    }

    private void OnSpawnStarted(InputAction.CallbackContext context)
    {
        SpawnDancerAtMousePosition();
    }

    private void Update()
    {
        // Обрабатываем перетаскивание
        if (isDragging && selectedDancers.Count > 0)
        {
            HandleDragging();
        }
    }

    private void StartSelection()
    {
        dragStartWorldPos = GetMouseWorldPos();

        // Проверяем клик по танцору
        DancerController clickedDancer = GetDancerAtMousePosition();
        
        if (clickedDancer != null)
        {
            HandleDancerSelection(clickedDancer);
        }
        else
        {
            // Клик по пустому месту
            if (!multiSelectPressed)
            {
                ClearSelection();
            }
        }
    }

    private void HandleDragging()
    {
        Vector3 currentMousePos = GetMouseWorldPos();
        Vector3 dragDelta = currentMousePos - dragStartWorldPos;

        // Перемещаем всех выделенных танцоров
        foreach (var dancer in selectedDancers)
        {
            Vector3 newPosition = dancer.transform.position + dragDelta;
            dancer.MoveToPosition(newPosition, snapToGrid);
        }
        
        dragStartWorldPos = currentMousePos;
    }

    private void EndSelection()
    {
        isDragging = false;
        
        // Применяем snap to grid после перемещения
        if (snapToGrid)
        {
            SnapSelectedToGrid();
        }
    }

    private void HandleDancerSelection(DancerController dancer)
    {
        if (!multiSelectPressed && !selectedDancers.Contains(dancer))
        {
            ClearSelection();
        }

        if (selectedDancers.Contains(dancer))
        {
            // Если уже выделен и зажат multiselect - убираем из выделения
            if (multiSelectPressed)
            {
                RemoveFromSelection(dancer);
            }
        }
        else
        {
            // Добавляем в выделение
            AddToSelection(dancer);
        }

        isDragging = true;
    }

    // Публичный метод для вызова из DancerController
    public void HandleDancerClicked(DancerController dancer)
    {
        HandleDancerSelection(dancer);
    }

    private void AddToSelection(DancerController dancer)
    {
        if (!selectedDancers.Contains(dancer))
        {
            selectedDancers.Add(dancer);
            dancer.SetSelected(true, multiSelectPressed);
            Debug.Log($"Added to selection: {dancer.Data.DancerName}");
        }
    }

    private void RemoveFromSelection(DancerController dancer)
    {
        if (selectedDancers.Contains(dancer))
        {
            selectedDancers.Remove(dancer);
            dancer.SetSelected(false);
            Debug.Log($"Removed from selection: {dancer.Data.DancerName}");
        }
    }

    private void ClearSelection()
    {
        foreach (var dancer in selectedDancers)
        {
            dancer.SetSelected(false);
        }
        selectedDancers.Clear();
        Debug.Log("Selection cleared");
    }

    private void SnapSelectedToGrid()
    {
        foreach (var dancer in selectedDancers)
        {
            Vector3 snappedPosition = dancer.transform.position;
            snappedPosition.x = Mathf.Round(snappedPosition.x / gridSize) * gridSize;
            snappedPosition.y = Mathf.Round(snappedPosition.y / gridSize) * gridSize;
            dancer.transform.position = snappedPosition;
        }
    }

    public void DeleteSelectedDancers()
    {
        if (selectedDancers.Count == 0) return;

        foreach (var dancer in selectedDancers)
        {
            Destroy(dancer.gameObject);
        }
        
        selectedDancers.Clear();
        Debug.Log("Deleted selected dancers");
    }

    public void SpawnDancerAtMousePosition()
    {
        Vector3 spawnPos = GetMouseWorldPos();
        if (snapToGrid)
        {
            spawnPos.x = Mathf.Round(spawnPos.x / gridSize) * gridSize;
            spawnPos.y = Mathf.Round(spawnPos.y / gridSize) * gridSize;
        }
        
        SpawnDancerAt(spawnPos);
    }

    public void SpawnDancerAt(Vector3 position)
    {
        GameObject dancerPrefab = Resources.Load<GameObject>("Prefabs/Dancer");
        if (dancerPrefab != null)
        {
            Instantiate(dancerPrefab, position, Quaternion.identity);
            Debug.Log($"Spawned dancer at {position}");
        }
        else
        {
            Debug.LogError("Dancer prefab not found in Resources/Prefabs/");
            CreateTemporaryDancer(position);
        }
    }

    private void CreateTemporaryDancer(Vector3 position)
    {
        GameObject tempDancer = new GameObject("TempDancer");
        tempDancer.transform.position = position;
        
        tempDancer.AddComponent<DancerData>();
        tempDancer.AddComponent<DancerController>();
        
        GameObject spriteObject = new GameObject("Sprite");
        spriteObject.transform.SetParent(tempDancer.transform);
        spriteObject.transform.localPosition = Vector3.zero;
        
        SpriteRenderer sr = spriteObject.AddComponent<SpriteRenderer>();
        sr.sprite = UnityEngine.Sprite.Create(
            new Texture2D(1, 1), 
            new Rect(0, 0, 1, 1), 
            new Vector2(0.5f, 0.5f)
        );
        sr.color = Color.blue;
        
        BoxCollider2D collider = tempDancer.AddComponent<BoxCollider2D>();
        collider.size = Vector2.one;
    }

    private DancerController GetDancerAtMousePosition()
    {
        Vector2 mousePos = GetMouseWorldPos();
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, dancerLayerMask);
        
        return hit.collider?.GetComponent<DancerController>();
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        mousePos.z = -mainCamera.transform.position.z;
        return mainCamera.ScreenToWorldPoint(mousePos);
    }

    private bool IsPointerOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }

    // Публичные методы для UI кнопок
    public void AlignSelectedToCenter()
    {
        if (selectedDancers.Count < 2) return;

        Vector3 center = Vector3.zero;
        foreach (var dancer in selectedDancers)
        {
            center += dancer.transform.position;
        }
        center /= selectedDancers.Count;

        foreach (var dancer in selectedDancers)
        {
            dancer.MoveToPosition(center, snapToGrid);
        }
    }

    // Для отладки
    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 150));
        GUILayout.Label($"SELECTION MANAGER DEBUG");
        GUILayout.Label($"Selected Dancers: {selectedDancers.Count}");
        GUILayout.Label($"Grid Snap: {snapToGrid}");
        GUILayout.Label($"Multi-Select: {multiSelectPressed}");
        
        if (selectedDancers.Count > 0)
        {
            GUILayout.Label($"First Selected: {selectedDancers[0].Data.DancerName}");
            GUILayout.Label($"Position: {selectedDancers[0].transform.position}");
        }
        
        GUILayout.EndArea();
    }
}
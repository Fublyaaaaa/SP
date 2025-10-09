using UnityEngine;

public class DancerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DancerData data;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Collider2D selectionCollider;

    [Header("Selection Visual")]
    [SerializeField] private Color selectedColor = Color.yellow;
    [SerializeField] private float selectedScale = 1.1f;
    
    private Color originalColor;
    private Vector3 originalScale;
    private bool isInitialized = false;

    public DancerData Data => data;
    public bool IsSelected { get; private set; }

    private void Awake()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        if (isInitialized) return;

        if (data == null) data = GetComponent<DancerData>();
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (selectionCollider == null) selectionCollider = GetComponent<Collider2D>();

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        
        originalScale = transform.localScale;
        isInitialized = true;
    }

    private void Start()
    {
        UpdateVisuals();
    }

    public void SetSelected(bool selected, bool isMultiSelect = false)
    {
        if (!isInitialized) InitializeComponents();

        IsSelected = selected;
        UpdateSelectionVisuals();

        if (selected)
        {
            Debug.Log($"Selected dancer: {data.DancerName}");
        }
        else if (!isMultiSelect)
        {
            Debug.Log($"Deselected dancer: {data.DancerName}");
        }
    }

    private void UpdateSelectionVisuals()
    {
        if (spriteRenderer == null) return;

        if (IsSelected)
        {
            spriteRenderer.color = selectedColor;
            transform.localScale = originalScale * selectedScale;
        }
        else
        {
            spriteRenderer.color = originalColor;
            transform.localScale = originalScale;
        }
    }

    public void UpdateVisuals()
    {
        if (!isInitialized) InitializeComponents();

        if (spriteRenderer != null)
        {
            originalColor = data.Color;
            if (!IsSelected)
            {
                spriteRenderer.color = originalColor;
            }
        }
    }

    public void MoveToPosition(Vector3 newPosition, bool snapToGrid = true)
    {
        if (snapToGrid)
        {
            newPosition.x = Mathf.Round(newPosition.x);
            newPosition.y = Mathf.Round(newPosition.y);
        }
        
        transform.position = newPosition;
    }

    public void ChangeColor(Color newColor)
    {
        if (data != null)
        {
            data.Color = newColor;
            UpdateVisuals();
        }
    }

    // Вызывается при клике на танцора
    private void OnMouseDown()
    {
        SelectionManager selectionManager = FindFirstObjectByType<SelectionManager>();
        if (selectionManager != null)
        {
            selectionManager.HandleDancerClicked(this);
        }
    }

    // Для отладки
    private void OnMouseEnter()
    {
        if (!IsSelected && spriteRenderer != null)
        {
            spriteRenderer.color = Color.Lerp(originalColor, Color.white, 0.3f);
        }
    }

    private void OnMouseExit()
    {
        if (!IsSelected && spriteRenderer != null)
        {
            spriteRenderer.color = IsSelected ? selectedColor : originalColor;
        }
    }

    private void OnValidate()
    {
        if (Application.isPlaying && isInitialized)
        {
            UpdateVisuals();
        }
    }
}
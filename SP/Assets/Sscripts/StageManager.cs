using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField] private GridRenderer gridRenderer;

    [Header("Stage size (в инспекторе)")]
    [SerializeField, Tooltip("Ширина сцены в клетках")] private int width = 16;
    [SerializeField, Tooltip("Высота сцены в клетках")] private int height = 9;
    [Header("Привязка сетки к сцене")]
    [SerializeField, Tooltip("Корневой объект сцены (тот, который должен масштабироваться)")] private Transform stageRoot;
    public enum StageAnchor { Center, BottomLeft }
    [SerializeField] private StageAnchor stageAnchor = StageAnchor.Center;
    [SerializeField, Tooltip("Размер клетки в единицах Unity (1 = 1 unit)")] private Vector2 cellSize = Vector2.one;

    private StageData stageData;

    private void Reset()
    {
        // Попытка получить StageData, если он прикреплён на том же объекте
        stageData = GetComponent<StageData>();
        if (stageData != null)
        {
            width = Mathf.Max(1, stageData.Width);
            height = Mathf.Max(1, stageData.Height);
        }
    }

    private void Start()
    {
        // Если StageData присутствует — синхронизируем с ним, иначе создаём временный представление
        stageData = GetComponent<StageData>();
        if (stageData == null)
        {
            stageData = new GameObject("_StageData_temp").AddComponent<StageData>();
            stageData.transform.SetParent(transform);
            stageData.Width = width;
            stageData.Height = height;
        }

        // Убедимся в корректных значениях
        width = Mathf.Clamp(width, 1, 1024);
        height = Mathf.Clamp(height, 1, 1024);
        stageData.Width = width;
        stageData.Height = height;

        if (gridRenderer != null)
            gridRenderer.DrawGrid(width, height);

        // Применяем размер к корню сцены
        ApplyStageSizeToRoot();
    }

    // Вызывается в редакторе при изменении сериализуемых полей
    private void OnValidate()
    {
        // Защита от отрицательных или нулевых размеров
        width = Mathf.Max(1, width);
        height = Mathf.Max(1, height);

        // Обновляем stageData, если он есть
        if (stageData == null)
            stageData = GetComponent<StageData>();

        if (stageData != null)
        {
            stageData.Width = width;
            stageData.Height = height;
        }

        if (gridRenderer != null)
        {
            // В редакторе DrawGrid можно вызывать напрямую — GridRenderer очищает старые линии
            gridRenderer.DrawGrid(width, height);
        }
        ApplyStageSizeToRoot();
    }

    // Публичный метод для изменения размера в рантайме
    public void ResizeStage(int newWidth, int newHeight)
    {
        newWidth = Mathf.Max(1, newWidth);
        newHeight = Mathf.Max(1, newHeight);

        width = newWidth;
        height = newHeight;

        if (stageData == null)
            stageData = GetComponent<StageData>();

        if (stageData != null)
        {
            stageData.Width = width;
            stageData.Height = height;
        }

        if (gridRenderer != null)
            gridRenderer.DrawGrid(width, height);

    ApplyStageSizeToRoot();
    }

    // Пара свойств для доступа из других скриптов
    public int Width => width;
    public int Height => height;

    private void ApplyStageSizeToRoot()
    {
        if (stageRoot == null)
            return;

        // Размер в единицах Unity
        float totalWidth = width * cellSize.x;
        float totalHeight = height * cellSize.y;

        // 1) RectTransform (UI) — изменяем sizeDelta
        var rect = stageRoot.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.sizeDelta = new Vector2(totalWidth, totalHeight);
            // выравнивание якоря
            if (stageAnchor == StageAnchor.Center)
                rect.anchoredPosition = Vector2.zero;
            else // BottomLeft
                rect.anchoredPosition = new Vector2(totalWidth / 2f, totalHeight / 2f);
            return;
        }

        // 2) Tilemap или SpriteRenderer — для простоты будем менять scale и позицию
        var sprite = stageRoot.GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            // Предположим, что sprite пиксели = 1 unit и scale определяет размер
            Vector3 newScale = new Vector3(totalWidth / Mathf.Max(1f, sprite.bounds.size.x), 1f, totalHeight / Mathf.Max(1f, sprite.bounds.size.y));
            stageRoot.localScale = new Vector3(newScale.x, newScale.z, 1f);
            // Выравнивание
            if (stageAnchor == StageAnchor.Center)
                stageRoot.localPosition = Vector3.zero;
            else
                stageRoot.localPosition = new Vector3(totalWidth / 2f, totalHeight / 2f, 0f);
            return;
        }

        // 3) Tilemap компонент
        var tilemap = stageRoot.GetComponent<UnityEngine.Tilemaps.Tilemap>();
        if (tilemap != null)
        {
            // Меняем localScale по cellSize
            stageRoot.localScale = new Vector3(cellSize.x, cellSize.y, 1f);
            // Позиция — в зависимости от anchor
            if (stageAnchor == StageAnchor.Center)
                stageRoot.localPosition = new Vector3(-totalWidth / 2f + cellSize.x / 2f, -totalHeight / 2f + cellSize.y / 2f, 0f);
            else
                stageRoot.localPosition = Vector3.zero;
            // При необходимости можно очистить/переставить тайлы, но это уже задача для Tilemap API
            return;
        }

        // 4) Fallback: просто выставляем scale и позицию
        stageRoot.localScale = new Vector3(totalWidth, totalHeight, 1f);
        if (stageAnchor == StageAnchor.Center)
            stageRoot.localPosition = Vector3.zero;
        else
            stageRoot.localPosition = new Vector3(totalWidth / 2f, totalHeight / 2f, 0f);
    }
}

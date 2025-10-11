using UnityEngine;

public class GridRenderer : MonoBehaviour
{
    [SerializeField] private Material lineMaterial;
    [SerializeField] private float lineWidth = 0.05f;
    private bool needsRedraw;

    public void DrawGrid(int width, int height)
    {
        // Если в режиме редактирования — просто помечаем, что нужно обновить
        if (!Application.isPlaying)
        {
            needsRedraw = true;
            return;
        }

        ClearGrid();

        // Смещение, чтобы сетка была по центру
        Vector3 offset = new Vector3(-width / 2f, -height / 2f, 0);

        // Вертикальные линии
        for (int x = 0; x <= width; x++)
        {
            CreateLine($"VLine{x}", new Vector3(x, 0, 0) + offset, new Vector3(x, height, 0) + offset);
        }

        // Горизонтальные линии
        for (int y = 0; y <= height; y++)
        {
            CreateLine($"HLine{y}", new Vector3(0, y, 0) + offset, new Vector3(width, y, 0) + offset);
        }
    }

    private void CreateLine(string name, Vector3 start, Vector3 end)
    {
        GameObject line = new GameObject(name);
        line.transform.parent = transform;

        LineRenderer lr = line.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.useWorldSpace = false;
        lr.material = lineMaterial != null ? lineMaterial : new Material(Shader.Find("Sprites/Default"));
        lr.startColor = Color.white;
        lr.endColor = Color.white;
    }

    private void ClearGrid()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // Если нужно обновить в редакторе — перерисовываем сетку безопасно
        if (needsRedraw)
        {
            needsRedraw = false;
            ClearEditorGrid();
            DrawEditorGrid();
        }
    }

    private void ClearEditorGrid()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            if (Application.isEditor)
                DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

    private void DrawEditorGrid()
    {
        // Получаем StageData из родителя
        StageData stage = GetComponentInParent<StageData>();
        if (stage != null)
            DrawGrid(stage.Width, stage.Height);
    }
#endif
}
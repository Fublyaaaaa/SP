using UnityEngine;

public class GridRenderer : MonoBehaviour
{
    [SerializeField] private Material lineMaterial;  // 🎨 Материал линий
    [SerializeField] private float lineWidth = 0.05f;

    public void DrawGrid(int width, int height)
    {
        // Очистка старых линий
        if (!Application.isPlaying && UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.LogWarning("GridRenderer: Нельзя удалять объекты в OnValidate. Очистка пропущена.");
            return;
        }
        foreach (Transform child in transform)
        {
            if (Application.isPlaying)
                Destroy(child.gameObject);
            else
                DestroyImmediate(child.gameObject);
        }

        // Смещение (чтобы сетка была по центру)
        Vector3 offset = new Vector3(-width / 2f, -height / 2f, 0);

        // Вертикальные линии
        for (int x = 0; x <= width; x++)
        {
            GameObject line = new GameObject("VLine" + x);
            line.transform.parent = transform;
            LineRenderer lr = line.AddComponent<LineRenderer>();

            lr.positionCount = 2;
            lr.SetPosition(0, new Vector3(x, 0, 0) + offset);
            lr.SetPosition(1, new Vector3(x, height, 0) + offset);
            lr.startWidth = lineWidth;
            lr.endWidth = lineWidth;
            lr.useWorldSpace = false;
            lr.material = lineMaterial != null ? lineMaterial : new Material(Shader.Find("Sprites/Default"));
            lr.startColor = Color.white;
            lr.endColor = Color.white;
        }

        // Горизонтальные линии
        for (int y = 0; y <= height; y++)
        {
            GameObject line = new GameObject("HLine" + y);
            line.transform.parent = transform;
            LineRenderer lr = line.AddComponent<LineRenderer>();

            lr.positionCount = 2;
            lr.SetPosition(0, new Vector3(0, y, 0) + offset);
            lr.SetPosition(1, new Vector3(width, y, 0) + offset);
            lr.startWidth = lineWidth;
            lr.endWidth = lineWidth;
            lr.useWorldSpace = false;
            lr.material = lineMaterial != null ? lineMaterial : new Material(Shader.Find("Sprites/Default"));
            lr.startColor = Color.white;
            lr.endColor = Color.white;
        }
    }
}
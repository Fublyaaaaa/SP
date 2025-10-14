
using UnityEngine;
[ExecuteAlways]
public class StageController : MonoBehaviour
{
    [Tooltip("Размер сцены в единицах unity (ширина, высота)")]
    [SerializeField] private Vector2 stageSize = new Vector2(16f, 9f);

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            Debug.Log("SpriteRenderer: нет SpriteRenderer на обьекте Stage");
    }

    private void OnValidate()
    {
        UpdateStageVisual();
    }

    public void SetStageSize(float width, float height)
    {
        stageSize.x = Mathf.Max(0.1f, width);
        stageSize.y = Mathf.Max(0.1f, height);
        UpdateStageVisual();
    }

    public Vector2 GetStageSize() => stageSize;
    
    private void UpdateStageVisual()
    {
        if (spriteRenderer == null) return;

        transform.localScale = new Vector3(stageSize.x, stageSize.y, 1f);
    }
}

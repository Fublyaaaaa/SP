using UnityEngine;

[System.Serializable]
public class DancerData : MonoBehaviour
{
    [Header("Dancer Identity")]
    public string Id;
    public string DancerName = "Dancer";
    
    [Header("Visual Settings")]
    public Color Color = Color.white;
    public int OrderInFormation = 0;
    
    private void Awake()
    {
        // Генерируем уникальный ID если его нет
        if (string.IsNullOrEmpty(Id))
        {
            Id = System.Guid.NewGuid().ToString();
        }
    }
    
    // Метод для клонирования данных
    public DancerData Clone()
    {
        DancerData clone = new DancerData();
        clone.Id = System.Guid.NewGuid().ToString();
        clone.DancerName = this.DancerName + " Copy";
        clone.Color = this.Color;
        clone.OrderInFormation = this.OrderInFormation;
        return clone;
    }
}
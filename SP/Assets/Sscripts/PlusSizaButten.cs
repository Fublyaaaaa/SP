
using UnityEngine;


public class PlusSizaButten : MonoBehaviour
{

    [SerializeField] private StageController stageController;

    public void PlusWidth()
    {
        if (stageController != null)
        {
            Vector2 currentSize = stageController.GetStageSize();
            stageController.SetStageSize(currentSize.x + 1f, currentSize.y);
        }
    }
    public void PlusHeight()
    {
        if (stageController != null)
        {
            Vector2 currentSize = stageController.GetStageSize();
            stageController.SetStageSize(currentSize.x, currentSize.y + 1f);
        }
    }
    public void MinusHeight()
    {
        if (stageController != null)
        {
            Vector2 currentSize = stageController.GetStageSize();
            stageController.SetStageSize(currentSize.x, currentSize.y - 1f);
        }
    }
    public void MinusWidth()
    {
        if (stageController != null)
        {
            Vector2 currentSize = stageController.GetStageSize();
            stageController.SetStageSize(currentSize.x - 1f, currentSize.y);
        }
    }


}

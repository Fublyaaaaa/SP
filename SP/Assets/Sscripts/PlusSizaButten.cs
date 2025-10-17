
using UnityEngine;


public class PlusSizaButten : MonoBehaviour
{

    [SerializeField] private StageController stageController;

    public void PlusWidth()
    {
        if (stageController != null)
        {
            Vector2 currentSize = stageController.GetStageSize();
            if(currentSize.x < 24)
            {
            stageController.SetStageSize(currentSize.x + 1f, currentSize.y);
            }
            else
            {
              return;
            }
        }
    }
    public void PlusHeight()
    {
        if (stageController != null)
        {
            Vector2 currentSize = stageController.GetStageSize();
            if(currentSize.y < 24)
            {
            stageController.SetStageSize(currentSize.x, currentSize.y + 1f);
            }
            else
            {
                return;
            }
        }
    }
    public void MinusHeight()
    {
        if (stageController != null)
        {
            Vector2 currentSize = stageController.GetStageSize();
            if(currentSize.y > 1)
            {
            stageController.SetStageSize(currentSize.x, currentSize.y - 1f);
            }
            else
            {
                return;
            }
        }
    }
    public void MinusWidth()
    {
        if (stageController != null)
        {
            Vector2 currentSize = stageController.GetStageSize();
            if(currentSize.x > 1)
            {
            stageController.SetStageSize(currentSize.x - 1f, currentSize.y);
            }
            else
            {
                return;
            }
        }
    }


}

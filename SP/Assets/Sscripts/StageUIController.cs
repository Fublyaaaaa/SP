
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class StageUIController : MonoBehaviour
{
    [SerializeField] private StageController stageController;
    [SerializeField] private TMP_InputField widthInput;
    [SerializeField] private TMP_InputField heightInput;

    private void Start()
    {
        if (stageController == null) Debug.LogError("stageUi: ne naznachen StageController");

        if (widthInput == null || heightInput == null) Debug.LogError("Dolbaeb Naznach knopki!");

        var size = stageController.GetStageSize();
        widthInput.text = size.x.ToString("0.##");
        heightInput.text = size.y.ToString("0.##");
    }

    public void OnApplySize()
    {
        if (stageController == null) return;


        if (float.TryParse(widthInput.text, out float w) && float.TryParse(heightInput.text, out float h))
        {
            if (w > 24 || h > 24 || w < 0 || h < 0)
            {
                return;
            }
            else
            {
                     stageController.SetStageSize(w, h);
            }   

        }
        else
        {
            Debug.LogWarning("Dohuia vvel");
        }
    }
    
}


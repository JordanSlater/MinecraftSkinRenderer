using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GUIController : MonoBehaviour
{
    private string filePathField = "";

    SkinLoader skinLoader;
    
    public RotationController rotationController;

    List<Rect> guiElements = new List<Rect>();

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 30;
        skinLoader = transform.root.GetComponent<SkinLoader>();
    }

    void OnGUI()
    {
        const int buffer = 10;
        guiElements.Clear();

        string prevFilePathField = filePathField;
        var filePathFieldRect = new Rect(buffer, buffer, Screen.width - 2*buffer, 20);
        guiElements.Add(filePathFieldRect);
        filePathField = GUI.TextField(filePathFieldRect, filePathField);
        if (System.String.IsNullOrEmpty(filePathField))
        {
            var filePathGhostTextRect = new Rect(buffer + 5, filePathFieldRect.y, filePathFieldRect.width, filePathFieldRect.height);
            guiElements.Add(filePathGhostTextRect);
            GUI.Label(filePathGhostTextRect, "Type path to file here");
        }
        if (prevFilePathField != filePathField)
            skinLoader.SetFilePath(filePathField);

        var rotationSensitivityLabelRect = new Rect(buffer, guiElements.Last().yMax, 100, 20);
        guiElements.Add(rotationSensitivityLabelRect);
        GUI.Label(rotationSensitivityLabelRect, "Look sensitivity: ");
        var rotationSensitivityRect = new Rect(rotationSensitivityLabelRect.xMax + buffer, rotationSensitivityLabelRect.yMin + 5, 150, 20);
        guiElements.Add(rotationSensitivityRect);
        rotationController.sensitivity = GUI.HorizontalSlider(rotationSensitivityRect, rotationController.sensitivity, 1, 10);
        
        var AlexToggleRect = new Rect(buffer, guiElements.Last().yMax - 5, 120, 15);
        guiElements.Add(AlexToggleRect);
        skinLoader.UseAlexVariant = GUI.Toggle(AlexToggleRect, skinLoader.UseAlexVariant, "Use Alex Model");

        var prevAlignment = GUI.skin.label.alignment;
        GUI.skin.label.alignment = TextAnchor.LowerLeft;
        const int statusMeassageRectHeight = 200;
        var statusMeassageRect = new Rect(buffer, Screen.height - statusMeassageRectHeight - buffer, Screen.width - 2*buffer, statusMeassageRectHeight);
        GUI.Label(statusMeassageRect, skinLoader.StatusMessage);
        GUI.skin.label.alignment = prevAlignment;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            bool usingGUI = false;
            foreach (var rect in guiElements)
            {
                Vector3 mousePositionOnGUI = Input.mousePosition;
                mousePositionOnGUI.y = Screen.height - mousePositionOnGUI.y;
                usingGUI |= rect.Contains(mousePositionOnGUI);
            }
            rotationController.ableToRotate = !usingGUI;
        }
        if (Input.GetMouseButtonUp(0))
            rotationController.ableToRotate = true;
    }
}

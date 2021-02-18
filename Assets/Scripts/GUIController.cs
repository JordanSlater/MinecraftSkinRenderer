using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GUIController : MonoBehaviour
{
    private string filePathField = "";

    SkinLoader skinLoader;
    
    public RotationController rotationController;

    // gui elements that shouldn't be clicked on while rotating
    List<Rect> guiElements = new List<Rect>();
    private const int buffer = 10;
    public Texture helpTexture;
    private Rect filePathFieldRect;
    private Rect filePathGhostTextRect;
    private Rect rotationSensitivityLabelRect;
    private Rect rotationSensitivityRect;
    private Rect alexToggleRect;
    private const int helpSize = 30;
    private string helpURL = "https://github.com/JordanSlater/MinecraftSkinRenderer";
    private Rect helpRect;
    private GUIStyle helpStyle;
    const int statusMeassageRectHeight = 200;
    private Rect statusMeassageRect;
    private GUIStyle statusMessageStyle;

    void Awake()
    {
        Application.targetFrameRate = 30;
        skinLoader = transform.root.GetComponent<SkinLoader>();

        filePathFieldRect = new Rect(buffer, buffer, Screen.width - 2*buffer, 20);
        guiElements.Add(filePathFieldRect);
        filePathGhostTextRect = new Rect(buffer + 5, filePathFieldRect.y, filePathFieldRect.width, filePathFieldRect.height);
        guiElements.Add(filePathGhostTextRect);

        rotationSensitivityLabelRect = new Rect(buffer, guiElements.Last().yMax, 100, 20);
        guiElements.Add(rotationSensitivityLabelRect);
        rotationSensitivityRect = new Rect(rotationSensitivityLabelRect.xMax + buffer, rotationSensitivityLabelRect.yMin + 5, 150, 20);
        guiElements.Add(rotationSensitivityRect);

        alexToggleRect = new Rect(buffer, guiElements.Last().yMax - 5, 120, 15);
        guiElements.Add(alexToggleRect);

        helpRect = new Rect(Screen.width - helpSize - buffer, Screen.height - helpSize - buffer, helpSize, helpSize);
        helpStyle = new GUIStyle() { imagePosition = ImagePosition.ImageOnly };
        guiElements.Add(helpRect);

        statusMeassageRect = new Rect(buffer, Screen.height - statusMeassageRectHeight - buffer, helpRect.xMin - 2*buffer, statusMeassageRectHeight);
    }

    void OnGUI()
    {
        GUI.skin.label.alignment = TextAnchor.LowerLeft;

        string prevFilePathField = filePathField;
        filePathField = GUI.TextField(filePathFieldRect, filePathField);
        if (System.String.IsNullOrEmpty(filePathField))
            GUI.Label(filePathGhostTextRect, "Type path to file here");
        if (prevFilePathField != filePathField)
            skinLoader.SetFilePath(filePathField);

        GUI.Label(rotationSensitivityLabelRect, "Look sensitivity: ");
        
        rotationController.sensitivity = GUI.HorizontalSlider(rotationSensitivityRect, rotationController.sensitivity, 1, 10);
        
        skinLoader.UseAlexVariant = GUI.Toggle(alexToggleRect, skinLoader.UseAlexVariant, "Use Alex Model");

        if (GUI.Button(helpRect, helpTexture, helpStyle))
            Application.OpenURL(helpURL);
        
        GUI.Label(statusMeassageRect, skinLoader.StatusMessage);
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

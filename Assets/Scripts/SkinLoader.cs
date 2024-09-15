using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO;
using Utils;

public class SkinLoader : MonoBehaviour
{
    private Texture2D skin;

    public Material SkinMaterial;

    private DateTime lastFileTimeStamp = DateTime.MinValue;

    private string filePath = null;

    public string StatusMessage = "No skin to load.";

    private bool loadDefaultSkin = true;
    private bool useAlexVariant;
    public bool UseAlexVariant
    {
        get => useAlexVariant;
        set {
            if (useAlexVariant != value)
                ReloadSkin();
            useAlexVariant = value;
        }
    }

    private const string SteveLocation = "Assets/TestSkins/steve_skin.png";
    private const string AlexLocation = "Assets/TestSkins/alex_skin.png";

    public void SetFilePath(string newFilePath)
    {
        filePath = newFilePath;
        lastFileTimeStamp = DateTime.MinValue;
        ReloadSkin();
    }

    // Start is called before the first frame update
    void Start()
    {
        skin = new Texture2D(Constants.ExpectedWidth, Constants.ExpectedHeight);
        skin.filterMode = FilterMode.Point;
    }

    private const float refreshRate = 0.2f;
    private const int maxFileReads = 5;
    private int numSkinLoads = 0;
    private float lastFileRead = 0;
    private bool skinLoaded = false;

    private void ReloadSkin()
    {
        Debug.Log("Called ReloadSkin");
        numSkinLoads = 0;
        skinLoaded = false;
    }

    // Update is called once per frame
    void Update()
    {
        string toLoad = filePath;
        if (ShouldReadFile(out DateTime fileTimeStamp))
        {
            StatusMessage = "Skin found, loading ...";
            lastFileTimeStamp = fileTimeStamp;
            loadDefaultSkin = false;
            ReloadSkin();
        }

        if (!skinLoaded && loadDefaultSkin)
        {
            toLoad = SteveLocation;
            if (UseAlexVariant)
                toLoad = AlexLocation;
            ReloadSkin();
        }

        if (!skinLoaded)
        {
            if (numSkinLoads < maxFileReads && Time.time - lastFileRead > refreshRate)
            {
                lastFileRead = Time.time;
                byte[] fileData = GetFileBytes(toLoad);
                
                if (FileDataWouldMakeValidSkin(fileData))
                {
                    skin.LoadImage(fileData);
                    SkinMaterial.SetTexture("_MainTex", skin);
                    StatusMessage = $"Loaded skin {filePath} successfully.";
                    skinLoaded = true;
                }
                numSkinLoads++;
            }
            else if (numSkinLoads >= maxFileReads)
            {
                StatusMessage = "Failed to load skin after " + numSkinLoads + " tries.";
            }
        }
    }

    private bool ShouldReadFile(out DateTime fileTimeStamp)
    {
        fileTimeStamp = new DateTime();
        if (System.String.IsNullOrEmpty(filePath))
        {
            StatusMessage = "No file specified, loading default skin";
            loadDefaultSkin = true;
            return false;
        }
        if (!File.Exists(filePath))
        {
            StatusMessage = "File " + filePath + " does not exist.";
            return false;
        }

        fileTimeStamp = File.GetLastWriteTime(filePath);

        return (DateTime.Compare(fileTimeStamp, lastFileTimeStamp) == 1);
    }

    private static byte[] GetFileBytes(string filePath)
    {
        byte[] bytes = null;
        try
        {
            using(FileStream fileStream = (new FileInfo(filePath)).Open(FileMode.Open, FileAccess.Read, FileShare.None))
            {
                using(var memoryStream = new MemoryStream())
                {
                    fileStream.CopyTo(memoryStream);
                    bytes = memoryStream.ToArray();
                }
            }
        }
        catch (IOException)
        {
            return null;
        }
        return bytes;
    }

    private bool FileDataWouldMakeValidSkin(byte[] fileData)
    {
        Texture2D tempSkin = new Texture2D(Constants.ExpectedWidth, Constants.ExpectedHeight);
        tempSkin.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        return tempSkin.width == Constants.ExpectedWidth && tempSkin.height == Constants.ExpectedHeight;
    }
}

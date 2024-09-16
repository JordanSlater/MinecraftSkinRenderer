using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO;
using Utils;

public class SkinLoader : MonoBehaviour
{
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

    public Texture2D SteveSkinTexture;
    public Texture2D AlexSkinTexture;

    public void SetFilePath(string newFilePath)
    {
        filePath = newFilePath;
        lastFileTimeStamp = DateTime.MinValue;
        ReloadSkin();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    private Texture2D CreateSkin()
    {
        Texture2D skin;
        skin = new Texture2D(Constants.ExpectedWidth, Constants.ExpectedHeight);
        skin.filterMode = FilterMode.Point;
        return skin;
    }

    private const float refreshRate = 0.2f;
    private const int maxFileReads = 5;
    private int numSkinLoads = 0;
    private float lastFileRead = 0;
    private bool skinLoaded = false;

    private void ReloadSkin()
    {
        numSkinLoads = 0;
        skinLoaded = false;
    }

    // Update is called once per frame
    void Update()
    {
        Texture2D skinTextureToLoad = null;

        if (ShouldReadFile(out DateTime fileTimeStamp))
        {
            StatusMessage = "Skin found, loading ...";
            lastFileTimeStamp = fileTimeStamp;
            loadDefaultSkin = false;
            ReloadSkin();
        }

        if (!skinLoaded)
        {
            if (loadDefaultSkin)
            {
                skinTextureToLoad = SteveSkinTexture;
                if (UseAlexVariant)
                    skinTextureToLoad = AlexSkinTexture;
            }
            else
            {
                if (numSkinLoads < maxFileReads && Time.time - lastFileRead > refreshRate)
                {
                    lastFileRead = Time.time;
                    byte[] fileData = GetFileBytes(filePath);
                
                    if (FileDataWouldMakeValidSkin(fileData))
                    {
                        skinTextureToLoad = CreateSkin();
                        skinTextureToLoad.LoadImage(fileData);
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
        if (skinTextureToLoad != null)
            SkinMaterial.SetTexture("_MainTex", skinTextureToLoad);
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

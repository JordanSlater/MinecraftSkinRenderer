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

    private string filePath;

    public string StatusMessage = "No skin to load.";

    public bool UseAlexVariant;

    public void SetFilePath(string newFilePath)
    {
        filePath = newFilePath;
        lastFileTimeStamp = DateTime.MinValue;
    }

    // Start is called before the first frame update
    void Start()
    {
        skin = new Texture2D(Constants.ExpectedWidth, Constants.ExpectedHeight);
        skin.filterMode = FilterMode.Point;

        // filePath = type one here to load on start
        // skinLoaded = false;
    }

    private const float refreshRate = 0.2f;
    private const int maxFileReads = 5;
    private int numSkinLoads = 0;
    private float lastFileRead = 0;
    private bool skinLoaded = true;

    // Update is called once per frame
    void Update()
    {
        if (ShouldReadFile(out DateTime fileTimeStamp))
        {
            StatusMessage = "Skin found, loading ...";
            lastFileTimeStamp = fileTimeStamp;
            numSkinLoads = 0;
            skinLoaded = false;
        }

        if (!skinLoaded)
        {
            if (numSkinLoads < maxFileReads && Time.time - lastFileRead > refreshRate)
            {
                lastFileRead = Time.time;
                byte[] fileData = GetFileBytes();
                
                if (FileDataWouldMakeValidSkin(fileData))
                {
                    skin.LoadImage(fileData);
                    SkinMaterial.SetTexture("_MainTex", skin);
                    StatusMessage = "Loaded skin successfully.";
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
            StatusMessage = "No file specified.";
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

    private byte[] GetFileBytes()
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

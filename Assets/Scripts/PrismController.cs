using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrismController : MonoBehaviour
{
    enum Face
    {
        Front,
        Top,
        Back,
        Bottom,
        Left, // the character's left
        Right, // the character's right
    };
    private static Array faces = System.Enum.GetValues(typeof(Face));

    private Dictionary<Face, Rect> skinMap;

    private static readonly Vector2[] corners = new Vector2[] {
        new Vector2(0, 0),
        new Vector2(1, 0),
        new Vector2(0, 1),
        new Vector2(1, 1),
    };

    private static readonly Dictionary<Face, int[]> uvIndices = new Dictionary<Face, int[]>
    {
        { Face.Front, new int[]{ 0, 1, 2, 3 } },
        { Face.Top, new int[]{ 8, 9, 4, 5 } },
        { Face.Back, new int[]{ 7, 6, 11, 10 } },
        { Face.Bottom, new int[]{ 13, 14, 12, 15 } },
        { Face.Left, new int[]{ 16, 19, 17, 18 } },
        { Face.Right, new int[]{ 20, 23, 21, 22 } },
    };

    MeshFilter meshFilter;

    [Tooltip("X coord of the bottom left corner of the front face. Measured from the left of the skin image.")]
    public int SkinLocationX;
    [Tooltip("Y coord of the bottom left corner of the front face. Measured from the bottom of the skin image.")]
    public int SkinLocationY;

    [Tooltip("The width of the front face.")]
    public int Width;
    [Tooltip("The height of the front face.")]
    public int Height;
    [Tooltip("The depth of this prism/bodypart.")]
    public int Depth;

    // Start is called before the first frame update
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();

        skinMap = new Dictionary<Face, Rect> {
            { Face.Front, new Rect(SkinLocationX, SkinLocationY, Width, Height) },
            { Face.Top, new Rect(SkinLocationX, SkinLocationY + Height, Width, Depth) },
            { Face.Back, new Rect(SkinLocationX + Width + Depth, SkinLocationY, Width, Height) },
            { Face.Bottom, new Rect(SkinLocationX + Width, SkinLocationY + Height, Width, Depth) },
            { Face.Left, new Rect(SkinLocationX + Width, SkinLocationY, Depth, Height) },
            { Face.Right, new Rect(SkinLocationX - Depth, SkinLocationY, Depth, Height) },
        };
        meshFilter.mesh.uv = PopulateExternalUVs(meshFilter?.mesh?.uv, skinMap);
        PopulateInternalUVs(transform.Find("Internal"), skinMap);
    }

    private static void PopulateInternalUVs(Transform internalFacesObject, Dictionary<Face, Rect> skinMap)
    {
        if (internalFacesObject == null)
            throw new NullReferenceException("internalFacesObject is null");
        Dictionary<Face, MeshFilter> internalMeshes = MapInternalFaceObjectsToFaces(internalFacesObject);
        foreach (Face face in faces)
        {
            var uvs = internalMeshes[face]?.mesh?.uv;
            if (uvs == null)
                throw new NullReferenceException("uvs is null for internal faces");
            for (int i = 0; i < 4; i++)
                uvs[i] = Utils.HelperFunctions.NormalizeToSkin(Rect.NormalizedToPoint(skinMap[face], corners[i]));
            internalMeshes[face].mesh.uv = uvs;
        }
    }

    private static Dictionary<Face, MeshFilter> MapInternalFaceObjectsToFaces(Transform internalFacesObject)
    {
        Dictionary<Face, MeshFilter> internalMeshes = new Dictionary<Face, MeshFilter>();
        foreach (Face face in faces)
        {
            var faceObject = internalFacesObject.Find(face.ToString());
            var meshFilter = faceObject.GetComponent<MeshFilter>();
            internalMeshes[face] = meshFilter;
            if (meshFilter == null)
                throw new NullReferenceException("meshFilter is null");
        }
        if (internalMeshes.Count != faces.Length)
            throw new NullReferenceException("not all internal meshes found");
        return internalMeshes;
    }

    private static Vector2[] PopulateExternalUVs(Vector2[] uvs, Dictionary<Face, Rect> skinMap)
    {
        if (uvs == null)
            throw new NullReferenceException("uvs is null for external faces");
        foreach (Face face in faces)
        {
            for (int i = 0; i < 4; i++)
                uvs[uvIndices[face][i]] = Utils.HelperFunctions.NormalizeToSkin(Rect.NormalizedToPoint(skinMap[face], corners[i]));
        }
        return uvs;
    }

    // Update is called once per frame
    void Update()
    {
    }
}

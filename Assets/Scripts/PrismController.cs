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

    private Dictionary<Face, Rect> skinMap;

    private Vector2[] corners = new Vector2[] {
        new Vector2(0, 0),
        new Vector2(1, 0),
        new Vector2(0, 1),
        new Vector2(1, 1),
    };

    private Dictionary<Face, int[]> uvIndices = new Dictionary<Face, int[]>
    {
        { Face.Front, new int[]{ 0, 1, 2, 3 } },
        { Face.Top, new int[]{ 8, 9, 4, 5 } },
        { Face.Back, new int[]{ 7, 6, 11, 10 } },
        { Face.Bottom, new int[]{ 13, 14, 12, 15 } },
        { Face.Left, new int[]{ 16, 19, 17, 18 } },
        { Face.Right, new int[]{ 20, 23, 21, 22 } },
    };


    int[][] quadPermutations = new int[][] {
        new int[] {0,1,2,3},
        new int[] {1,0,2,3},
        new int[] {2,0,1,3},
        new int[] {0,2,1,3},
        new int[] {1,2,0,3},
        new int[] {2,1,0,3},
        new int[] {2,1,3,0},
        new int[] {1,2,3,0},
        new int[] {3,2,1,0},
        new int[] {2,3,1,0},
        new int[] {1,3,2,0},
        new int[] {3,1,2,0},
        new int[] {3,0,2,1},
        new int[] {0,3,2,1},
        new int[] {2,3,0,1},
        new int[] {3,2,0,1},
        new int[] {0,2,3,1},
        new int[] {2,0,3,1},
        new int[] {1,0,3,2},
        new int[] {0,1,3,2},
        new int[] {3,1,0,2},
        new int[] {1,3,0,2},
        new int[] {0,3,1,2},
        new int[] {3,0,1,2},
    };

    int permutation = 0;

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

        if (permute)
        {
            for (int i = 0; i < 4; i++)
                Debug.Log(uvIndices[Face.Bottom][quadPermutations[18][i]]);
            Debug.Log("best: ^ ");
        }
    }

    static int tick = 0;
    bool permute = false;

    // Update is called once per frame
    void Update()
    {
        // TODO move back to Start when you're done
        var uvs = meshFilter?.mesh?.uv;
        if (uvs == null)
        {
            Debug.Log("uvs is null");
            return;
        }
        if (permute)
        {
            const int refreshTickRate = 40;
            tick = (tick + 1) % refreshTickRate;
            if (tick % refreshTickRate == 0)
            {
                permutation = (permutation + 1) % quadPermutations.Length;
                Debug.Log(permutation);
            }
        }
        foreach (Face face in System.Enum.GetValues(typeof(Face)))
        {
            if (permute && face == Face.Bottom)
            {
                for (int i = 0; i < 4; i++)
                    uvs[uvIndices[face][quadPermutations[permutation][i]]] = Utils.HelperFunctions.NormalizeToSkin(Rect.NormalizedToPoint(skinMap[face], corners[i]));
            } else
            {
                for (int i = 0; i < 4; i++)
                    uvs[uvIndices[face][i]] = Utils.HelperFunctions.NormalizeToSkin(Rect.NormalizedToPoint(skinMap[face], corners[i]));
            }
        }
        meshFilter.mesh.uv = uvs;
    }
}

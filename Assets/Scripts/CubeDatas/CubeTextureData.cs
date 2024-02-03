using System.Collections.Generic;
using UnityEngine;
using static CubeMeshData;

[System.Serializable]
public struct CubeTextureData
{
    public int textureAtlasId;
    public float cellSize;

    [Tooltip("x = Column Index, y = Row Index")]
    public Vector2Int frontUV;

    [Tooltip("x = Column Index, y = Row Index")]
    public Vector2Int topUV;

    [Tooltip("x = Column Index, y = Row Index")]
    public Vector2Int backUV;

    [Tooltip("x = Column Index, y = Row Index")]
    public Vector2Int bottomUV;

    [Tooltip("x = Column Index, y = Row Index")]
    public Vector2Int rightUV;

    [Tooltip("x = Column Index, y = Row Index")]
    public Vector2Int leftUV;


    public Dictionary<FaceOrientation, Vector2Int> faceUVs
    {
        get
        {
            return new Dictionary<FaceOrientation, Vector2Int>
            {
                { FaceOrientation.Front, frontUV},
                { FaceOrientation.Top, topUV},
                { FaceOrientation.Back, backUV},
                { FaceOrientation.Bottom, bottomUV},
                { FaceOrientation.Right, rightUV},
                { FaceOrientation.Left, leftUV},
            };
        }
    }
}

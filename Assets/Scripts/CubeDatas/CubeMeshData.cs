using System;
using System.Collections.Generic;
using UnityEngine;

public static class CubeMeshData
{
    public const int kFaceCount = 6;

    private static Vector3 _v0 = new(-0.5f, -0.5f, -0.5f);
    private static Vector3 _v1 = new(-0.5f, 0.5f, -0.5f);
    private static Vector3 _v2 = new(0.5f, 0.5f, -0.5f);
    private static Vector3 _v3 = new(0.5f, -0.5f, -0.5f);

    private static Vector3 _v4 = new(-0.5f, -0.5f, 0.5f);
    private static Vector3 _v5 = new(-0.5f, 0.5f, 0.5f);
    private static Vector3 _v6 = new(0.5f, 0.5f, 0.5f);
    private static Vector3 _v7 = new(0.5f, -0.5f, 0.5f);

    public static Vector3[] vertices =
    {
        //FRONT
        _v0,//0
        _v1,//1
        _v2,//2
        _v3,//3

        //TOP
        _v1,//4
        _v5,//5
        _v6,//6
        _v2,//7

        //BACK
        _v4,//8
        _v5,//9
        _v6,//10
        _v7,//11

        //BOTTOM
        _v0,//12
        _v4,//13
        _v7,//14
        _v3,//15

        //RIGHT
        _v3,//16
        _v2,//17
        _v6,//18
        _v7,//19

        //LEFT
        _v0,//20
        _v1,//21
        _v5,//22
        _v4,//23
    };

    public static Vector3[] GetVertices(float size)
    {
        Vector3[] _localVertices = new Vector3[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
            _localVertices[i] = vertices[i] * size;

        return _localVertices;
    }

    private static QuadFace _front = new(new int[] { 0, 1, 2 }, new int[] { 0, 2, 3 });
    private static QuadFace _top = new(new int[] { 4, 5, 6 }, new int[] { 4, 6, 7 });
    private static QuadFace _back = new(new int[] { 8, 10, 9 }, new int[] { 8, 11, 10 });
    private static QuadFace _bottom = new(new int[] { 12, 14, 13 }, new int[] { 12, 15, 14 });
    private static QuadFace _right = new(new int[] { 16, 17, 18 }, new int[] { 16, 18, 19 });
    private static QuadFace _left = new(new int[] { 20, 22, 21 }, new int[] { 20, 23, 22 });

    public static QuadFace[] quads =
    {
        new( new int[]{0,1,2}, new int[]{0,2,3}), // FRONT
        new( new int[]{4,5,6}, new int[]{4,6,7}), // TOP
        new( new int[]{8,10,9}, new int[]{8,11,10}), // BACK
        new( new int[]{12,14,13}, new int[]{12,15,14}), // BOTTOM
        new( new int[]{16,17,18}, new int[]{16,18,19}), // RIGHT
        new( new int[]{20,22,21}, new int[]{20,23,22}), // LEFT
    };

    public static int[] GetTriangles(uint faceBitMask)
    {
        List<int> triangles = new();

        var values = (FaceOrientation[])Enum.GetValues(typeof(FaceOrientation));

        for (int i = 0; i < values.Length; i++)
        {
            bool contains = faceBitMask.Contains(i);
            if (contains)
            {
                var faceOrientation = values[i];
                QuadFace quadToAdd = faceOrientation switch
                {
                    FaceOrientation.Front => _front,
                    FaceOrientation.Top => _top,
                    FaceOrientation.Back => _back,
                    FaceOrientation.Bottom => _bottom,
                    FaceOrientation.Right => _right,
                    FaceOrientation.Left => _left,
                    _ => default
                };

                triangles.AddRange(quadToAdd.GetTriangles());
            }
        }

        return triangles.ToArray();
    }

    public static Vector2[] uvs =
    {
        new Vector2(0,0),
        new Vector2(0,1),
        new Vector2(1,1),
        new Vector2(1,0)
    };

    public static Vector2[] GetUVs(Dictionary<FaceOrientation, Vector2Int> faceCoords, float cellSize = 16, float textureSize = 512f)
    {
        List<Vector2> uvs = new();
        foreach (var faceCoord in faceCoords)
        {
            var faceUVs = CalculateUV(faceCoord.Value, cellSize, textureSize);
            uvs.AddRange(faceUVs);
        }

        return uvs.ToArray();
    }

    public static Vector2[] CalculateUV(Vector2Int columnRow, float cellSize = 16, float textureSize = 512f)
    {
        Vector2[] uvs = new Vector2[4];
        float factor = cellSize / textureSize;

        uvs[0] = new Vector2(columnRow.x * factor, columnRow.y * factor); //0,0
        uvs[1] = new Vector2(columnRow.x * factor, (columnRow.y + 1) * factor); //0,1
        uvs[2] = new Vector2((columnRow.x + 1) * factor, (columnRow.y + 1) * factor); //1,1
        uvs[3] = new Vector2((columnRow.x + 1) * factor, columnRow.y * factor); //1,0

        return uvs;
    }

    public static Vector2[] GetUVs(int facesCount)
    {
        List<Vector2> _uvs = new();

        for (int i = 0; i < facesCount; i++)
            _uvs.AddRange(uvs);

        return _uvs.ToArray();
    }

    /// <summary>
    /// Each Quad Face is made of two triangles
    /// </summary>
    public struct QuadFace
    {
        /// <summary>
        /// Triangle Top Ids
        /// </summary>
        public int[] triangleTop;

        /// <summary>
        /// Triangle Bottom Ids
        /// </summary>
        public int[] triangleBottom;

        public QuadFace(int[] triangleTop, int[] triangleBottom)
        {
            this.triangleTop = triangleTop;
            this.triangleBottom = triangleBottom;
        }

        public int[] GetTriangles()
        {
            List<int> triangles = new();
            triangles.AddRange(triangleTop);
            triangles.AddRange(triangleBottom);
            return triangles.ToArray();
        }
    }

    [System.Flags]
    public enum FaceOrientation
    {
        Front = 1,
        Top = 2,
        Back = 4,
        Bottom = 8,
        Right = 16,
        Left = 32,
    }
}

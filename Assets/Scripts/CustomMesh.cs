using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class CustomMesh : MonoBehaviour
{
    [SerializeField] CubeMeshData.FaceOrientation _visibleFaces;
    [SerializeField] private Material _material;
    [SerializeField] private float _size = 1.0f;
    [SerializeField] private BlockData _blockData;

    private MeshRenderer _meshRenderer;
    private MeshFilter _meshFilter;
    private Mesh _mesh;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshFilter = GetComponent<MeshFilter>();
        _mesh = new();

        ((uint)_visibleFaces).DebugConsole();
    }
    void Start()
    {
        DrawCube(_size);

        UpdateMesh();
    }

    private void UpdateMesh()
    {
        _meshFilter.mesh = _mesh;
        _meshRenderer.sharedMaterial = _material;
    }

    private void DrawCube(float size)
    {
        _mesh.Clear();

        Vector3[] vertices = CubeMeshData.GetVertices(size);

        int[] triangles = CubeMeshData.GetTriangles((uint)_visibleFaces);

        Vector2[] uvs = CubeMeshData.GetUVs(_blockData.textureData.faceUVs, _blockData.textureData.cellSize);

        _mesh.vertices = vertices;
        _mesh.triangles = triangles;
        _mesh.uv = uvs;
        _mesh.RecalculateNormals();
    }

    private static Mesh CreateQuad()
    {
        Mesh mesh = new();

        Vector3[] vertices = new Vector3[4];
        Vector2[] uvs = new Vector2[4];
        int[] triangles = new int[6];

        vertices[0] = new Vector3(0, 0, 0);
        vertices[1] = new Vector3(0, 100, 0);
        vertices[2] = new Vector3(100, 100, 0);
        vertices[3] = new Vector3(100, 0, 0);

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;

        triangles[3] = 0;
        triangles[4] = 2;
        triangles[5] = 3;

        uvs[0] = new Vector2(0, 0);
        uvs[1] = new Vector2(0, 1);
        uvs[2] = new Vector2(1, 1);
        uvs[3] = new Vector2(1, 0);

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        return mesh;
    }
}

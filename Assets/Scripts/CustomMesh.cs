using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class CustomMesh : MonoBehaviour
{
    public FaceOrientation _visibleFaces;
    [SerializeField] private Material _material;
    [SerializeField] private float _size = 1.0f;
    [SerializeField] private BlockData _blockData;

    private MeshRenderer _meshRenderer;
    private MeshFilter _meshFilter;
    private Mesh _mesh;

    private void Awake()
    {
        InitializeComponents();
    }

    void Start()
    {
        Draw();
    }

    private void InitializeComponents()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshFilter = GetComponent<MeshFilter>();
        _mesh = new();
    }

    [InspectorButton]
    public void Draw()
    {
        InitializeComponents();

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

        Vector2[] uvs = CubeMeshData.GetUVs(_blockData.textureData.faceUVs, _blockData.textureData.textureAtlasData.cellSize);

        _mesh.vertices = vertices;
        _mesh.triangles = triangles;
        _mesh.uv = uvs;
        _mesh.RecalculateNormals();
    }
}

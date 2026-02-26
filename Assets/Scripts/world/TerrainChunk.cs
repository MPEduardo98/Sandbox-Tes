using UnityEngine;

/// <summary>
/// Chunk visual + datos. 
/// Crea la malla del suelo Y guarda los datos del grid de celdas.
/// </summary>
public class TerrainChunk : MonoBehaviour
{
    // ─── Datos del chunk ──────────────────────────────────────────────────────

    /// <summary>Los datos lógicos de este chunk (celdas, ocupantes, etc.)</summary>
    public ChunkData Data { get; private set; }

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;

    void Awake()
    {
        meshFilter   = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshCollider = gameObject.AddComponent<MeshCollider>();
    }

    /// <summary>
    /// Inicializa el chunk: crea los datos del grid y construye la malla visual.
    /// </summary>
    public void Initialize(Vector2Int chunkGridPos, int chunkSize, Material material)
    {
        // Creamos los datos lógicos del chunk (el grid de celdas)
        Data = new ChunkData(chunkGridPos, chunkSize);

        // Creamos la malla visual
        meshRenderer.material = material;
        Mesh mesh = BuildFlatMesh(chunkSize);
        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
    }

    private Mesh BuildFlatMesh(int size)
    {
        Mesh mesh = new Mesh();
        mesh.name = "FlatChunk";

        Vector3[] vertices = new Vector3[]
        {
            new Vector3(0,    0, 0),
            new Vector3(size, 0, 0),
            new Vector3(0,    0, size),
            new Vector3(size, 0, size),
        };

        int[] triangles = new int[] { 0, 2, 1, 2, 3, 1 };

        // Los UVs ahora van de 0 a chunkSize para que cada celda = 1 tile de textura
        Vector2[] uvs = new Vector2[]
        {
            new Vector2(0,    0),
            new Vector2(size, 0),
            new Vector2(0,    size),
            new Vector2(size, size),
        };

        mesh.vertices  = vertices;
        mesh.triangles = triangles;
        mesh.uv        = uvs;
        mesh.RecalculateNormals();

        return mesh;
    }
}
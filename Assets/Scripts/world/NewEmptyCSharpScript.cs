using UnityEngine;

/// <summary>
/// Representa un pedazo individual de terreno (chunk).
/// Se encarga de crear su propia malla (mesh) cuadrada y plana.
/// </summary>
public class TerrainChunk : MonoBehaviour
{
    // ─── Variables que se configuran desde el ChunkManager ───────────────────

    [SerializeField] private Material terrainMaterial;

    // Referencia a los componentes de renderizado de la malla
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;

    // ─── Inicialización ───────────────────────────────────────────────────────

    void Awake()
    {
        // Agregamos los componentes necesarios para mostrar una malla 3D
        meshFilter   = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshCollider = gameObject.AddComponent<MeshCollider>();
    }

    /// <summary>
    /// Construye la geometría del chunk. Llamar esto justo después de instanciar.
    /// </summary>
    /// <param name="chunkSize">Cuántas unidades de lado tiene el chunk (ej: 16)</param>
    /// <param name="material">El material/textura que usará el suelo</param>
    public void Initialize(int chunkSize, Material material)
    {
        terrainMaterial = material;
        meshRenderer.material = terrainMaterial;

        Mesh mesh = BuildFlatMesh(chunkSize);
        meshFilter.mesh   = mesh;
        meshCollider.sharedMesh = mesh; // Para que el jugador pueda pararse encima
    }

    /// <summary>
    /// Crea una malla plana cuadrada.
    /// Una malla es una colección de vértices (puntos) y triángulos que los conectan.
    /// </summary>
    private Mesh BuildFlatMesh(int size)
    {
        Mesh mesh = new Mesh();
        mesh.name = "FlatChunk";

        // ── Vértices (las 4 esquinas del cuadrado) ────────────────────────────
        // En Unity: X = derecha, Y = arriba, Z = adelante
        // El chunk vive en Y=0 (el suelo)
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(0,    0, 0),     // esquina inferior-izquierda
            new Vector3(size, 0, 0),     // esquina inferior-derecha
            new Vector3(0,    0, size),  // esquina superior-izquierda
            new Vector3(size, 0, size),  // esquina superior-derecha
        };

        // ── Triángulos ────────────────────────────────────────────────────────
        // Una malla se compone de triángulos. Un cuadrado = 2 triángulos.
        // Los números son índices de la lista de vértices de arriba.
        // El orden importa: deben ir en sentido horario para que la cara mire hacia arriba.
        int[] triangles = new int[]
        {
            0, 2, 1,  // primer triángulo
            2, 3, 1   // segundo triángulo
        };

        // ── UVs (coordenadas de textura) ──────────────────────────────────────
        // Le dicen a Unity cómo pegar la textura sobre la malla.
        // (0,0) = esquina inferior-izquierda de la textura
        // (1,1) = esquina superior-derecha de la textura
        Vector2[] uvs = new Vector2[]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1),
        };

        mesh.vertices  = vertices;
        mesh.triangles = triangles;
        mesh.uv        = uvs;
        mesh.RecalculateNormals(); // Calcula la dirección de la luz automáticamente

        return mesh;
    }
}
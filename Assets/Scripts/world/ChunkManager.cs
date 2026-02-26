using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Administra qué chunks existen en el mundo según la posición del jugador.
/// Ahora también le pide al TreeGenerator que pueble cada chunk nuevo.
/// </summary>
public class ChunkManager : MonoBehaviour
{
    [Header("Configuración del Mundo")]
    [SerializeField] private int chunkSize = 32;
    [SerializeField] private int viewDistance = 4;

    [Header("Referencias")]
    [SerializeField] private Transform player;
    [SerializeField] private Material terrainMaterial;

    // Referencia al generador de árboles (está en este mismo GameObject)
    private TreeGenerator treeGenerator;

    private Dictionary<Vector2Int, TerrainChunk> activeChunks = new Dictionary<Vector2Int, TerrainChunk>();
    private Vector2Int lastPlayerChunkPos;

    void Awake()
    {
        // Buscamos el TreeGenerator en este mismo GameObject
        treeGenerator = GetComponent<TreeGenerator>();
    }

    void Start()
    {
        lastPlayerChunkPos = GetChunkPosition(player.position);
        UpdateChunks();
    }

    void Update()
    {
        Vector2Int currentChunkPos = GetChunkPosition(player.position);
        if (currentChunkPos != lastPlayerChunkPos)
        {
            lastPlayerChunkPos = currentChunkPos;
            UpdateChunks();
        }
    }

    private void UpdateChunks()
    {
        Vector2Int playerChunk = GetChunkPosition(player.position);
        HashSet<Vector2Int> chunksNeeded = new HashSet<Vector2Int>();

        for (int x = -viewDistance; x <= viewDistance; x++)
            for (int z = -viewDistance; z <= viewDistance; z++)
                if (x * x + z * z <= viewDistance * viewDistance)
                    chunksNeeded.Add(new Vector2Int(playerChunk.x + x, playerChunk.y + z));

        foreach (Vector2Int pos in chunksNeeded)
            if (!activeChunks.ContainsKey(pos))
                SpawnChunk(pos);

        List<Vector2Int> toRemove = new List<Vector2Int>();
        foreach (var kvp in activeChunks)
            if (!chunksNeeded.Contains(kvp.Key))
                toRemove.Add(kvp.Key);

        foreach (Vector2Int pos in toRemove)
        {
            Destroy(activeChunks[pos].gameObject);
            activeChunks.Remove(pos);
        }
    }

        private void SpawnChunk(Vector2Int gridPos)
    {
        Vector3 worldPos = new Vector3(gridPos.x * chunkSize, 0, gridPos.y * chunkSize);

        GameObject chunkGO = new GameObject($"Chunk_{gridPos.x}_{gridPos.y}");
        chunkGO.transform.position = worldPos;
        chunkGO.transform.parent   = this.transform;

        // Asignamos la layer Ground automáticamente a cada chunk
        chunkGO.layer = LayerMask.NameToLayer("Ground");

        TerrainChunk chunk = chunkGO.AddComponent<TerrainChunk>();
        chunk.Initialize(gridPos, chunkSize, terrainMaterial);

        if (treeGenerator != null)
            treeGenerator.GenerateTrees(chunk.Data, chunkGO.transform);

        activeChunks[gridPos] = chunk;
    }

    private Vector2Int GetChunkPosition(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt(worldPosition.x / chunkSize);
        int z = Mathf.FloorToInt(worldPosition.z / chunkSize);
        return new Vector2Int(x, z);
    }

    public CellData GetCellAtWorldPosition(Vector3 worldPos)
    {
        Vector2Int chunkPos = GetChunkPosition(worldPos);
        if (activeChunks.TryGetValue(chunkPos, out TerrainChunk chunk))
            return chunk.Data.GetCellFromWorldPosition(worldPos);
        return null;
    }


}
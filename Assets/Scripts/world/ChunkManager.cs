using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controla qué chunks existen en el mundo.
/// Cada frame revisa la posición del jugador y carga/descarga chunks según sea necesario.
/// Este script va en un GameObject vacío llamado "ChunkManager" en la escena.
/// </summary>
public class ChunkManager : MonoBehaviour
{
    // ─── Configuración (editable desde el Inspector) ──────────────────────────

    [Header("Configuración del Mundo")]
    [Tooltip("Tamaño en unidades de cada chunk (16 = chunk de 16x16 unidades)")]
    [SerializeField] private int chunkSize = 16;

    [Tooltip("Cuántos chunks se cargan en cada dirección alrededor del jugador")]
    [SerializeField] private int viewDistance = 5;

    [Header("Referencias")]
    [Tooltip("Arrastra aquí el Transform del jugador")]
    [SerializeField] private Transform player;

    [Tooltip("El material del suelo (puedes crear uno con una textura de césped, tierra, etc.)")]
    [SerializeField] private Material terrainMaterial;

    // ─── Variables internas ───────────────────────────────────────────────────

    // Diccionario: guarda los chunks que existen, indexados por su posición en la grilla
    // Ejemplo: el chunk en la posición (2, -1) de la grilla se guarda con la clave (2,-1)
    private Dictionary<Vector2Int, TerrainChunk> activeChunks = new Dictionary<Vector2Int, TerrainChunk>();

    // La posición en la grilla del chunk donde estaba el jugador el frame anterior
    private Vector2Int lastPlayerChunkPos;

    // ─── Unity Messages ───────────────────────────────────────────────────────

    void Start()
    {
        // Forzamos una actualización inicial para cargar los chunks alrededor del spawn
        lastPlayerChunkPos = GetChunkPosition(player.position);
        UpdateChunks();
    }

    void Update()
    {
        // Solo recalculamos si el jugador cambió de chunk (optimización importante)
        Vector2Int currentChunkPos = GetChunkPosition(player.position);

        if (currentChunkPos != lastPlayerChunkPos)
        {
            lastPlayerChunkPos = currentChunkPos;
            UpdateChunks();
        }
    }

    // ─── Lógica principal ─────────────────────────────────────────────────────

    /// <summary>
    /// Revisa qué chunks deben existir dado donde está el jugador,
    /// crea los que faltan y destruye los que ya están lejos.
    /// </summary>
    private void UpdateChunks()
    {
        Vector2Int playerChunk = GetChunkPosition(player.position);

        // Construimos un set con todas las posiciones que DEBERÍAN estar cargadas
        HashSet<Vector2Int> chunksNeeded = new HashSet<Vector2Int>();

        for (int x = -viewDistance; x <= viewDistance; x++)
        {
            for (int z = -viewDistance; z <= viewDistance; z++)
            {
                // Solo cargamos chunks dentro de un círculo (más natural que un cuadrado)
                if (x * x + z * z <= viewDistance * viewDistance)
                {
                    chunksNeeded.Add(new Vector2Int(playerChunk.x + x, playerChunk.y + z));
                }
            }
        }

        // ── Crear chunks que faltan ────────────────────────────────────────────
        foreach (Vector2Int pos in chunksNeeded)
        {
            if (!activeChunks.ContainsKey(pos))
            {
                SpawnChunk(pos);
            }
        }

        // ── Destruir chunks que ya no se necesitan ─────────────────────────────
        // Usamos una lista temporal porque no podemos modificar el diccionario mientras lo recorremos
        List<Vector2Int> chunksToRemove = new List<Vector2Int>();

        foreach (var kvp in activeChunks)
        {
            if (!chunksNeeded.Contains(kvp.Key))
            {
                chunksToRemove.Add(kvp.Key);
            }
        }

        foreach (Vector2Int pos in chunksToRemove)
        {
            Destroy(activeChunks[pos].gameObject);
            activeChunks.Remove(pos);
        }
    }

    /// <summary>
    /// Instancia un nuevo chunk en la posición de grilla indicada.
    /// </summary>
    private void SpawnChunk(Vector2Int gridPos)
    {
        // Convertimos la posición en la grilla a posición real en el mundo
        // Multiplicamos por chunkSize para que los chunks no se superpongan
        Vector3 worldPos = new Vector3(gridPos.x * chunkSize, 0, gridPos.y * chunkSize);

        // Creamos un GameObject vacío y le añadimos el componente TerrainChunk
        GameObject chunkGO = new GameObject($"Chunk_{gridPos.x}_{gridPos.y}");
        chunkGO.transform.position = worldPos;
        chunkGO.transform.parent   = this.transform; // Lo hacemos hijo del ChunkManager para tener orden en la jerarquía

        TerrainChunk chunk = chunkGO.AddComponent<TerrainChunk>();
        chunk.Initialize(chunkSize, terrainMaterial);

        activeChunks[gridPos] = chunk;
    }

    // ─── Utilidades ───────────────────────────────────────────────────────────

    /// <summary>
    /// Convierte una posición 3D del mundo a una posición 2D en la grilla de chunks.
    /// Ejemplo: si chunkSize=16 y el jugador está en x=20, está en el chunk x=1 de la grilla.
    /// </summary>
    private Vector2Int GetChunkPosition(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt(worldPosition.x / chunkSize);
        int z = Mathf.FloorToInt(worldPosition.z / chunkSize);
        return new Vector2Int(x, z);
    }
}
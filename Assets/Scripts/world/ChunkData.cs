using UnityEngine;

/// <summary>
/// Contiene el grid de celdas de un chunk.
/// Un chunk de 32x32 unidades tiene 32x32 = 1024 celdas.
/// No es un MonoBehaviour — es un contenedor de datos puro.
/// </summary>
public class ChunkData
{
    // ─── Datos ────────────────────────────────────────────────────────────────

    /// <summary>Posición de este chunk en la grilla de chunks</summary>
    public Vector2Int chunkGridPosition;

    /// <summary>
    /// El grid de celdas. Se accede como: cells[x, z]
    /// Ejemplo: cells[0, 0] = esquina inferior izquierda del chunk
    /// </summary>
    public CellData[,] cells;

    private int chunkSize;

    // ─── Constructor ──────────────────────────────────────────────────────────

    /// <summary>
    /// Crea el chunk y genera todas sus celdas automáticamente.
    /// </summary>
    /// <param name="chunkGridPos">Posición del chunk en la grilla de chunks</param>
    /// <param name="size">Cuántas celdas de lado tiene (ej: 32 → 32x32 celdas)</param>
    public ChunkData(Vector2Int chunkGridPos, int size)
    {
        chunkGridPosition = chunkGridPos;
        chunkSize = size;
        cells = new CellData[size, size];

        GenerateCells();
    }

    /// <summary>
    /// Llena el array de celdas con datos iniciales.
    /// Por ahora todas son Grass vacías.
    /// Aquí más adelante agregaremos generación procedural (árboles, rocas, etc.)
    /// </summary>
    private void GenerateCells()
    {
        for (int x = 0; x < chunkSize; x++)
        {
            for (int z = 0; z < chunkSize; z++)
            {
                // Posición global en la grilla (única en todo el mundo)
                Vector2Int globalGridPos = new Vector2Int(
                    chunkGridPosition.x * chunkSize + x,
                    chunkGridPosition.y * chunkSize + z
                );

                // Posición real en el mundo 3D
                Vector3 worldPos = new Vector3(globalGridPos.x, 0f, globalGridPos.y);

                cells[x, z] = new CellData(globalGridPos, worldPos);
            }
        }
    }

    // ─── Utilidades ───────────────────────────────────────────────────────────

    /// <summary>
    /// Devuelve la celda en una posición local del chunk (0 a chunkSize-1).
    /// Devuelve null si la posición está fuera del chunk.
    /// </summary>
    public CellData GetCell(int localX, int localZ)
    {
        if (localX < 0 || localX >= chunkSize || localZ < 0 || localZ >= chunkSize)
            return null;

        return cells[localX, localZ];
    }

    /// <summary>
    /// Dado un punto en el mundo, devuelve la celda más cercana dentro de este chunk.
    /// Útil para saber "¿en qué celda está el jugador?"
    /// </summary>
    public CellData GetCellFromWorldPosition(Vector3 worldPos)
    {
        int localX = Mathf.FloorToInt(worldPos.x) - chunkGridPosition.x * chunkSize;
        int localZ = Mathf.FloorToInt(worldPos.z) - chunkGridPosition.y * chunkSize;
        return GetCell(localX, localZ);
    }
}
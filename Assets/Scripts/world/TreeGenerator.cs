using UnityEngine;

/// <summary>
/// Genera árboles en cada chunk usando Perlin Noise.
/// Los árboles aparecen en grupos naturales, respetan el grid de celdas
/// y quedan registrados en CellData para que otros sistemas sepan que la celda está ocupada.
/// Este script va en el GameObject ChunkManager.
/// </summary>
public class TreeGenerator : MonoBehaviour
{
    [Header("Prefab")]
    [Tooltip("Arrastra aquí el prefab de tu árbol 3D")]
    [SerializeField] private GameObject treePrefab;

    [Header("Distribución con Perlin Noise")]
    [Tooltip("Escala del ruido. Valor bajo = bosques grandes. Valor alto = bosques pequeños y dispersos")]
    [SerializeField] private float noiseScale = 0.08f;

    [Tooltip("A partir de qué valor del ruido aparece un árbol (0 a 1). Más alto = menos árboles")]
    [SerializeField] private float treeThreshold = 0.6f;

    [Tooltip("Semilla del mundo. Cambia este número para generar mundos diferentes")]
    [SerializeField] private float worldSeed = 42f;

    [Header("Variación visual")]
    [Tooltip("Variación aleatoria en la escala del árbol")]
    [SerializeField] private float scaleVariation = 0.3f;

    [Tooltip("Escala base del árbol (ajusta según tu modelo)")]
    [SerializeField] private float baseScale = 1f;

    // ─── Método público que llama el ChunkManager ─────────────────────────────

    /// <summary>
    /// Genera los árboles de un chunk. Se llama una vez cuando el chunk aparece.
    /// </summary>
    /// <param name="chunkData">Los datos del chunk donde generar árboles</param>
    /// <param name="chunkParent">El GameObject del chunk (los árboles serán sus hijos)</param>
    public void GenerateTrees(ChunkData chunkData, Transform chunkParent)
    {
        if (treePrefab == null)
        {
            Debug.LogWarning("TreeGenerator: No hay prefab de árbol asignado.");
            return;
        }

        int chunkSize = chunkData.cells.GetLength(0);

        for (int x = 0; x < chunkSize; x++)
        {
            for (int z = 0; z < chunkSize; z++)
            {
                CellData cell = chunkData.cells[x, z];

                // Solo ponemos árboles en celdas vacías de tipo Grass
                if (!cell.IsEmpty || cell.terrainType != CellData.TerrainType.Grass)
                    continue;

                // Calculamos el valor de Perlin Noise para esta celda
                // Sumamos worldSeed para que cada mundo se vea diferente
                float noiseValue = Mathf.PerlinNoise(
                    (cell.gridPosition.x + worldSeed) * noiseScale,
                    (cell.gridPosition.y + worldSeed) * noiseScale
                );

                // Si el ruido supera el umbral, colocamos un árbol aquí
                if (noiseValue >= treeThreshold)
                {
                    PlaceTree(cell, chunkParent);
                }
            }
        }
    }

    // ─── Lógica interna ───────────────────────────────────────────────────────

    /// <summary>
    /// Instancia un árbol en la celda indicada y lo registra como ocupante.
    /// </summary>
    private void PlaceTree(CellData cell, Transform parent)
    {
        // Centramos el árbol en la celda (la celda empieza en worldPosition,
        // sumamos 0.5 en X y Z para centrarlo en el cuadrado de 1x1)
        Vector3 spawnPos = cell.worldPosition + new Vector3(0.5f, 0f, 0.5f);

        GameObject tree = Instantiate(treePrefab, spawnPos, Quaternion.identity, parent);

        // Rotación aleatoria en Y para que no todos miren igual
        tree.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

        // Escala aleatoria para variedad visual
        float scale = baseScale + Random.Range(-scaleVariation, scaleVariation);
        tree.transform.localScale = Vector3.one * scale;

        // Registramos el árbol en la celda para que el sistema de construcción
        // y recursos sepan que esta celda está ocupada
        cell.occupant = tree;
    }
}
using UnityEngine;
using System.Collections.Generic;

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
    [SerializeField] private float noiseScale = 0.05f;

    [Tooltip("A partir de qué valor del ruido aparece un árbol (0 a 1). Más alto = menos árboles")]
    [SerializeField] private float treeThreshold = 0.68f;

    [Tooltip("Semilla del mundo. Cambia este número para generar mundos diferentes")]
    [SerializeField] private float worldSeed = 42f;

    [Header("Separación entre árboles")]
    [Tooltip("Distancia mínima en unidades entre árboles. Ajusta según el tamaño visual de tu árbol")]
    [SerializeField] private float minDistanceBetweenTrees = 4f;

    [Header("Variación visual")]
    [Tooltip("Variación aleatoria en la escala del árbol")]
    [SerializeField] private float scaleVariation = 0.2f;

    [Tooltip("Escala base del árbol (ajusta según tu modelo)")]
    [SerializeField] private float baseScale = 1f;

    [Header("Hover")]
    [Tooltip("Material que se aplica cuando el mouse está encima del árbol")]
    [SerializeField] private Material hoverMaterial;

    // Guardamos las posiciones donde ya pusimos árboles en este chunk
    private List<Vector3> placedPositions = new List<Vector3>();

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

        // Limpiamos la lista para cada chunk nuevo
        placedPositions.Clear();

        int chunkSize = chunkData.cells.GetLength(0);

        for (int x = 0; x < chunkSize; x++)
        {
            for (int z = 0; z < chunkSize; z++)
            {
                CellData cell = chunkData.cells[x, z];

                // Solo ponemos árboles en celdas vacías de tipo Grass
                if (!cell.IsEmpty || cell.terrainType != CellData.TerrainType.Grass)
                    continue;

                // 1. Verificar el ruido
                float noiseValue = Mathf.PerlinNoise(
                    (cell.gridPosition.x + worldSeed) * noiseScale,
                    (cell.gridPosition.y + worldSeed) * noiseScale
                );

                if (noiseValue < treeThreshold) continue;

                // 2. Verificar que no haya otro árbol demasiado cerca
                Vector3 candidatePos = cell.worldPosition + new Vector3(0.5f, 0f, 0.5f);

                if (IsTooCloseToOtherTree(candidatePos)) continue;

                // 3. Poner el árbol
                PlaceTree(cell, candidatePos, chunkParent);
            }
        }
    }

    // ─── Lógica interna ───────────────────────────────────────────────────────

    /// <summary>
    /// Revisa si la posición candidata está demasiado cerca de un árbol ya colocado.
    /// </summary>
    private bool IsTooCloseToOtherTree(Vector3 candidatePos)
    {
        foreach (Vector3 placed in placedPositions)
        {
            float dist = Vector2.Distance(
                new Vector2(candidatePos.x, candidatePos.z),
                new Vector2(placed.x, placed.z)
            );

            if (dist < minDistanceBetweenTrees)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Instancia un árbol en la celda indicada, le agrega el componente Tree,
    /// asigna la layer y lo registra como ocupante de la celda.
    /// </summary>
    private void PlaceTree(CellData cell, Vector3 position, Transform parent)
    {
        // Guardamos la posición para que los siguientes árboles respeten la distancia
        placedPositions.Add(position);

        // Pequeño offset aleatorio para que no queden en fila perfecta
        float offsetX = Random.Range(-0.3f, 0.3f);
        float offsetZ = Random.Range(-0.3f, 0.3f);
        Vector3 spawnPos = position + new Vector3(offsetX, 0f, offsetZ);

        GameObject treeGO = Instantiate(treePrefab, spawnPos, Quaternion.identity, parent);

        // Rotación aleatoria en Y para variedad visual
        treeGO.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

        // Escala aleatoria para variedad visual
        float scale = baseScale + Random.Range(-scaleVariation, scaleVariation);
        treeGO.transform.localScale = Vector3.one * scale;

        // Asignamos la layer Tree al raíz Y a todos sus hijos recursivamente
        // Necesario porque el prefab tiene tronco y copa como objetos separados
        SetLayerRecursively(treeGO, LayerMask.NameToLayer("Tree"));

        // Agregamos el componente Tree y le decimos qué celda ocupa
        // para que cuando se destruya pueda liberar la celda
        Tree tree = treeGO.AddComponent<Tree>();
        tree.SetCell(cell);
        tree.SetHoverMaterial(hoverMaterial); // Le pasamos el material de hover

        // Registramos el árbol en la celda
        cell.occupant = treeGO;
    }

    /// <summary>
    /// Asigna una layer a un GameObject y a todos sus hijos recursivamente.
    /// Necesario porque los prefabs de árboles tienen tronco y copa como hijos separados.
    /// </summary>
    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
}
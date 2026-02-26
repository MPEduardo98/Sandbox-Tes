using UnityEngine;
using System.Collections.Generic;

public class TreeGenerator : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject treePrefab;

    [Header("Distribución con Perlin Noise")]
    [SerializeField] private float noiseScale = 0.05f;
    [Tooltip("Más alto = menos árboles")]
    [SerializeField] private float treeThreshold = 0.68f;
    [SerializeField] private float worldSeed = 42f;

    [Header("Separación")]
    [Tooltip("Distancia mínima en unidades entre árboles. Ajusta según el tamaño visual de tu árbol")]
    [SerializeField] private float minDistanceBetweenTrees = 4f;

    [Header("Variación visual")]
    [SerializeField] private float scaleVariation = 0.2f;
    [SerializeField] private float baseScale = 1f;

    // Guardamos las posiciones donde ya pusimos árboles en este chunk
    private List<Vector3> placedPositions = new List<Vector3>();

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

    /// <summary>
    /// Revisa si la posición candidata está demasiado cerca de un árbol ya colocado.
    /// </summary>
    private bool IsTooCloseToOtherTree(Vector3 candidatePos)
    {
        foreach (Vector3 placed in placedPositions)
        {
            // Usamos distancia en el plano XZ (ignoramos Y)
            float dist = Vector2.Distance(
                new Vector2(candidatePos.x, candidatePos.z),
                new Vector2(placed.x, placed.z)
            );

            if (dist < minDistanceBetweenTrees)
                return true;
        }
        return false;
    }

    private void PlaceTree(CellData cell, Transform parent)
    {
        float offsetX = Random.Range(-0.3f, 0.3f);
        float offsetZ = Random.Range(-0.3f, 0.3f);

        Vector3 spawnPos = cell.worldPosition + new Vector3(0.5f + offsetX, 0f, 0.5f + offsetZ);

        GameObject treeGO = Instantiate(treePrefab, spawnPos, Quaternion.identity, parent);
        treeGO.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        treeGO.layer = LayerMask.NameToLayer("Tree");
        
        float scale = baseScale + Random.Range(-scaleVariation, scaleVariation);
        treeGO.transform.localScale = Vector3.one * scale;

        // Agregamos el componente Tree y le decimos qué celda ocupa
        Tree tree = treeGO.AddComponent<Tree>();
        tree.SetCell(cell);

        cell.occupant = treeGO;
    }
}
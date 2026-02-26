using UnityEngine;

/// <summary>
/// Representa una celda individual del grid (1x1 unidad).
/// Guarda qué tipo de terreno es y qué objeto ocupa esa celda.
/// No es un MonoBehaviour — es solo un contenedor de datos (no va en ningún GameObject).
/// </summary>
[System.Serializable]
public class CellData
{
    // ─── Tipo de terreno ──────────────────────────────────────────────────────

    public enum TerrainType
    {
        Grass,  // Pasto normal, se puede caminar y construir
        Water,  // Agua, no se puede caminar (futuro)
        Road    // Camino, más velocidad (futuro)
    }

    // ─── Datos de la celda ────────────────────────────────────────────────────

    /// <summary>Posición de esta celda en el mundo (siempre Y=0)</summary>
    public Vector3 worldPosition;

    /// <summary>Posición en la grilla global (ej: celda 47, 23 del mundo)</summary>
    public Vector2Int gridPosition;

    /// <summary>Qué tipo de suelo es</summary>
    public TerrainType terrainType = TerrainType.Grass;

    /// <summary>
    /// El GameObject que ocupa esta celda (árbol, roca, edificio...).
    /// null significa que está vacía.
    /// </summary>
    public GameObject occupant = null;

    /// <summary>True si no hay nada en la celda y se puede construir/colocar algo</summary>
    public bool IsEmpty => occupant == null;

    // ─── Constructor ──────────────────────────────────────────────────────────

    public CellData(Vector2Int gridPos, Vector3 worldPos)
    {
        gridPosition  = gridPos;
        worldPosition = worldPos;
    }
}
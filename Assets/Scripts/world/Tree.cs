using UnityEngine;

/// <summary>
/// Gestiona la vida, destrucción y efectos visuales del árbol.
/// </summary>
public class Tree : MonoBehaviour
{
    [Header("Vida")]
    [SerializeField] private int maxHealth = 3;

    [Header("Efecto Hover")]
    [Tooltip("Material que se aplica cuando el mouse está encima")]
    [SerializeField] private Material hoverMaterial;

    private int currentHealth;
    private CellData occupiedCell;

    // Guardamos los materiales originales de cada Renderer del árbol
    // Un árbol puede tener varios (tronco, copa, ramas...)
    private Renderer[] renderers;
    private Material[][] originalMaterials;

    void Awake()
    {
        currentHealth = maxHealth;

        // Obtenemos todos los Renderers del árbol y sus hijos
        // (tronco y copa pueden ser objetos separados dentro del prefab)
        renderers = GetComponentsInChildren<Renderer>();

        // Guardamos una copia de los materiales originales de cada Renderer
        originalMaterials = new Material[renderers.Length][];
        for (int i = 0; i < renderers.Length; i++)
        {
            originalMaterials[i] = renderers[i].materials;
        }
    }

    public void SetCell(CellData cell)
    {
        occupiedCell = cell;
    }

    public void SetHoverMaterial(Material mat)
    {
        hoverMaterial = mat;
    }

    // ─── Hover ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Activa el brillo — llamado cuando el mouse entra al árbol.
    /// </summary>
    public void SetHover(bool isHovered)
    {
        if (hoverMaterial == null) return;

        for (int i = 0; i < renderers.Length; i++)
        {
            if (isHovered)
            {
                // Reemplazamos todos los materiales por el hover material
                Material[] hoverMats = new Material[renderers[i].materials.Length];
                for (int j = 0; j < hoverMats.Length; j++)
                    hoverMats[j] = hoverMaterial;

                renderers[i].materials = hoverMats;
            }
            else
            {
                // Restauramos los materiales originales
                renderers[i].materials = originalMaterials[i];
            }
        }
    }

    // ─── Combate ──────────────────────────────────────────────────────────────

    public void TakeHit()
    {
        currentHealth--;

        StopAllCoroutines();
        StartCoroutine(ShakeEffect());

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        if (occupiedCell != null)
            occupiedCell.occupant = null;

        Destroy(gameObject);
    }

    private System.Collections.IEnumerator ShakeEffect()
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0f;
        float duration = 0.2f;
        float magnitude = 0.15f;

        while (elapsed < duration)
        {
            float x = Random.Range(-magnitude, magnitude);
            float z = Random.Range(-magnitude, magnitude);
            transform.localPosition = originalPos + new Vector3(x, 0f, z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }
}
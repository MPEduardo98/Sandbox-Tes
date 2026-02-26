using UnityEngine;

/// <summary>
/// Componente que va en cada árbol del mundo.
/// Gestiona su vida y su destrucción.
/// </summary>
public class Tree : MonoBehaviour
{
    [Header("Vida")]
    [Tooltip("Golpes necesarios para destruir el árbol")]
    [SerializeField] private int maxHealth = 3;

    private int currentHealth;

    // La celda del grid que ocupa este árbol
    // La necesitamos para liberarla cuando el árbol muera
    private CellData occupiedCell;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    /// <summary>
    /// Llamado por TreeGenerator al crear el árbol para registrar qué celda ocupa.
    /// </summary>
    public void SetCell(CellData cell)
    {
        occupiedCell = cell;
    }

    /// <summary>
    /// Recibe un golpe. Si la vida llega a 0, el árbol se destruye.
    /// </summary>
    public void TakeHit()
    {
        currentHealth--;

        // Feedback visual: el árbol se sacude un poco
        StopAllCoroutines();
        StartCoroutine(ShakeEffect());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Destruye el árbol y libera su celda en el grid.
    /// </summary>
    private void Die()
    {
        // Liberamos la celda para que otros sistemas sepan que está vacía
        if (occupiedCell != null)
            occupiedCell.occupant = null;

        Destroy(gameObject);
    }

    /// <summary>
    /// Pequeña animación de sacudida al recibir un golpe.
    /// </summary>
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
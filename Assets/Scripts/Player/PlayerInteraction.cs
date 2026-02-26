using UnityEngine;

/// <summary>
/// Gestiona el hover y el clic del jugador sobre objetos interactuables.
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float interactionRange = 5f;
    [SerializeField] private LayerMask interactableLayer;

    private Camera mainCamera;
    private Tree currentHoveredTree; // El árbol que tiene el hover activo ahora mismo

    void Awake()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        HandleHover();

        if (Input.GetMouseButtonDown(0))
            TryInteract();
    }

    // ─── Hover ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Cada frame lanzamos un rayo desde el mouse.
    /// Si apunta a un árbol dentro del rango → activamos hover.
    /// Si apunta a otro lado → desactivamos hover del árbol anterior.
    /// </summary>
    private void HandleHover()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Tree detectedTree = null;

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, interactableLayer))
        {
            float distance = Vector3.Distance(transform.position, hit.point);

            if (distance <= interactionRange)
            {
                detectedTree = hit.collider.GetComponentInParent<Tree>();
            }
        }

        // Si el árbol detectado cambió, actualizamos el hover
        if (detectedTree != currentHoveredTree)
        {
            // Apagamos el hover del árbol anterior
            if (currentHoveredTree != null)
                currentHoveredTree.SetHover(false);

            // Encendemos el hover del árbol nuevo
            currentHoveredTree = detectedTree;
            if (currentHoveredTree != null)
                currentHoveredTree.SetHover(true);
        }
    }

    // ─── Interacción ──────────────────────────────────────────────────────────

    private void TryInteract()
    {
        if (currentHoveredTree != null)
        {
            currentHoveredTree.TakeHit();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
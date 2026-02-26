using UnityEngine;

/// <summary>
/// Gestiona el clic del jugador sobre objetos interactuables del mundo.
/// Este script va en el Player.
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Distancia máxima desde la que puede interactuar con un árbol")]
    [SerializeField] private float interactionRange = 5f;

    [Tooltip("Capas con las que puede interactuar (selecciona la layer de los árboles)")]
    [SerializeField] private LayerMask interactableLayer;

    private Camera mainCamera;

    void Awake()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Detectamos clic izquierdo del mouse
        if (Input.GetMouseButtonDown(0))
        {
            TryInteract();
        }
    }

    private void TryInteract()
    {
        // Lanzamos un rayo desde la cámara hacia donde hizo clic el mouse
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, interactableLayer))
        {
            // ¿Está el objeto dentro del rango de interacción?
            float distance = Vector3.Distance(transform.position, hit.point);

            if (distance <= interactionRange)
            {
                // ¿El objeto tiene el componente Tree?
                Tree tree = hit.collider.GetComponentInParent<Tree>();
                if (tree != null)
                {
                    tree.TakeHit();
                }
            }
            else
            {
                Debug.Log("Árbol demasiado lejos — acércate más");
            }
        }
    }

    /// <summary>
    /// Dibuja el rango de interacción en la vista Scene para depuración.
    /// Solo visible en el editor, no en el juego final.
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Objetivo")]
    [SerializeField] private Transform target;

    [Header("Posición de la Cámara")]
    [SerializeField] private float distance = 20f;
    [SerializeField] private float verticalAngle = 50f;

    [Tooltip("Qué tan suave sigue al jugador. Más alto = más pegada al jugador")]
    [SerializeField] private float smoothSpeed = 6f;

    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 4f;
    [SerializeField] private float minDistance = 8f;
    [SerializeField] private float maxDistance = 40f;
    [SerializeField] private float zoomSmoothSpeed = 8f;

    private float targetDistance;
    private Vector3 currentVelocity; // Usado por SmoothDamp

    void Start()
    {
        targetDistance = distance;
        transform.rotation = Quaternion.Euler(verticalAngle, 45f, 0f);

        // Posicionamos la cámara instantáneamente al inicio
        // para que no haga el efecto de "volar hasta su posición"
        if (target != null)
            transform.position = GetTargetPosition();
    }

    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            targetDistance -= scroll * zoomSpeed * targetDistance;
            targetDistance = Mathf.Clamp(targetDistance, minDistance, maxDistance);
        }

        distance = Mathf.Lerp(distance, targetDistance, zoomSmoothSpeed * Time.deltaTime);
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = GetTargetPosition();

        // SmoothDamp es más estable que Lerp para seguimiento de cámara
        // No produce el efecto de "rebote" que tenías antes
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref currentVelocity,
            1f / smoothSpeed  // tiempo para llegar al destino
        );
    }

    /// <summary>
    /// Calcula la posición ideal de la cámara en base a la posición del jugador.
    /// </summary>
    private Vector3 GetTargetPosition()
    {
        Vector3 offset = Quaternion.Euler(verticalAngle, 45f, 0f) * new Vector3(0, 0, -distance);
        return target.position + offset;
    }
}
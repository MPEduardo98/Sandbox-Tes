using UnityEngine;

/// <summary>
/// Hace que la cámara siga al jugador manteniendo siempre
/// el mismo ángulo y distancia isométrica (como Age of Empires).
/// Este script va en la Main Camera.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    // ─── Configuración desde el Inspector ────────────────────────────────────

    [Header("Objetivo")]
    [Tooltip("Arrastra aquí el Transform del Player")]
    [SerializeField] private Transform target;

    [Header("Posición de la Cámara")]
    [Tooltip("Qué tan lejos está la cámara del jugador")]
    [SerializeField] private float distance = 20f;

    [Tooltip("Ángulo vertical de la cámara (45-60 es isométrico clásico)")]
    [SerializeField] private float verticalAngle = 50f;

    [Tooltip("Qué tan suave sigue al jugador (más alto = más suave pero más lento)")]
    [SerializeField] private float smoothSpeed = 10f;

    // ─── Variables internas ───────────────────────────────────────────────────

    private Vector3 offset; // La distancia fija entre cámara y jugador

    // ─── Unity Messages ───────────────────────────────────────────────────────

    void Start()
    {
        // Calculamos el offset una sola vez basándonos en el ángulo isométrico
        // Esto posiciona la cámara arriba y detrás del jugador
        offset = Quaternion.Euler(verticalAngle, 45f, 0f) * new Vector3(0, 0, -distance);

        // Apuntamos la cámara hacia donde mirará siempre
        transform.rotation = Quaternion.Euler(verticalAngle, 45f, 0f);
    }

    void LateUpdate()
    {
        // LateUpdate corre DESPUÉS de Update — ideal para cámaras
        // porque el jugador ya se movió cuando la cámara se actualiza

        if (target == null) return; // Protección por si no hay jugador asignado

        // Posición ideal de la cámara = posición del jugador + offset fijo
        Vector3 targetPosition = target.position + offset;

        // Lerp = interpolación suave entre posición actual y posición ideal
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
    }
}
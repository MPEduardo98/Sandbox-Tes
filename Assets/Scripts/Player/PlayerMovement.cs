using UnityEngine;
using UnityEngine.InputSystem; // Nuevo Input System de Unity

/// <summary>
/// Mueve al jugador con WASD usando el nuevo Input System.
/// Este script va en el GameObject "Player" que tiene un Rigidbody.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    // ─── Configuración desde el Inspector ────────────────────────────────────

    [Header("Movimiento")]
    [Tooltip("Qué tan rápido se mueve el jugador")]
    [SerializeField] private float moveSpeed = 8f;

    [Tooltip("Qué tan rápido frena el jugador al soltar las teclas")]
    [SerializeField] private float drag = 8f;

    // ─── Referencias internas ─────────────────────────────────────────────────

    private Rigidbody rb;
    private Vector2 inputDirection; // Lo que el jugador está presionando (X y Z)

    // ─── Unity Messages ───────────────────────────────────────────────────────

    void Awake()
    {
        // Obtenemos el Rigidbody una sola vez aquí (nunca en Update)
        rb = GetComponent<Rigidbody>();
        rb.linearDamping = drag; // Le decimos al Rigidbody cuánto frena solo
    }

    /// <summary>
    /// El nuevo Input System llama a este método automáticamente cuando
    /// el jugador presiona o suelta las teclas WASD o las flechas.
    /// El nombre DEBE ser "OnMove" para que el sistema lo encuentre.
    /// </summary>
    public void OnMove(InputValue value)
    {
        // value.Get<Vector2>() nos da un vector como (1,0) para derecha, (-1,0) para izquierda, etc.
        inputDirection = value.Get<Vector2>();
    }

    void FixedUpdate()
    {
        // FixedUpdate se usa para física — corre a 50 veces por segundo de forma constante
        MovePlayer();
    }

    // ─── Lógica de movimiento ─────────────────────────────────────────────────

    /// <summary>
    /// Aplica fuerza al Rigidbody en la dirección que el jugador está presionando.
    /// Usamos el plano XZ porque Y es arriba/abajo (la gravedad lo maneja Unity).
    /// </summary>
    private void MovePlayer()
    {
        // Convertimos el input 2D (X, Y del joystick/teclado) al plano 3D (X, Z del mundo)
        // inputDirection.x = izquierda/derecha → mueve en X
        // inputDirection.y = arriba/abajo del teclado → mueve en Z (profundidad)
        Vector3 moveDirection = new Vector3(inputDirection.x, 0f, inputDirection.y);

        // AddForce empuja al Rigidbody. El drag que pusimos antes frena automáticamente.
        rb.AddForce(moveDirection.normalized * moveSpeed, ForceMode.VelocityChange);

        // Limitamos la velocidad máxima para que no acelere infinitamente
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (horizontalVelocity.magnitude > moveSpeed)
        {
            Vector3 clamped = horizontalVelocity.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(clamped.x, rb.linearVelocity.y, clamped.z);
        }
    }
}
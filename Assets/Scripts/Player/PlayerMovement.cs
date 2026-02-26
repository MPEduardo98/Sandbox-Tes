using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Movimiento suave del jugador relativo a la cámara isométrica.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    [Tooltip("Velocidad máxima del jugador")]
    [SerializeField] private float moveSpeed = 6f;

    [Tooltip("Qué tan rápido acelera (más alto = más responsivo)")]
    [SerializeField] private float acceleration = 12f;

    [Tooltip("Qué tan rápido frena al soltar las teclas")]
    [SerializeField] private float deceleration = 10f;

    [Header("Salto")]
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Cámara")]
    [Tooltip("Arrastra aquí el Transform de la Main Camera")]
    [SerializeField] private Transform cameraTransform;

    // ─── Variables internas ───────────────────────────────────────────────────

    private Rigidbody rb;
    private Vector2 inputDirection;
    private bool isGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearDamping = 0f; // El drag lo manejamos nosotros manualmente
    }

    void Update()
    {
        CheckGrounded();
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    public void OnMove(InputValue value)
    {
        inputDirection = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void MovePlayer()
    {
        // ── Dirección relativa a la cámara ────────────────────────────────────
        // Tomamos los ejes de la cámara pero los aplanamos en Y=0
        // para que el movimiento sea siempre horizontal sin importar el ángulo
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight   = cameraTransform.right;

        camForward.y = 0f;
        camRight.y   = 0f;

        camForward.Normalize();
        camRight.Normalize();

        // Combinamos input con los ejes de la cámara
        Vector3 desiredDirection = (camForward * inputDirection.y + camRight * inputDirection.x);
        Vector3 desiredVelocity  = desiredDirection * moveSpeed;

        // ── Aceleración / Deceleración suave ─────────────────────────────────
        Vector3 currentHorizontal = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        float lerpSpeed;

        if (inputDirection.magnitude > 0.1f)
        {
            // El jugador está presionando una tecla → acelerar
            lerpSpeed = acceleration * Time.fixedDeltaTime;
        }
        else
        {
            // No hay input → frenar suavemente
            lerpSpeed = deceleration * Time.fixedDeltaTime;
            desiredVelocity = Vector3.zero;
        }

        Vector3 newHorizontal = Vector3.Lerp(currentHorizontal, desiredVelocity, lerpSpeed);

        // Aplicamos la velocidad manteniendo la Y (gravedad y salto intactos)
        rb.linearVelocity = new Vector3(newHorizontal.x, rb.linearVelocity.y, newHorizontal.z);
    }

    private void CheckGrounded()
    {
        isGrounded = Physics.Raycast(
            transform.position,
            Vector3.down,
            1f + groundCheckDistance,
            groundLayer
        );
    }
}
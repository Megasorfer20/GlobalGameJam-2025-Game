using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CharacterMovement : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float moveSpeed = 5f; // Velocidad de movimiento del personaje

    [Header("Configuración de Dash")]
    public float dashForce = 15f;    // Fuerza del dash
    public float dashDuration = 0.2f; // Duración del dash
    public float dashCooldown = 1f;   // Tiempo de espera entre dashes
    private bool canDash = true;
    private bool isDashing = false;
    private float dashTimer;
    private float dashCooldownTimer;

    private Rigidbody rb3D;
    private Vector2 currentMoveInput; // Almacena la dirección de movimiento (Vector2 para el input)

    void Awake()
    {
        rb3D = GetComponent<Rigidbody>();
        rb3D.freezeRotation = true; // Congelar rotación en X, Y y Z.
    }

    /// <summary>
    /// Establece la dirección de movimiento para el personaje.
    /// </summary>
    public void SetMoveInput(Vector2 inputDirection)
    {
        // Si el personaje está en dash, el input del jugador no lo debe mover.
        if (!isDashing)
        {
            currentMoveInput = inputDirection.normalized;
        }
    }

    /// <summary>
    /// Inicia un Dash en la dirección actual de movimiento.
    /// </summary>
    public void Dash()
    {
        if (canDash)
        {
            canDash = false;
            isDashing = true;
            dashTimer = dashDuration;
            dashCooldownTimer = dashCooldown;

            // Determinar la dirección del dash.
            // Si hay input, dash en esa dirección. Si no, dash hacia adelante del personaje.
            Vector3 dashDirection;
            if (currentMoveInput != Vector2.zero)
            {
                dashDirection = new Vector3(currentMoveInput.x, 0f, currentMoveInput.y);
            }
            else
            {
                // Si no hay input de movimiento, dash en la dirección en la que está mirando el personaje
                dashDirection = transform.forward;
                // Ajuste para 2.5D: Asegurarse de que el dash sea en el plano XZ
                dashDirection.y = 0;
                if (dashDirection.magnitude > 0) dashDirection.Normalize();
                else dashDirection = transform.right; // Si no hay forward, dash a la derecha
            }

            rb3D.linearVelocity = dashDirection.normalized * dashForce; // Aplicar fuerza instantánea
            // Debug.Log($"{gameObject.name} dashed in direction {dashDirection.normalized} with force {dashForce}");
        }
    }

    void FixedUpdate()
    {
        HandleDash(); // Primero maneja el dash para que tenga prioridad

        if (!isDashing)
        {
            // Movimiento normal si no está en dash
            Vector3 moveVector = new Vector3(currentMoveInput.x, 0f, currentMoveInput.y);
            rb3D.linearVelocity = moveVector * moveSpeed;

            // Opcional: Rotación del personaje
            if (moveVector != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveVector);
                rb3D.transform.rotation = Quaternion.Slerp(rb3D.transform.rotation, targetRotation, Time.fixedDeltaTime * 10f);
            }
        }
    }

    void HandleDash()
    {
        if (isDashing)
        {
            dashTimer -= Time.fixedDeltaTime;
            if (dashTimer <= 0)
            {
                isDashing = false;
                // Opcional: Aquí podrías restablecer la velocidad si quieres una parada abrupta después del dash
                // rb3D.velocity = Vector3.zero;
            }
        }

        if (!canDash)
        {
            dashCooldownTimer -= Time.fixedDeltaTime;
            if (dashCooldownTimer <= 0)
            {
                canDash = true;
            }
        }
    }
}
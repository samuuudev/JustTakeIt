using UnityEngine;

/// <summary>
/// Controla el movimiento del jugador: caminar, saltar y apuntar/disparar el arma de retroceso.
/// Requiere un Rigidbody2D en el mismo GameObject.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    // --- Componentes ---
    private Rigidbody2D rb;

    [Header("Materiales de física")]
    [Tooltip("Material sin fricción para evitar que el jugador se pegue a las paredes en el aire")]
    [SerializeField] private PhysicsMaterial2D noFriction;
    [Tooltip("Material con fricción normal para cuando el jugador está en el suelo")]
    [SerializeField] private PhysicsMaterial2D defaultFriction;

    [Header("Estadísticas del jugador")]
    [Tooltip("Velocidad horizontal del jugador")]
    [SerializeField] private float speed = 5f;

    [Header("Salto")]
    [Tooltip("Fuerza del salto aplicada como impulso")]
    [SerializeField] private float jumpForce = 5f;
    [Tooltip("Capa(s) que el Raycast considera como suelo")]
    [SerializeField] private LayerMask whatIsGround;
    [Tooltip("Distancia del Raycast hacia abajo para detectar el suelo")]
    [SerializeField] private float groundDistance = 0.2f;
    [Tooltip("Objeto hijo desde donde se lanza el Raycast hacia abajo")]
    [SerializeField] private GameObject groundCheck;
    [Tooltip("Multiplicador de fuerza del retroceso del arma en el aire")]
    [SerializeField] private float airMultiplier = 4f;
    [SerializeField] private bool isGrounded;

    [Header("Arma de retroceso")]
    [Tooltip("Transform hijo que rota apuntando hacia el ratón (pivote del arma)")]
    [SerializeField] private Transform handleRotation;
    [Tooltip("Munición máxima del arma (se recarga al tocar el suelo)")]
    [SerializeField] private int gunAmmo = 3;
    private int currentGunAmmo;

    /// <summary>
    /// Awake se ejecuta antes que Start. Obtenemos el componente Rigidbody2D.
    /// </summary>
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentGunAmmo = gunAmmo;
    }

    /// <summary>
    /// Update se ejecuta cada frame. Desde aquí llamamos a toda la lógica del jugador.
    /// </summary>
    private void Update()
    {
        // Si el juego ha terminado, no hacer nada
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver())
            return;

        // Cambiamos el material de física según si estamos en el suelo o en el aire.
        // Esto evita que el jugador se "pegue" a las paredes al saltar.
        rb.sharedMaterial = isGrounded ? defaultFriction : noFriction;

        HandleMovement();
        HandleLook();
        Jump();
    }

    /// <summary>
    /// Mueve al jugador horizontalmente usando el eje "Horizontal" (A/D o flechas).
    /// La velocidad vertical del Rigidbody se mantiene intacta para no afectar saltos/caídas.
    /// </summary>
    private void HandleMovement()
    {
        float hor = Input.GetAxis("Horizontal");

        // Si no hay input horizontal, no modificamos la velocidad
        if (hor == 0f)
            return;

        rb.linearVelocity = new Vector2(hor * speed, rb.linearVelocity.y);
    }

    /// <summary>
    /// Gestiona el salto del jugador. Usa un Raycast hacia abajo desde el groundCheck
    /// para detectar si el jugador está tocando el suelo.
    /// </summary>
    private void Jump()
    {
        // Solo podemos saltar si estamos en el suelo y pulsamos el botón de salto
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        // Raycast: lanza un rayo invisible hacia abajo para comprobar si hay suelo
        RaycastHit2D hit = Physics2D.Raycast(
            groundCheck.transform.position,  // Origen del rayo
            Vector2.down,                     // Dirección: hacia abajo
            groundDistance,                   // Distancia máxima del rayo
            whatIsGround                      // Solo detecta objetos en esta capa
        );

        isGrounded = hit.collider != null;
    }

    /// <summary>
    /// Rota el pivote del arma hacia la posición del ratón y gestiona el disparo.
    /// El "disparo" no lanza proyectiles: aplica una fuerza de retroceso al jugador
    /// en la dirección contraria a donde apunta, permitiendo impulsarse por el aire.
    /// </summary>
    private void HandleLook()
    {
        // Convertimos la posición del ratón en pantalla a coordenadas del mundo
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - transform.position).normalized;

        // Calculamos el ángulo en grados para rotar el pivote del arma
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        handleRotation.rotation = Quaternion.Euler(0f, 0f, angle);

        // Al hacer clic izquierdo, si tenemos munición, aplicamos retroceso
        if (Input.GetKeyDown(KeyCode.Mouse0) && currentGunAmmo > 0)
        {
            // La fuerza se aplica en dirección CONTRARIA a donde apunta el arma
            rb.AddForce(direction * (-jumpForce * airMultiplier), ForceMode2D.Impulse);
            currentGunAmmo--;
        }

        // La munición se recarga automáticamente al tocar el suelo
        if (isGrounded)
        {
            currentGunAmmo = gunAmmo;
        }
    }

    /// <summary>
    /// Método llamado cuando el jugador colisiona con un enemigo o trampa.
    /// Se activa con triggers (Collider2D marcado como "Is Trigger").
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si tocamos un objeto con tag "Enemy" o "Spike", morimos
        if (collision.CompareTag("Enemy") || collision.CompareTag("Spike"))
        {
            Die();
        }
    }

    /// <summary>
    /// Gestiona la muerte del jugador: desactiva el objeto y llama al Game Over.
    /// </summary>
    private void Die()
    {
        // Notificamos al GameManager que el juego ha terminado
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }

        // Desactivamos el sprite del jugador (desaparece de la pantalla)
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Dibuja el rayo del groundCheck en la vista Scene (solo visible en el editor,
    /// no aparece en el juego compilado). Útil para depurar.
    /// </summary>
    private void OnDrawGizmos()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(
            groundCheck.transform.position,
            groundCheck.transform.position + Vector3.down * groundDistance
        );
    }
}
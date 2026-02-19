using UnityEngine;

/// <summary>
/// Comportamiento básico de un enemigo (fantasma).
/// Se mueve en patrulla horizontal entre dos puntos.
/// El jugador muere al tocarlo (gestionado desde PlayerMovement).
/// Requiere un Collider2D marcado como "Is Trigger" y el tag "Enemy".
/// </summary>
public class Enemy : MonoBehaviour
{
    [Header("Patrulla")]
    [Tooltip("Velocidad de movimiento del enemigo")]
    [SerializeField] private float speed = 2f;
    [Tooltip("Distancia que recorre en cada dirección desde su posición inicial")]
    [SerializeField] private float patrolDistance = 3f;

    // Posición inicial del enemigo (se guarda al empezar)
    private Vector3 startPosition;
    // Dirección actual del movimiento (1 = derecha, -1 = izquierda)
    private int direction = 1;

    /// <summary>
    /// Start se ejecuta al inicio. Guardamos la posición donde se colocó el enemigo.
    /// </summary>
    private void Start()
    {
        startPosition = transform.position;
    }

    /// <summary>
    /// Update se ejecuta cada frame. Movemos al enemigo de lado a lado.
    /// </summary>
    private void Update()
    {
        // Si el juego terminó, el enemigo se detiene
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver())
            return;

        // Movemos al enemigo en la dirección actual
        transform.Translate(Vector2.right * (direction * speed * Time.deltaTime));

        // Si se aleja demasiado de su punto de inicio, cambia de dirección
        if (Vector2.Distance(startPosition, transform.position) >= patrolDistance)
        {
            direction *= -1; // Invertimos la dirección

            // Volteamos el sprite para que mire hacia donde camina
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }
}
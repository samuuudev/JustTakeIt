using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Controla la cámara para que siga al jugador con un ligero desplazamiento
/// hacia la dirección del ratón, dando una sensación de "mirar adelante".
/// Se asigna al GameObject de la Main Camera.
/// </summary>
public class CameraController : MonoBehaviour
{
    // Los headers se usan para organizar las variables en el inspector de Unity, haciendo más fácil su edición.
    [Header("Referencias")]
    [Tooltip("Transform del jugador al que la cámara debe seguir")] // Los tooltips se usan para mostrar información en el inspector de unity
    public Transform player;
    [SerializeField] private float followSpeedX = 0.1f; // Velocidad de seguimiento horizontal (0-1, más bajo = más suave)
    [SerializeField] private float followSpeedY = 0.05f; // Velocidad de seguimiento vertical (0-1, más bajo = más suave)

    [SerializeField] private float maxCameraDistance = 10f; // Distancia máxima que la cámara puede alejarse del jugador (para evitar que se pierda de vista)
    [SerializeField] private float minCameraDistance = 5f; // Distancia mínima que la cámara debe mantener del jugador (para evitar que se acerque demasiado)
    [SerializeField] private float currentCameraDistance; // Distancia actual de la cámara al jugador (se ajusta dinámicamente)
    [SerializeField] private LayerMask groundMask; // Capa que representa el suelo para calcular la distancia de la cámara
    [SerializeField] private float mouseInfluence = 2f; // 1 = actual, >1 mueve más la cámara
    
    // Referencia cacheada a la cámara principal
    private Camera _camera;

    /// <summary>
    /// Start se ejecuta una vez al inicio. Guardamos la referencia a la cámara.
    /// </summary>
    void Start()
    {
        _camera = Camera.main;
    }

    /// <summary>
    /// Update se ejecuta cada frame. Calculamos la posición objetivo de la cámara
    /// y la movemos suavemente con Lerp.
    /// </summary>
    void Update()
    {
        // Si el jugador ha sido destruido/desactivado, retornamos por que no queremos errores de referencia
        if (player == null) return;

        // En el caso de que el juego ha terminado, dejamos la cámara quieta
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver())
            return;

        // Convertimos la posición del ratón en pantalla a posición en el mundo
        Vector3 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);

        // Calculamos un offset normalizado desde el jugador hacia el ratón
        // un offset es una distancia o diferencia que se aplica respecto a un punto de referencia.
        Vector3 offset = (mousePos - player.position).normalized * mouseInfluence;

        // La posición objetivo es el jugador + el offset (Z=-10 para que la cámara vea la escena)
        Vector3 targetPos = new Vector3(
            player.position.x + offset.x,
            player.position.y + offset.y,
            -10f
        );
    
        // Cambiamos la distancia de la camara al jugador segun su altura.
        float targetSize = CalculateCameraDistance();
        // Debug.Log(targetSize);
        _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, targetSize, 0.05f);
        
        // Lerp: movimiento suave. 0.1f controla la velocidad (más bajo = más suave)
        float newX = Mathf.Lerp(_camera.transform.position.x, targetPos.x, followSpeedX);
        float newY = Mathf.Lerp(_camera.transform.position.y, targetPos.y, followSpeedY);
        _camera.transform.position = new Vector3(newX, newY, -10f);
    }
    
    private float CalculateCameraDistance()
    {
        float cameraDistance = minCameraDistance;
        // calculo la distancia respecto al suelo (sobre la capa ground) usando un raycast desde la posición del jugador hacia abajo
        RaycastHit2D hit = Physics2D.Raycast(player.position, Vector2.down, math.INFINITY, groundMask);

        if (hit.collider != null)
        {
            // si el tamaño de la camra ortográfica es menor a la distancia al suelo, ajustamos la distancia minima
            float distanceToGround = hit.distance;
            cameraDistance = Mathf.Clamp(distanceToGround, minCameraDistance, maxCameraDistance);
        }
        
        return cameraDistance;
    }
    
    private void OnDrawGizmos()
    {
        // calculo la distancia respecto al suelo (sobre la capa ground) usando un raycast desde la posición del jugador hacia abajo
            RaycastHit2D hit = Physics2D.Raycast(player.position, Vector2.down, math.INFINITY, groundMask);
            float distanceToGround = hit.collider != null ? hit.distance : 0f;
        // Dibuja un rayo hacia abajo desde el jugador para visualizar la distancia al suelo en el editor
        if (player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(player.position, player.position + Vector3.down * distanceToGround);
        }
    }
}
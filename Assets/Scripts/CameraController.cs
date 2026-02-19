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
        Vector3 offset = (mousePos - player.position).normalized;

        // La posición objetivo es el jugador + el offset (Z=-10 para que la cámara vea la escena)
        Vector3 targetPos = new Vector3(
            player.position.x + offset.x,
            player.position.y + offset.y,
            -10f
        );

        // Lerp: movimiento suave. 0.1f controla la velocidad (más bajo = más suave)
        _camera.transform.position = Vector3.Lerp(
            _camera.transform.position,
            targetPos,
            0.1f
        );
    }
}
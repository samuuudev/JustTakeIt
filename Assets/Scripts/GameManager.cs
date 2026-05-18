using MelenitasDev.SoundsGood;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// GameManager es un Singleton que controla el estado general del juego:
/// puntuación, tiempo, estados de juego (jugando / game over) y la UI.
/// </summary>
public class GameManager : MonoBehaviour
{
    // --- Singleton ---
    // El patrón Singleton asegura que solo exista UNA instancia de GameManager
    // en toda la escena, accesible desde cualquier script con GameManager.Instance
    public static GameManager Instance;

    // --- Estado del juego ---
    private int score = 0;            // Monedas recogidas
    private float elapsedTime = 0f;   // Tiempo transcurrido en segundos
    private bool isGameOver = false;  // ¿Ha terminado la partida?

    // --- Referencias UI (se asignan desde el Inspector) ---
    [Header("UI - HUD (durante el juego)")]
    [SerializeField] private TextMeshProUGUI scoreText;       // Texto de monedas en pantalla
    [SerializeField] private TextMeshProUGUI timerText;       // Texto del cronómetro en pantalla
    
    
    [Header("UI - Pantalla Clasificación")]
    [SerializeField] private GameObject leaderboardPanel;     // Panel del leaderboard (canvas overlay

    [Header("UI - Pantalla Game Over")]
    [SerializeField] private GameObject gameOverPanel;        // Panel que se muestra al morir
    [SerializeField] private TextMeshProUGUI finalScoreText;  // Puntuación final
    [SerializeField] private TextMeshProUGUI finalTimeText;   // Tiempo final
    [SerializeField] private string menuSceneName = "Menu"; // Nombre de la escena del menú principal

    private Music music;
    
    /// <summary>
    /// Awake se ejecuta antes que Start. Aquí configuramos el Singleton.
    /// </summary>
    private void Awake()
    {
        music = new Music(Track.musicLoop)
            .SetLoop(true)
            .SetVolume(AudioSettingsManager.Instance.CurrentMusicVolumeLinear)
            .SetOutput(Output.Music);
        
        // Si ya existe una instancia, destruimos este duplicado
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Start se ejecuta una vez al inicio. Inicializamos la UI.
    /// </summary>
    private void Start()
    {
        // Nos aseguramos de que el juego esté en marcha
        Time.timeScale = 1f;
        isGameOver = false;

        // Ocultamos el panel de Game Over al empezar
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        
        // Ocultamos el panel de la clasificacion al empezar
        if (leaderboardPanel != null)
            leaderboardPanel.SetActive(false);
        

        // Actualizamos la UI con los valores iniciales
        UpdateScoreUI();
        UpdateTimerUI();
        
        // reproduce la música de fondo
        music.Play();
    }

    /// <summary>
    /// Update se ejecuta cada frame. Aquí actualizamos el cronómetro.
    /// </summary>
    private void Update()
    {
        // Solo contamos el tiempo si el juego NO ha terminado
        if (!isGameOver)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerUI();
        }
    }

    /// <summary>
    /// Añade un punto a la puntuación. Llamado por las monedas al recogerlas.
    /// </summary>
    public void AddPoint()
    {
        if (isGameOver) return; // No sumar puntos si el juego terminó

        score++;
        UpdateScoreUI();
    }
    public void TakeAllPoints()
    {
        if (isGameOver) return; // No restar puntos si el juego terminó
        
        if (score >= 23)
            GameOver();
        
        UpdateScoreUI();
    }
    /// <summary>
    /// Activa el estado de Game Over. Llamado cuando el jugador muere.
    /// </summary>
    public void GameOver()
    {
        if (isGameOver) return; // Evitar llamar dos veces

        isGameOver = true;

        // Congelamos el juego (todo se pausa: física, animaciones, etc.)
        Time.timeScale = 0f;

        // Mostramos la pantalla de Game Over con los datos finales
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);

            if (finalScoreText != null)
                finalScoreText.text = "Monedas: " + score + " / 23";

            if (finalTimeText != null)
                finalTimeText.text = "Tiempo: " + FormatTime(elapsedTime);
        }
        music.Stop();
    }

    /// <summary>
    /// Reinicia la escena actual. Se conecta al botón "Reintentar".
    /// </summary>
    public void RestartGame()
    {
        // Restauramos el tiempo antes de recargar (importante porque lo pusimos a 0)
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Devuelve si el juego ha terminado. Útil para que otros scripts
    /// dejen de funcionar cuando el jugador muere.
    /// </summary>
    public bool IsGameOver()
    {
        return isGameOver;
    }

    // --- Métodos privados de UI ---

    /// <summary>
    /// Actualiza el texto de puntuación en el HUD.
    /// </summary>
    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Monedas: " + score + " / 23";
    }

    /// <summary>
    /// Actualiza el texto del cronómetro en el HUD.
    /// </summary>
    private void UpdateTimerUI()
    {
        if (timerText != null)
            timerText.text = FormatTime(elapsedTime);
    }

    /// <summary>
    /// Convierte segundos a formato "MM:SS" legible.
    /// Ejemplo: 125.7 segundos → "02:05"
    /// </summary>
    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    
    public void switchToMainMenu()
    {
        Time.timeScale = 1f; // Aseguramos que el tiempo esté normalizado
        SceneManager.LoadScene(menuSceneName);
    }

    /// <summary>
    /// Devuelve la puntuación actual (monedas recogidas).
    /// </summary>
    public int GetScore() => score;

    /// <summary>
    /// Devuelve el tiempo transcurrido en segundos.
    /// </summary>
    public float GetElapsedTime() => elapsedTime;
}
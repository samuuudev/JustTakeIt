using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// LeaderboardUI gestiona la pantalla del Top 10.
/// Muestra las mejores puntuaciones obtenidas desde Supabase.
/// 
/// CONFIGURACIÓN EN EL INSPECTOR:
/// - Leaderboard Panel: panel/contenedor principal del leaderboard
/// - Entry Container: el Transform padre donde se instanciarán las filas
/// - Entry Prefab: prefab de cada fila del leaderboard (ver guía abajo)
/// - Loading Text: texto "Cargando..." que se muestra mientras se obtienen datos
/// - Error Text: texto que se muestra si hay error de conexión
/// - Close Button: botón para cerrar el leaderboard
/// </summary>
public class LeaderboardUI : MonoBehaviour
{
    [Header("Panel principal")]
    [Tooltip("Panel/GameObject que contiene todo el leaderboard")]
    [SerializeField] private GameObject leaderboardPanel;

    [Header("Contenedor de entradas")]
    [Tooltip("Transform padre donde se instanciarán las filas del top 10")]
    [SerializeField] private Transform entryContainer;

    [Tooltip("Prefab de una fila del leaderboard (ver guía de configuración)")]
    [SerializeField] private GameObject entryPrefab;

    [Header("Estados")]
    [Tooltip("Texto que aparece mientras se cargan los datos")]
    [SerializeField] private TextMeshProUGUI loadingText;

    [Tooltip("Texto que aparece si hay un error")]
    [SerializeField] private TextMeshProUGUI errorText;

    [Header("Botón cerrar")]
    [Tooltip("Botón para cerrar el panel del leaderboard")]
    [SerializeField] private Button closeButton;

    private void Start()
    {
        // Ocultar el panel al inicio
        if (leaderboardPanel != null)
            leaderboardPanel.SetActive(false);

        // Configurar el botón de cerrar
        if (closeButton != null)
            closeButton.onClick.AddListener(HideLeaderboard);
    }

    /// <summary>
    /// Muestra el leaderboard y carga los datos desde Supabase.
    /// Conectar este método a un botón "Ver Top 10".
    /// </summary>
    public void ShowLeaderboard()
    {
        if (leaderboardPanel != null)
            leaderboardPanel.SetActive(true);

        // Limpiar entradas anteriores
        ClearEntries();

        // Mostrar estado de carga
        SetLoadingState(true);
        SetErrorState(false, "");

        // Pedir el Top 10 a Supabase
        if (SupabaseManager.Instance != null)
        {
            SupabaseManager.Instance.GetTop10(OnTop10Received, OnTop10Error);
        }
        else
        {
            SetLoadingState(false);
            SetErrorState(true, "SupabaseManager no encontrado en la escena");
        }
    }

    /// <summary>
    /// Oculta el panel del leaderboard.
    /// </summary>
    public void HideLeaderboard()
    {
        if (leaderboardPanel != null)
            leaderboardPanel.SetActive(false);
    }

    /// <summary>
    /// Callback cuando se reciben los datos del Top 10.
    /// </summary>
    private void OnTop10Received(List<LeaderboardEntry> entries)
    {
        SetLoadingState(false);

        if (entries == null || entries.Count == 0)
        {
            SetErrorState(true, "No hay puntuaciones todavía");
            return;
        }

        // Crear una fila por cada entrada
        for (int i = 0; i < entries.Count; i++)
        {
            CreateEntryRow(i + 1, entries[i]);
        }
    }

    /// <summary>
    /// Callback cuando hay un error al obtener el Top 10.
    /// </summary>
    private void OnTop10Error(string error)
    {
        SetLoadingState(false);
        SetErrorState(true, "Error al cargar el leaderboard");
        Debug.LogError(error);
    }

    /// <summary>
    /// Crea una fila visual para una entrada del leaderboard.
    /// El prefab debe tener 4 TextMeshProUGUI hijos con estos nombres:
    /// - RankText (posición: #1, #2...)
    /// - UsernameText (nombre del jugador)
    /// - CoinsText (monedas recogidas)
    /// - TimeText (tiempo de la partida)
    /// </summary>
    private void CreateEntryRow(int rank, LeaderboardEntry entry)
    {
        if (entryPrefab == null || entryContainer == null) return;

        GameObject row = Instantiate(entryPrefab, entryContainer);

        // Buscar los textos dentro del prefab por nombre
        TextMeshProUGUI rankText = FindChildText(row, "RankText");
        TextMeshProUGUI usernameText = FindChildText(row, "UsernameText");
        TextMeshProUGUI coinsText = FindChildText(row, "CoinsText");
        TextMeshProUGUI timeText = FindChildText(row, "TimeText");

        if (rankText != null) rankText.text = "#" + rank;
        if (usernameText != null) usernameText.text = entry.username;
        if (coinsText != null) coinsText.text = entry.coins.ToString();
        if (timeText != null) timeText.text = FormatTime(entry.time);
    }

    /// <summary>
    /// Busca un TextMeshProUGUI hijo por nombre.
    /// </summary>
    private TextMeshProUGUI FindChildText(GameObject parent, string childName)
    {
        Transform child = parent.transform.Find(childName);
        if (child != null)
            return child.GetComponent<TextMeshProUGUI>();
        return null;
    }

    /// <summary>
    /// Elimina todas las filas instanciadas del contenedor.
    /// </summary>
    private void ClearEntries()
    {
        if (entryContainer == null) return;

        foreach (Transform child in entryContainer)
        {
            Destroy(child.gameObject);
        }
    }

    private void SetLoadingState(bool show)
    {
        if (loadingText != null)
            loadingText.gameObject.SetActive(show);
    }

    private void SetErrorState(bool show, string message)
    {
        if (errorText != null)
        {
            errorText.gameObject.SetActive(show);
            errorText.text = message;
        }
    }

    /// <summary>
    /// Convierte segundos a formato "MM:SS".
    /// </summary>
    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}

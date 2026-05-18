using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// SupabaseManager gestiona la conexión con Supabase usando la REST API.
/// Es un Singleton: se accede con SupabaseManager.Instance desde cualquier script.
/// 
/// CONFIGURACIÓN EN EL INSPECTOR:
/// - Supabase Url: tu URL de proyecto (ej: https://xxxxx.supabase.co)
/// - Supabase Key: tu anon/public key
/// </summary>
public class SupabaseManager : MonoBehaviour
{
    public static SupabaseManager Instance;

    [Header("Configuración de Supabase")]
    [Tooltip("URL de tu proyecto Supabase (ej: https://xxxxx.supabase.co)")]
    [SerializeField] private string supabaseUrl = "";

    [Tooltip("Clave pública (anon key) de tu proyecto Supabase")]
    [SerializeField] private string supabaseKey = "";

    // Nombre de la tabla en Supabase
    private const string TABLE_NAME = "leaderboard";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ======================================================
    // INSERTAR PUNTUACIÓN
    // ======================================================

    /// <summary>
    /// Inserta una nueva entrada en el leaderboard.
    /// Solo necesitas enviar username, time y coins.
    /// id y created_at los genera Supabase automáticamente.
    /// </summary>
    /// <param name="username">Nombre del jugador</param>
    /// <param name="time">Tiempo de la partida en segundos</param>
    /// <param name="coins">Monedas recogidas</param>
    /// <param name="onSuccess">Callback cuando se inserta correctamente</param>
    /// <param name="onError">Callback si hay un error</param>
    public void InsertScore(string username, float time, int coins,
        Action onSuccess = null, Action<string> onError = null)
    {
        StartCoroutine(InsertScoreCoroutine(username, time, coins, onSuccess, onError));
    }

    private IEnumerator InsertScoreCoroutine(string username, float time, int coins,
        Action onSuccess, Action<string> onError)
    {
        string url = $"{supabaseUrl}/rest/v1/{TABLE_NAME}";

        // JSON con los datos a insertar (id y created_at los genera Supabase)
        string json = JsonUtility.ToJson(new ScoreInsert
        {
            username = username,
            time = time,
            coins = coins
        });

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            // Headers requeridos por Supabase
            request.SetRequestHeader("apikey", supabaseKey);
            request.SetRequestHeader("Authorization", "Bearer " + supabaseKey);
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Prefer", "return=minimal");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("[Supabase] Puntuación insertada correctamente");
                onSuccess?.Invoke();
            }
            else
            {
                string error = $"[Supabase] Error al insertar: {request.error} - {request.downloadHandler.text}";
                Debug.LogError(error);
                onError?.Invoke(error);
            }
        }
    }

    // ======================================================
    // OBTENER TOP 10
    // ======================================================

    /// <summary>
    /// Obtiene el Top 10 del leaderboard ordenado por monedas (desc) y tiempo (asc).
    /// </summary>
    /// <param name="onSuccess">Callback con la lista de entradas</param>
    /// <param name="onError">Callback si hay un error</param>
    public void GetTop10(Action<List<LeaderboardEntry>> onSuccess, Action<string> onError = null)
    {
        StartCoroutine(GetTop10Coroutine(onSuccess, onError));
    }
    
    private IEnumerator GetTop10Coroutine(Action<List<LeaderboardEntry>> onSuccess, Action<string> onError)
    {
        // Ordenar por más monedas primero, y si empatan, por menor tiempo
        string url = $"{supabaseUrl}/rest/v1/{TABLE_NAME}?select=*&order=coins.desc,time.asc&limit=10";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("apikey", supabaseKey);
            request.SetRequestHeader("Authorization", "Bearer " + supabaseKey);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                // Parseamos el array JSON
                List<LeaderboardEntry> entries = JsonHelper.FromJson<LeaderboardEntry>(jsonResponse);
                Debug.Log($"[Supabase] Top 10 obtenido: {entries.Count} entradas");
                onSuccess?.Invoke(entries);
            }
            else
            {
                string error = $"[Supabase] Error al obtener top 10: {request.error} - {request.downloadHandler.text}";
                Debug.LogError(error);
                onError?.Invoke(error);
            }
        }
    }
}

// ======================================================
// CLASES DE DATOS
// ======================================================

/// <summary>
/// Datos que se envían a Supabase al insertar una puntuación.
/// id y created_at NO se incluyen porque Supabase los genera automáticamente.
/// </summary>
[Serializable]
public class ScoreInsert
{
    public string username;
    public float time;
    public int coins;
}

/// <summary>
/// Datos que se reciben de Supabase al consultar el leaderboard.
/// Incluye todos los campos de la tabla.
/// </summary>
[Serializable]
public class LeaderboardEntry
{
    public int id;
    public string created_at;
    public string username;
    public float time;
    public int coins;
}

/// <summary>
/// Helper para parsear arrays JSON con JsonUtility de Unity.
/// JsonUtility no soporta arrays raíz directamente, así que lo envolvemos.
/// </summary>
public static class JsonHelper
{
    public static List<T> FromJson<T>(string json)
    {
        // Envolvemos el array en un objeto para que JsonUtility pueda parsearlo
        string wrappedJson = "{\"items\":" + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(wrappedJson);
        return new List<T>(wrapper.items);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] items;
    }
}

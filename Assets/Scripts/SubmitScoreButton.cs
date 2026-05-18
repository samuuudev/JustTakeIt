using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// SubmitScoreButton: conecta la UI (username input) con SupabaseManager.
/// Al enviar, muestra el LeaderboardPanel (canvas overlay) o llama a LeaderboardUI.ShowLeaderboard().
/// </summary>
public class SubmitScoreButton : MonoBehaviour
{
    [Header("Datos a enviar")]
    public TMP_InputField usernameField;
    public float timeValue;
    public int coinsValue;

    [Header("Comportamiento después de enviar")]
    public bool useOverlay = true; // si true activa leaderboardPanel, si false intenta usar leaderboardUI.ShowLeaderboard()
    public GameObject leaderboardPanel;
    public LeaderboardUI leaderboardUI;

    [Header("UI")]
    public Button submitButton;
    public TextMeshProUGUI feedbackText;

    private void Start()
    {
        if (submitButton != null)
            submitButton.onClick.AddListener(OnSubmit);
    }

    public void OnSubmit()
    {
        if (submitButton != null)
            submitButton.interactable = false;

        if (feedbackText != null)
            feedbackText.text = "Enviando...";

        string username = "Anon";
        if (usernameField != null && !string.IsNullOrEmpty(usernameField.text))
            username = usernameField.text.Trim();
        timeValue = GameManager.Instance.GetElapsedTime();
        coinsValue = GameManager.Instance.GetScore();

        if (string.IsNullOrEmpty(username))
        {
            if (feedbackText != null) feedbackText.text = "Introduce un nombre.";
            if (submitButton != null) submitButton.interactable = true;
            return;
        }

        if (SupabaseManager.Instance == null)
        {
            if (feedbackText != null) feedbackText.text = "Supabase no inicializado";
            if (submitButton != null) submitButton.interactable = true;
            return;
        }

        SupabaseManager.Instance.InsertScore(username, timeValue, coinsValue,
            onSuccess: () =>
            {
                if (feedbackText != null) feedbackText.text = "Enviado!";

                if (useOverlay)
                {
                    if (leaderboardUI != null)
                        leaderboardUI.ShowLeaderboard();
                    else if (leaderboardPanel != null)
                        leaderboardPanel.SetActive(true);
                }
                else
                {
                    if (leaderboardUI != null)
                        leaderboardUI.ShowLeaderboard();
                }

                if (submitButton != null) submitButton.interactable = true;
            },
            onError: (err) =>
            {
                if (feedbackText != null) feedbackText.text = "Error al enviar";
                Debug.LogError(err);
                if (submitButton != null) submitButton.interactable = true;
            });
    }
}

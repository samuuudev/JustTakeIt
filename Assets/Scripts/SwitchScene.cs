using MelenitasDev.SoundsGood;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DefaultNamespace
{
    public class SwitchScene : MonoBehaviour
    {
        [SerializeField] private GameObject leaderboardPanel;
        [SerializeField] private string gameplaySceneName = "Gameplay"; // Nombre de la escena de juego

        public void PlayGame()
        {
            SoundsGoodManager.StopAll();
            SceneManager.LoadScene(gameplaySceneName);
        }

        public void OpenLeaderboard()
        {
            if (leaderboardPanel != null)
                leaderboardPanel.SetActive(true);
        }

        public void CloseLeaderboard()
        {
            if (leaderboardPanel != null)
                leaderboardPanel.SetActive(false);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
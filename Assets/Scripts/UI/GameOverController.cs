using UnityEngine;
using TMPro;

namespace SoulKnight.UI
{
    public class GameOverController : MonoBehaviour
    {
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private TextMeshProUGUI finalScoreText;
        [SerializeField] private TextMeshProUGUI floorReachedText;

        private void Start()
        {
            gameOverPanel?.SetActive(false);
            if (Systems.GameManager.Instance != null)
                Systems.GameManager.Instance.OnGameStateChanged += OnStateChanged;
        }

        private void OnStateChanged(Systems.GameState state)
        {
            if (state != Systems.GameState.GameOver) return;

            gameOverPanel?.SetActive(true);

            if (finalScoreText && Systems.ScoreManager.Instance != null)
                finalScoreText.text = $"Score: {Systems.ScoreManager.Instance.Score}";

            if (floorReachedText && Systems.GameManager.Instance != null)
                floorReachedText.text = $"Floor Reached: {Systems.GameManager.Instance.CurrentFloor}";
        }

        public void OnRetryClicked() => Systems.GameManager.Instance?.StartNewRun();
        public void OnMenuClicked() => Systems.GameManager.Instance?.ReturnToMainMenu();

        private void OnDestroy()
        {
            if (Systems.GameManager.Instance != null)
                Systems.GameManager.Instance.OnGameStateChanged -= OnStateChanged;
        }
    }
}

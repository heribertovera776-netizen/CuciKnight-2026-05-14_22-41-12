using UnityEngine;

namespace SoulKnight.UI
{
    /// <summary>
    /// Pause menu panel. Reacts to GameManager state changes.
    /// </summary>
    public class PauseMenuController : MonoBehaviour
    {
        [SerializeField] private GameObject pausePanel;

        private void Start()
        {
            pausePanel?.SetActive(false);

            if (Systems.GameManager.Instance != null)
                Systems.GameManager.Instance.OnGameStateChanged += OnStateChanged;
        }

        private void OnStateChanged(Systems.GameState state)
        {
            pausePanel?.SetActive(state == Systems.GameState.Paused);
        }

        public void OnResumeClicked() => Systems.GameManager.Instance?.ResumeGame();
        public void OnRestartClicked() => Systems.GameManager.Instance?.StartNewRun();
        public void OnMainMenuClicked() => Systems.GameManager.Instance?.ReturnToMainMenu();

        private void OnDestroy()
        {
            if (Systems.GameManager.Instance != null)
                Systems.GameManager.Instance.OnGameStateChanged -= OnStateChanged;
        }
    }
}

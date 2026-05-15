using UnityEngine;
using UnityEngine.SceneManagement;

namespace SoulKnight.Systems
{
    /// <summary>
    /// Central singleton. Manages game state (menu, playing, paused, gameover).
    /// Persists across scenes.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Scene Names")]
        [SerializeField] private string mainMenuScene = "MainMenu";
        [SerializeField] private string gameScene = "Game";

        [Header("Run Data")]
        [SerializeField] private int currentFloor = 1;
        [SerializeField] private int totalCoinsCollected;

        public GameState CurrentState { get; private set; } = GameState.MainMenu;
        public int CurrentFloor => currentFloor;
        public int TotalCoins => totalCoinsCollected;

        // Events
        public event System.Action<GameState> OnGameStateChanged;
        public event System.Action OnRunStarted;
        public event System.Action OnRunEnded;
        public event System.Action OnNextFloor;

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

        public void StartNewRun()
        {
            currentFloor = 1;
            totalCoinsCollected = 0;
            SetState(GameState.Playing);
            OnRunStarted?.Invoke();
            SceneManager.LoadScene(gameScene);
        }

        public void GoToNextFloor()
        {
            currentFloor++;
            OnNextFloor?.Invoke();
            SceneManager.LoadScene(gameScene);
        }

        public void GameOver()
        {
            SetState(GameState.GameOver);
            OnRunEnded?.Invoke();
            // UI will react to state change
        }

        public void PauseGame()
        {
            if (CurrentState != GameState.Playing) return;
            SetState(GameState.Paused);
            Time.timeScale = 0f;
        }

        public void ResumeGame()
        {
            if (CurrentState != GameState.Paused) return;
            SetState(GameState.Playing);
            Time.timeScale = 1f;
        }

        public void ReturnToMainMenu()
        {
            Time.timeScale = 1f;
            SetState(GameState.MainMenu);
            SceneManager.LoadScene(mainMenuScene);
        }

        public void AddCoins(int amount) => totalCoinsCollected += amount;

        private void SetState(GameState newState)
        {
            CurrentState = newState;
            OnGameStateChanged?.Invoke(newState);
            Debug.Log($"[GameManager] State → {newState}");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (CurrentState == GameState.Playing) PauseGame();
                else if (CurrentState == GameState.Paused) ResumeGame();
            }
        }
    }

    public enum GameState { MainMenu, Playing, Paused, GameOver, Victory }
}

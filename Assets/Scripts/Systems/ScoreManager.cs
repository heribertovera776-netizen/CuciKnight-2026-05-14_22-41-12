using UnityEngine;

namespace SoulKnight.Systems
{
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance { get; private set; }

        private int score;
        public int Score => score;

        public event System.Action<int> OnScoreChanged;

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
        }

        public void AddScore(int amount)
        {
            score += amount;
            OnScoreChanged?.Invoke(score);
        }

        public void ResetScore()
        {
            score = 0;
            OnScoreChanged?.Invoke(score);
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SoulKnight.UI
{
    /// <summary>
    /// Connects PlayerStats events to HUD elements (health bar, armor bar, energy bar, score).
    /// Attach to Canvas / HUD root object.
    /// </summary>
    public class HUDController : MonoBehaviour
    {
        [Header("Health")]
        [SerializeField] private Slider healthBar;
        [SerializeField] private TextMeshProUGUI healthText;

        [Header("Armor")]
        [SerializeField] private Slider armorBar;
        [SerializeField] private TextMeshProUGUI armorText;

        [Header("Energy")]
        [SerializeField] private Slider energyBar;
        [SerializeField] private TextMeshProUGUI energyText;

        [Header("Score & Floor")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI floorText;

        [Header("Weapon Info")]
        [SerializeField] private Image weaponIcon;
        [SerializeField] private TextMeshProUGUI ammoText;

        private Player.PlayerStats playerStats;

        private void Start()
        {
            // Find player
            var player = FindFirstObjectByType<Player.PlayerStats>();
            if (player != null) RegisterPlayer(player);

            // Subscribe to score
            if (Systems.ScoreManager.Instance != null)
                Systems.ScoreManager.Instance.OnScoreChanged += UpdateScore;

            UpdateFloor();
        }

        public void RegisterPlayer(Player.PlayerStats stats)
        {
            playerStats = stats;
            stats.OnHealthChanged.AddListener(UpdateHealth);
            stats.OnArmorChanged.AddListener(UpdateArmor);
            stats.OnEnergyChanged.AddListener(UpdateEnergy);

            // Set initial values
            UpdateHealth(stats.CurrentHealth, stats.MaxHealth);
            UpdateArmor(stats.CurrentArmor, stats.MaxArmor);
            UpdateEnergy(stats.CurrentEnergy, stats.MaxEnergy);
        }

        private void UpdateHealth(int current, int max)
        {
            if (healthBar) healthBar.value = (float)current / max;
            if (healthText) healthText.text = $"{current}/{max}";
        }

        private void UpdateArmor(int current, int max)
        {
            if (armorBar) armorBar.value = (float)current / max;
            if (armorText) armorText.text = $"{current}/{max}";
        }

        private void UpdateEnergy(float current, float max)
        {
            if (energyBar) energyBar.value = current / max;
            if (energyText) energyText.text = $"{Mathf.FloorToInt(current)}/{Mathf.FloorToInt(max)}";
        }

        private void UpdateScore(int newScore)
        {
            if (scoreText) scoreText.text = $"Score: {newScore}";
        }

        private void UpdateFloor()
        {
            if (floorText && Systems.GameManager.Instance != null)
                floorText.text = $"Floor {Systems.GameManager.Instance.CurrentFloor}";
        }

        public void UpdateWeaponInfo(Weapons.WeaponData data, int currentAmmo)
        {
            if (weaponIcon && data?.Icon) weaponIcon.sprite = data.Icon;
            if (ammoText)
                ammoText.text = data != null && data.IsInfiniteAmmo ? "∞" : $"{currentAmmo}/{data?.MaxAmmo}";
        }

        private void OnDestroy()
        {
            if (playerStats != null)
            {
                playerStats.OnHealthChanged.RemoveListener(UpdateHealth);
                playerStats.OnArmorChanged.RemoveListener(UpdateArmor);
                playerStats.OnEnergyChanged.RemoveListener(UpdateEnergy);
            }

            if (Systems.ScoreManager.Instance != null)
                Systems.ScoreManager.Instance.OnScoreChanged -= UpdateScore;
        }
    }
}

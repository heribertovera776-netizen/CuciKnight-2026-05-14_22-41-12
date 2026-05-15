using UnityEngine;
using UnityEngine.Events;

namespace SoulKnight.Player
{
    /// <summary>
    /// Manages all player stats: health, armor, speed, energy.
    /// </summary>
    public class PlayerStats : MonoBehaviour
    {
        [Header("Health")]
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private int currentHealth;

        [Header("Armor")]
        [SerializeField] private int maxArmor = 50;
        [SerializeField] private int currentArmor;

        [Header("Energy")]
        [SerializeField] private float maxEnergy = 100f;
        [SerializeField] private float currentEnergy;
        [SerializeField] private float energyRegenRate = 5f;

        [Header("Movement")]
        [SerializeField] private float moveSpeed = 5f;

        // Events
        public UnityEvent<int, int> OnHealthChanged;   // current, max
        public UnityEvent<int, int> OnArmorChanged;
        public UnityEvent<float, float> OnEnergyChanged;
        public UnityEvent OnPlayerDied;

        public int MaxHealth => maxHealth;
        public int CurrentHealth => currentHealth;
        public int MaxArmor => maxArmor;
        public int CurrentArmor => currentArmor;
        public float MaxEnergy => maxEnergy;
        public float CurrentEnergy => currentEnergy;
        public float MoveSpeed => moveSpeed;
        public bool IsAlive => currentHealth > 0;

        private void Start()
        {
            currentHealth = maxHealth;
            currentArmor = maxArmor;
            currentEnergy = maxEnergy;
        }

        private void Update()
        {
            RegenerateEnergy();
        }

        public void TakeDamage(int damage)
        {
            if (!IsAlive) return;

            int remainingDamage = damage;

            // Armor absorbs damage first
            if (currentArmor > 0)
            {
                int armorAbsorb = Mathf.Min(currentArmor, remainingDamage);
                currentArmor -= armorAbsorb;
                remainingDamage -= armorAbsorb;
                OnArmorChanged?.Invoke(currentArmor, maxArmor);
            }

            // Apply leftover damage to health
            if (remainingDamage > 0)
            {
                currentHealth = Mathf.Max(0, currentHealth - remainingDamage);
                OnHealthChanged?.Invoke(currentHealth, maxHealth);
            }

            if (!IsAlive) Die();
        }

        public void Heal(int amount)
        {
            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        public void RestoreArmor(int amount)
        {
            currentArmor = Mathf.Min(maxArmor, currentArmor + amount);
            OnArmorChanged?.Invoke(currentArmor, maxArmor);
        }

        public bool UseEnergy(float amount)
        {
            if (currentEnergy < amount) return false;
            currentEnergy -= amount;
            OnEnergyChanged?.Invoke(currentEnergy, maxEnergy);
            return true;
        }

        private void RegenerateEnergy()
        {
            if (currentEnergy < maxEnergy)
            {
                currentEnergy = Mathf.Min(maxEnergy, currentEnergy + energyRegenRate * Time.deltaTime);
                OnEnergyChanged?.Invoke(currentEnergy, maxEnergy);
            }
        }

        public void UpgradeStat(StatType stat, float value)
        {
            switch (stat)
            {
                case StatType.MaxHealth:
                    maxHealth += (int)value;
                    currentHealth = Mathf.Min(currentHealth + (int)value, maxHealth);
                    OnHealthChanged?.Invoke(currentHealth, maxHealth);
                    break;
                case StatType.MaxArmor:
                    maxArmor += (int)value;
                    OnArmorChanged?.Invoke(currentArmor, maxArmor);
                    break;
                case StatType.MaxEnergy:
                    maxEnergy += value;
                    OnEnergyChanged?.Invoke(currentEnergy, maxEnergy);
                    break;
                case StatType.MoveSpeed:
                    moveSpeed += value;
                    break;
            }
        }

        private void Die()
        {
            OnPlayerDied?.Invoke();
            Debug.Log("Player has died.");
            // GameManager.Instance.GameOver();
        }
    }

    public enum StatType { MaxHealth, MaxArmor, MaxEnergy, MoveSpeed, Damage, FireRate }
}

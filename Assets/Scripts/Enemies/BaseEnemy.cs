using UnityEngine;
using UnityEngine.Events;
using SoulKnight.Weapons;
using SoulKnight.Systems;

namespace SoulKnight.Enemies
{
    /// <summary>
    /// Base class for all enemies. Handles health, damage reception, death and loot.
    /// </summary>
    public abstract class BaseEnemy : MonoBehaviour, IDamageable
    {
        [Header("Stats")]
        [SerializeField] protected EnemyData enemyData;

        [Header("Events")]
        public UnityEvent OnEnemyDied;
        public UnityEvent<int, int> OnHealthChanged;

        protected int currentHealth;
        protected bool isDead;

        // Reference to the player (assigned by EnemyManager or OnEnable)
        protected Transform playerTransform;

        public bool IsDead => isDead;
        public EnemyData Data => enemyData;

        protected virtual void Start()
        {
            currentHealth = enemyData.MaxHealth;
            playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        public virtual void TakeDamage(int damage)
        {
            if (isDead) return;

            currentHealth = Mathf.Max(0, currentHealth - damage);
            OnHealthChanged?.Invoke(currentHealth, enemyData.MaxHealth);

            OnDamageReceived(damage);

            if (currentHealth <= 0) Die();
        }

        /// <summary>Called each time the enemy takes damage. Override for reactions (flash, knockback).</summary>
        protected virtual void OnDamageReceived(int damage) { }

        protected virtual void Die()
        {
            isDead = true;
            OnEnemyDied?.Invoke();

            // Drop loot
            DropLoot();

            // Grant EXP / score
            ScoreManager.Instance?.AddScore(enemyData.ScoreValue);

            // Play death VFX
            if (enemyData.DeathVFX != null)
                Instantiate(enemyData.DeathVFX, transform.position, Quaternion.identity);

            Destroy(gameObject, 0.05f);
        }

        protected virtual void DropLoot()
        {
            if (enemyData.LootTable == null) return;
            enemyData.LootTable.RollDrop(transform.position);
        }

        /// <summary>Abstract: subclasses implement their AI update loop here.</summary>
        protected abstract void UpdateAI();

        private void Update()
        {
            if (!isDead) UpdateAI();
        }
    }
}

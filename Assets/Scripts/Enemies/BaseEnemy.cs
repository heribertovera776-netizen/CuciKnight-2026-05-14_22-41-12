using UnityEngine;
using UnityEngine.Events;
using SoulKnight.Weapons;
using SoulKnight.Systems;

namespace SoulKnight.Enemies
{
    public abstract class BaseEnemy : MonoBehaviour, IDamageable
    {
        [Header("Stats")]
        [SerializeField] protected EnemyData enemyData;

        public UnityEvent OnEnemyDied;
        public UnityEvent<int, int> OnHealthChanged;

        protected int currentHealth;
        protected bool isDead;
        protected Transform playerTransform;

        public bool IsDead => isDead;
        public EnemyData Data => enemyData;

        protected virtual void Start()
        {
            currentHealth = enemyData.MaxHealth;

            // Compatible con Unity 6
            var playerObj = GameObject.FindGameObjectWithTag("player");
            if (playerObj != null)
                playerTransform = playerObj.transform;
        }

        public virtual void TakeDamage(int damage)
        {
            if (isDead) return;
            currentHealth = Mathf.Max(0, currentHealth - damage);
            OnHealthChanged?.Invoke(currentHealth, enemyData.MaxHealth);
            OnDamageReceived(damage);
            if (currentHealth <= 0) Die();
        }

        protected virtual void OnDamageReceived(int damage) { }

        protected virtual void Die()
        {
            isDead = true;
            OnEnemyDied?.Invoke();
            DropLoot();
            ScoreManager.Instance?.AddScore(enemyData.ScoreValue);

            if (enemyData.DeathVFX != null)
                Instantiate(enemyData.DeathVFX, transform.position, Quaternion.identity);

            Destroy(gameObject, 0.05f);
        }

        protected virtual void DropLoot()
        {
            if (enemyData.LootTable == null) return;
            enemyData.LootTable.RollDrop(transform.position);
        }

        protected abstract void UpdateAI();

        private void Update()
        {
            if (!isDead) UpdateAI();
        }
    }
}
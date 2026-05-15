using UnityEngine;
using SoulKnight.Weapons;

namespace SoulKnight.Enemies
{
    /// <summary>
    /// Ranged enemy: keeps distance and fires projectiles at the player.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class RangedEnemy : BaseEnemy
    {
        [Header("Ranged AI")]
        [SerializeField] private float preferredRange = 5f;
        [SerializeField] private float detectionRadius = 8f;
        [SerializeField] private float fireRate = 1.5f;
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Transform firePoint;
        [SerializeField] private int projectileDamage = 10;
        [SerializeField] private float projectileSpeed = 8f;

        private Rigidbody2D rb;
        private float fireTimer;

        protected override void Start()
        {
            base.Start();
            rb = GetComponent<Rigidbody2D>();
        }

        protected override void UpdateAI()
        {
            if (playerTransform == null) return;

            float dist = Vector2.Distance(transform.position, playerTransform.position);
            fireTimer -= Time.deltaTime;

            if (dist > detectionRadius)
            {
                rb.linearVelocity = Vector2.zero;
                return;
            }

            // Face the player
            Vector2 dir = ((Vector2)playerTransform.position - rb.position).normalized;

            // Maintain preferred range
            if (dist > preferredRange + 0.5f)
                rb.linearVelocity = dir * enemyData.MoveSpeed;
            else if (dist < preferredRange - 0.5f)
                rb.linearVelocity = -dir * enemyData.MoveSpeed * 0.6f;
            else
                rb.linearVelocity = Vector2.zero;

            // Rotate to face player
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            // Shoot
            if (fireTimer <= 0f && dist <= detectionRadius)
            {
                FireProjectile(dir);
                fireTimer = 1f / fireRate;
            }
        }

        private void FireProjectile(Vector2 dir)
        {
            if (projectilePrefab == null) return;

            Transform spawnPoint = firePoint != null ? firePoint : transform;
            GameObject proj = Instantiate(projectilePrefab, spawnPoint.position,
                Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg));

            if (proj.TryGetComponent<Projectile>(out var p))
                p.Init(projectileDamage, projectileSpeed, 20f, "Enemy");
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, preferredRange);
        }
    }
}

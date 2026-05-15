using UnityEngine;

namespace SoulKnight.Weapons
{
    /// <summary>
    /// A projectile fired by a weapon. Travels in a straight line,
    /// deals damage on hit, and destroys itself at max range or on collision.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class Projectile : MonoBehaviour
    {
        [Header("Visual")]
        [SerializeField] private GameObject hitVFXPrefab;

        private int damage;
        private float speed;
        private float maxRange;
        private Vector2 startPosition;
        private Rigidbody2D rb;

        private string ownerTag = "Player"; // Ignore collisions with owner

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
        }

        public void Init(int dmg, float spd, float range, string shooterTag = "Player")
        {
            damage = dmg;
            speed = spd;
            maxRange = range;
            ownerTag = shooterTag;
            startPosition = transform.position;
            rb.linearVelocity = transform.right * speed;
        }

        private void Update()
        {
            if (Vector2.Distance(startPosition, transform.position) >= maxRange)
                DestroyProjectile();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(ownerTag)) return;
            if (other.CompareTag("Projectile")) return;

            // Deal damage to IDamageable targets
            if (other.TryGetComponent<IDamageable>(out var target))
                target.TakeDamage(damage);

            SpawnHitVFX();
            DestroyProjectile();
        }

        private void SpawnHitVFX()
        {
            if (hitVFXPrefab != null)
            {
                GameObject vfx = Instantiate(hitVFXPrefab, transform.position, Quaternion.identity);
                Destroy(vfx, 1f);
            }
        }

        private void DestroyProjectile()
        {
            Destroy(gameObject);
        }
    }

    /// <summary>Implement this interface on any object that can receive damage.</summary>
    public interface IDamageable
    {
        void TakeDamage(int damage);
    }
}

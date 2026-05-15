using UnityEngine;

namespace SoulKnight.Pickups
{
    public abstract class BasePickup : MonoBehaviour
    {
        [SerializeField] private float magnetRadius = 2f;
        [SerializeField] private float magnetSpeed = 6f;
        [SerializeField] protected AudioClip pickupSFX;

        private Transform playerTransform;

        private void Start()
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                playerTransform = playerObj.transform;
        }

        private void Update()
        {
            if (playerTransform == null) return;
            float dist = Vector2.Distance(transform.position, playerTransform.position);
            if (dist <= magnetRadius)
                transform.position = Vector2.MoveTowards(transform.position,
                    playerTransform.position, magnetSpeed * Time.deltaTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            var stats = other.GetComponent<Player.PlayerStats>();
            if (stats != null)
            {
                OnPickup(stats);
                if (pickupSFX != null)
                    AudioSource.PlayClipAtPoint(pickupSFX, transform.position);
                Destroy(gameObject);
            }
        }

        protected abstract void OnPickup(Player.PlayerStats stats);
    }

    public class HealthPickup : BasePickup
    {
        [SerializeField] private int healAmount = 20;
        protected override void OnPickup(Player.PlayerStats stats) => stats.Heal(healAmount);
    }

    public class ArmorPickup : BasePickup
    {
        [SerializeField] private int armorAmount = 15;
        protected override void OnPickup(Player.PlayerStats stats) => stats.RestoreArmor(armorAmount);
    }

    public class CoinPickup : BasePickup
    {
        [SerializeField] private int coinValue = 5;
        protected override void OnPickup(Player.PlayerStats stats)
        {
            Systems.GameManager.Instance?.AddCoins(coinValue);
            Systems.ScoreManager.Instance?.AddScore(coinValue);
        }
    }

    public class AmmoPickup : BasePickup
    {
        [SerializeField] private int ammoAmount = 15;
        protected override void OnPickup(Player.PlayerStats stats)
        {
            var shooter = stats.GetComponent<Player.PlayerShooter>();
            shooter?.GetCurrentWeapon()?.AddAmmo(ammoAmount);
        }
    }
}
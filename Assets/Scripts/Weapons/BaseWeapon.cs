using UnityEngine;

namespace SoulKnight.Weapons
{
    /// <summary>
    /// Abstract base for all weapons. Override Shoot() for custom behaviour.
    /// </summary>
    public abstract class BaseWeapon : MonoBehaviour
    {
        [Header("Weapon Config")]
        [SerializeField] protected WeaponData weaponData;
        [SerializeField] protected Transform firePoint;

        protected float fireCooldownTimer;
        protected int currentAmmo;

        public WeaponData Data => weaponData;
        public int CurrentAmmo => currentAmmo;
        public bool IsEmpty => weaponData.IsInfiniteAmmo ? false : currentAmmo <= 0;

        protected virtual void Awake()
        {
            if (weaponData != null)
                currentAmmo = weaponData.MaxAmmo;
        }

        protected virtual void Update()
        {
            fireCooldownTimer -= Time.deltaTime;
        }

        public bool TryShoot()
        {
            if (fireCooldownTimer > 0f) return false;
            if (IsEmpty) { OnAmmoEmpty(); return false; }

            Shoot();
            fireCooldownTimer = 1f / weaponData.FireRate;

            if (!weaponData.IsInfiniteAmmo)
                currentAmmo--;

            return true;
        }

        /// <summary>Implement bullet/projectile spawning here.</summary>
        protected abstract void Shoot();

        protected virtual void OnAmmoEmpty()
        {
            Debug.Log($"{weaponData.WeaponName} is out of ammo!");
            // TODO: play empty click SFX
        }

        public void AddAmmo(int amount)
        {
            currentAmmo = Mathf.Min(currentAmmo + amount, weaponData.MaxAmmo);
        }

        public void Reload()
        {
            currentAmmo = weaponData.MaxAmmo;
        }
    }
}

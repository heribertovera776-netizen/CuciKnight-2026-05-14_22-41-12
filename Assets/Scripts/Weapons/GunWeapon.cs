using UnityEngine;

namespace SoulKnight.Weapons
{
    /// <summary>
    /// Standard ranged weapon: spawns projectiles from the firePoint.
    /// Supports single-shot, burst, and spread (shotgun) patterns.
    /// </summary>
    public class GunWeapon : BaseWeapon
    {
        [Header("Muzzle Flash")]
        [SerializeField] private Transform muzzleFlashSpawn;

        protected override void Shoot()
        {
            if (weaponData.ProjectilePrefab == null)
            {
                Debug.LogWarning($"[{weaponData.WeaponName}] No ProjectilePrefab assigned!");
                return;
            }

            float baseAngle = firePoint.eulerAngles.z;
            int count = weaponData.ProjectilesPerShot;
            float spread = weaponData.SpreadAngle;

            for (int i = 0; i < count; i++)
            {
                float angleOffset = 0f;
                if (count > 1)
                    angleOffset = Mathf.Lerp(-spread / 2f, spread / 2f, (float)i / (count - 1));

                float finalAngle = baseAngle + angleOffset;
                Quaternion rotation = Quaternion.Euler(0, 0, finalAngle);

                GameObject proj = Instantiate(weaponData.ProjectilePrefab, firePoint.position, rotation);

                if (proj.TryGetComponent<Projectile>(out var projectile))
                {
                    projectile.Init(weaponData.Damage, weaponData.ProjectileSpeed, weaponData.Range);
                }
            }

            SpawnMuzzleFlash();
            PlayShootSFX();
        }

        private void SpawnMuzzleFlash()
        {
            if (weaponData.MuzzleFlashPrefab == null) return;
            Transform spawnPoint = muzzleFlashSpawn != null ? muzzleFlashSpawn : firePoint;
            GameObject flash = Instantiate(weaponData.MuzzleFlashPrefab, spawnPoint.position, spawnPoint.rotation);
            Destroy(flash, 0.1f);
        }

        private void PlayShootSFX()
        {
            if (weaponData.ShootSFX != null)
                AudioSource.PlayClipAtPoint(weaponData.ShootSFX, firePoint.position);
        }
    }
}

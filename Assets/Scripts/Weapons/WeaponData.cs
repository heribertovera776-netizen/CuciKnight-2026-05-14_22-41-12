using UnityEngine;

namespace SoulKnight.Weapons
{
    /// <summary>
    /// ScriptableObject that holds all weapon configuration.
    /// Create via: Assets > Create > SoulKnight > Weapon Data
    /// </summary>
    [CreateAssetMenu(fileName = "NewWeapon", menuName = "SoulKnight/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        [Header("Identity")]
        public string WeaponName = "Unknown Weapon";
        [TextArea] public string Description;
        public Sprite Icon;
        public WeaponType Type;
        public WeaponRarity Rarity;

        [Header("Combat")]
        public int Damage = 10;
        public float FireRate = 5f;          // Shots per second
        public float ProjectileSpeed = 15f;
        public float Range = 20f;
        public int ProjectilesPerShot = 1;   // e.g. shotgun = 5
        public float SpreadAngle = 0f;       // Degrees

        [Header("Ammo")]
        public bool IsInfiniteAmmo = false;
        public int MaxAmmo = 30;
        public float ReloadTime = 1.5f;

        [Header("Energy")]
        public float EnergyCostPerShot = 0f;

        [Header("Prefabs & FX")]
        public GameObject ProjectilePrefab;
        public GameObject MuzzleFlashPrefab;
        public AudioClip ShootSFX;
        public AudioClip EmptySFX;
        public AudioClip ReloadSFX;
    }

    public enum WeaponType { Pistol, Shotgun, MachineGun, Sniper, Launcher, Magic, Melee }
    public enum WeaponRarity { Common, Uncommon, Rare, Epic, Legendary }
}

using UnityEngine;

namespace SoulKnight.Enemies
{
    [CreateAssetMenu(fileName = "NewEnemy", menuName = "SoulKnight/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        [Header("Identity")]
        public string EnemyName = "Enemy";
        public Sprite Icon;
        public EnemyType Type;

        [Header("Stats")]
        public int MaxHealth = 30;
        public int Damage = 10;
        public float MoveSpeed = 2.5f;
        public int ScoreValue = 10;

        [Header("Loot")]
        public LootTable LootTable;

        [Header("VFX")]
        public GameObject DeathVFX;
        public AudioClip DeathSFX;
        public AudioClip HitSFX;
    }

    public enum EnemyType { Melee, Ranged, Boss, Elite }
}

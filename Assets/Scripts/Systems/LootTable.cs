using System.Collections.Generic;
using UnityEngine;

namespace SoulKnight.Systems
{
    /// <summary>
    /// ScriptableObject that defines what an enemy can drop.
    /// Each entry has a weight (higher = more likely).
    /// </summary>
    [CreateAssetMenu(fileName = "NewLootTable", menuName = "SoulKnight/Loot Table")]
    public class LootTable : ScriptableObject
    {
        [System.Serializable]
        public class LootEntry
        {
            public GameObject prefab;
            [Range(0f, 100f)] public float weight = 10f;
            [Range(0f, 1f)] public float dropChance = 0.5f;
        }

        [SerializeField] private List<LootEntry> entries;
        [SerializeField] [Range(0f, 1f)] private float globalDropChance = 0.6f;

        public void RollDrop(Vector3 position)
        {
            if (Random.value > globalDropChance) return;
            if (entries == null || entries.Count == 0) return;

            float totalWeight = 0f;
            foreach (var e in entries) totalWeight += e.weight;

            float roll = Random.Range(0f, totalWeight);
            float cumulative = 0f;

            foreach (var entry in entries)
            {
                cumulative += entry.weight;
                if (roll <= cumulative)
                {
                    if (Random.value <= entry.dropChance && entry.prefab != null)
                        Instantiate(entry.prefab, position + (Vector3)Random.insideUnitCircle * 0.3f, Quaternion.identity);
                    return;
                }
            }
        }
    }
}

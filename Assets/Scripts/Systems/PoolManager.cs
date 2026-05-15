using System.Collections.Generic;
using UnityEngine;

namespace SoulKnight.Systems
{
    /// <summary>
    /// Generic object pool to avoid instantiate/destroy overhead for bullets and VFX.
    /// Usage: PoolManager.Instance.Get("Bullet", prefab, position, rotation);
    /// </summary>
    public class PoolManager : MonoBehaviour
    {
        public static PoolManager Instance { get; private set; }

        [System.Serializable]
        public class PoolEntry
        {
            public string key;
            public GameObject prefab;
            public int initialSize = 10;
        }

        [SerializeField] private List<PoolEntry> initialPools;

        private Dictionary<string, Queue<GameObject>> pools = new();
        private Dictionary<string, GameObject> prefabLookup = new();

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            foreach (var entry in initialPools)
                CreatePool(entry.key, entry.prefab, entry.initialSize);
        }

        public void CreatePool(string key, GameObject prefab, int size)
        {
            if (pools.ContainsKey(key)) return;

            prefabLookup[key] = prefab;
            pools[key] = new Queue<GameObject>();

            for (int i = 0; i < size; i++)
                Enqueue(key, Instantiate(prefab));
        }

        public GameObject Get(string key, Vector3 position, Quaternion rotation)
        {
            if (!pools.ContainsKey(key))
            {
                Debug.LogWarning($"[PoolManager] Pool '{key}' not found.");
                return null;
            }

            GameObject obj = pools[key].Count > 0
                ? pools[key].Dequeue()
                : Instantiate(prefabLookup[key]);

            obj.transform.SetPositionAndRotation(position, rotation);
            obj.SetActive(true);
            return obj;
        }

        public void Return(string key, GameObject obj)
        {
            obj.SetActive(false);
            Enqueue(key, obj);
        }

        private void Enqueue(string key, GameObject obj)
        {
            obj.SetActive(false);
            obj.transform.SetParent(transform);
            pools[key].Enqueue(obj);
        }
    }
}

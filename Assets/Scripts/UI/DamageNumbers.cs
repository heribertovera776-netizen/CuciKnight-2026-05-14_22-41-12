using System.Collections;
using UnityEngine;
using TMPro;

namespace SoulKnight.UI
{
    /// <summary>
    /// Spawns a floating damage number at a world position.
    /// The prefab should have a TextMeshPro component on it.
    /// </summary>
    public class DamageNumber : MonoBehaviour
    {
        [SerializeField] private TextMeshPro label;
        [SerializeField] private float floatSpeed = 1.5f;
        [SerializeField] private float fadeDuration = 0.8f;

        public void Init(int damage, Color color)
        {
            label.text = damage.ToString();
            label.color = color;
            StartCoroutine(Animate());
        }

        private IEnumerator Animate()
        {
            float elapsed = 0f;
            Color startColor = label.color;
            Vector3 startPos = transform.position;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fadeDuration;

                transform.position = startPos + Vector3.up * (floatSpeed * elapsed);
                label.color = new Color(startColor.r, startColor.g, startColor.b, 1f - t);

                yield return null;
            }

            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Manager to spawn damage numbers. Call DamageNumberSpawner.Spawn() from anywhere.
    /// </summary>
    public class DamageNumberSpawner : MonoBehaviour
    {
        public static DamageNumberSpawner Instance { get; private set; }

        [SerializeField] private DamageNumber damageNumberPrefab;
        [SerializeField] private Color playerDamageColor = Color.white;
        [SerializeField] private Color critColor = Color.yellow;
        [SerializeField] private Color healColor = Color.green;

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
        }

        public void SpawnDamage(int amount, Vector3 worldPos, bool isCrit = false)
        {
            if (damageNumberPrefab == null) return;
            var num = Instantiate(damageNumberPrefab, worldPos + (Vector3)Random.insideUnitCircle * 0.3f, Quaternion.identity);
            num.Init(amount, isCrit ? critColor : playerDamageColor);
        }

        public void SpawnHeal(int amount, Vector3 worldPos)
        {
            if (damageNumberPrefab == null) return;
            var num = Instantiate(damageNumberPrefab, worldPos, Quaternion.identity);
            num.Init(amount, healColor);
        }
    }
}

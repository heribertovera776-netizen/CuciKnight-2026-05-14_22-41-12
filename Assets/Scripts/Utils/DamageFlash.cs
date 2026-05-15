using System.Collections;
using UnityEngine;

namespace SoulKnight.Utils
{
    /// <summary>
    /// Flashes the sprite renderer red when the entity takes damage.
    /// Attach alongside SpriteRenderer.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class DamageFlash : MonoBehaviour
    {
        [SerializeField] private Color flashColor = Color.red;
        [SerializeField] private float flashDuration = 0.1f;
        [SerializeField] private int flashCount = 2;

        private SpriteRenderer sr;
        private Color originalColor;
        private Coroutine flashCoroutine;

        private void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
            originalColor = sr.color;
        }

        public void Flash()
        {
            if (flashCoroutine != null) StopCoroutine(flashCoroutine);
            flashCoroutine = StartCoroutine(FlashRoutine());
        }

        private IEnumerator FlashRoutine()
        {
            for (int i = 0; i < flashCount; i++)
            {
                sr.color = flashColor;
                yield return new WaitForSeconds(flashDuration);
                sr.color = originalColor;
                yield return new WaitForSeconds(flashDuration);
            }
            sr.color = originalColor;
        }
    }
}

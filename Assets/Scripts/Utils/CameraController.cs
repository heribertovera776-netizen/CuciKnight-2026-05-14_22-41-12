using UnityEngine;

namespace SoulKnight.Utils
{
    /// <summary>
    /// Smooth camera follow with optional room-locked mode.
    /// Place on the Main Camera.
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        [Header("Follow")]
        [SerializeField] private Transform target;
        [SerializeField] private float smoothSpeed = 8f;
        [SerializeField] private Vector3 offset = new Vector3(0, 0, -10f);

        [Header("Bounds (optional)")]
        [SerializeField] private bool useBounds;
        [SerializeField] private Vector2 minBounds;
        [SerializeField] private Vector2 maxBounds;

        [Header("Screen Shake")]
        [SerializeField] private float shakeDuration;
        [SerializeField] private float shakeMagnitude = 0.15f;

        private Vector3 shakeOffset;
        private float shakeTimer;

        private void LateUpdate()
        {
            if (target == null) return;

            Vector3 desired = target.position + offset;

            if (useBounds)
            {
                desired.x = Mathf.Clamp(desired.x, minBounds.x, maxBounds.x);
                desired.y = Mathf.Clamp(desired.y, minBounds.y, maxBounds.y);
            }

            // Shake
            if (shakeTimer > 0f)
            {
                shakeOffset = Random.insideUnitSphere * shakeMagnitude;
                shakeOffset.z = 0f;
                shakeTimer -= Time.deltaTime;
            }
            else
            {
                shakeOffset = Vector3.zero;
            }

            transform.position = Vector3.Lerp(transform.position, desired + shakeOffset, smoothSpeed * Time.deltaTime);
        }

        public void SetTarget(Transform t) => target = t;

        public void Shake(float duration = 0.2f, float magnitude = 0.15f)
        {
            shakeTimer = duration;
            shakeMagnitude = magnitude;
        }

        public void SetRoomBounds(Vector2 min, Vector2 max)
        {
            useBounds = true;
            minBounds = min;
            maxBounds = max;
        }
    }
}

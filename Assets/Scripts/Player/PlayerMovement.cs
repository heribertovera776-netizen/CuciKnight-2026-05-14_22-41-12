using UnityEngine;

namespace SoulKnight.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float dashDistance = 3f;
        [SerializeField] private float dashCooldown = 1f;

        private Rigidbody2D rb;
        private Vector2 moveInput;
        private float dashTimer;
        private bool isDashing;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            // Leer input
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            moveInput = new Vector2(h, v).normalized;

            dashTimer -= Time.deltaTime;

            // Dash con Space
            if (Input.GetKeyDown(KeyCode.Space) && dashTimer <= 0f && !isDashing)
            {
                StartCoroutine(DashRoutine());
            }
        }

        private void FixedUpdate()
        {
            if (!isDashing)
            {
                rb.linearVelocity = moveInput * moveSpeed;
            }
        }

        private System.Collections.IEnumerator DashRoutine()
        {
            isDashing = true;
            dashTimer = dashCooldown;

            Vector2 dir = moveInput != Vector2.zero ? moveInput : Vector2.right;
            Vector2 target = rb.position + dir * dashDistance;

            float elapsed = 0f;
            float duration = 0.15f;
            Vector2 start = rb.position;

            while (elapsed < duration)
            {
                rb.MovePosition(Vector2.Lerp(start, target, elapsed / duration));
                elapsed += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            rb.MovePosition(target);
            isDashing = false;
        }
    }
}
using UnityEngine;

namespace SoulKnight.Player
{
    /// <summary>
    /// Top-down player movement using Rigidbody2D.
    /// WASD / Arrow keys / Left Joystick support.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(PlayerStats))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float dashDistance = 3f;
        [SerializeField] private float dashCooldown = 1f;
        [SerializeField] private float dashEnergyCost = 20f;

        private Rigidbody2D rb;
        private PlayerStats stats;
        private Animator animator;

        private Vector2 moveInput;
        private float dashCooldownTimer;
        private bool isDashing;

        // Animator parameter hashes
        private static readonly int HashMoveX = Animator.StringToHash("MoveX");
        private static readonly int HashMoveY = Animator.StringToHash("MoveY");
        private static readonly int HashIsMoving = Animator.StringToHash("IsMoving");
        private static readonly int HashDash = Animator.StringToHash("Dash");

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            stats = GetComponent<PlayerStats>();
            animator = GetComponent<Animator>();
        }

        private void Update()
        {
            GatherInput();
            HandleDash();
            UpdateAnimator();

            dashCooldownTimer -= Time.deltaTime;
        }

        private void FixedUpdate()
        {
            if (!isDashing)
                Move();
        }

        private void GatherInput()
        {
            moveInput.x = Input.GetAxisRaw("Horizontal");
            moveInput.y = Input.GetAxisRaw("Vertical");
            moveInput = moveInput.normalized;
        }

        private void Move()
        {
            rb.linearVelocity = moveInput * stats.MoveSpeed;
        }

        private void HandleDash()
        {
            if (Input.GetKeyDown(KeyCode.Space) && dashCooldownTimer <= 0f && !isDashing)
            {
                if (stats.UseEnergy(dashEnergyCost))
                {
                    StartCoroutine(DashRoutine());
                }
            }
        }

        private System.Collections.IEnumerator DashRoutine()
        {
            isDashing = true;
            dashCooldownTimer = dashCooldown;
            animator?.SetTrigger(HashDash);

            Vector2 dashDir = moveInput != Vector2.zero ? moveInput : (Vector2)transform.up;
            Vector2 startPos = rb.position;
            Vector2 targetPos = startPos + dashDir * dashDistance;

            float elapsed = 0f;
            float dashDuration = 0.15f;

            while (elapsed < dashDuration)
            {
                rb.MovePosition(Vector2.Lerp(startPos, targetPos, elapsed / dashDuration));
                elapsed += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            rb.MovePosition(targetPos);
            isDashing = false;
        }

        private void UpdateAnimator()
        {
            if (animator == null) return;
            animator.SetFloat(HashMoveX, moveInput.x);
            animator.SetFloat(HashMoveY, moveInput.y);
            animator.SetBool(HashIsMoving, moveInput != Vector2.zero);
        }

        public Vector2 GetMoveDirection() => moveInput;
    }
}

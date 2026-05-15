using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;

    [Header("Dash")]
    [SerializeField] private float dashDistance = 3f;
    [SerializeField] private float dashCooldown = 1f;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 moveInput;
    private float dashTimer;
    private bool isDashing;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(h, v).normalized;

        // Animacion
        if (animator != null)
            animator.SetBool("IsMoving", moveInput != Vector2.zero);

        // Voltear sprite segun direccion horizontal
        if (moveInput.x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = moveInput.x < 0 ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
            transform.localScale = scale;
        }

        // Dash
        dashTimer -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space) && dashTimer <= 0f && !isDashing)
            StartCoroutine(DashRoutine());
    }

    private void FixedUpdate()
    {
        if (!isDashing)
            rb.linearVelocity = moveInput * speed;
    }

    private System.Collections.IEnumerator DashRoutine()
    {
        isDashing = true;
        dashTimer = dashCooldown;

        Vector2 dir = moveInput != Vector2.zero ? moveInput : Vector2.right;
        Vector2 start = rb.position;
        Vector2 target = start + dir * dashDistance;

        float elapsed = 0f;
        float duration = 0.15f;

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
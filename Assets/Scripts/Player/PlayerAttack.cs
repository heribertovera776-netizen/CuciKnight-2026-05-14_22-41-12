using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float attackRadius = 1f;
    public int attackDamage = 20;
    public float attackCooldown = 0.5f;
    public KeyCode attackKey = KeyCode.Z;
    public float knockbackForce = 3f;
    private float attackTimer;

    private void Update()
    {
        attackTimer -= Time.deltaTime;
        if (Input.GetKeyDown(attackKey) && attackTimer <= 0f)
        {
            Attack();
            attackTimer = attackCooldown;
        }
    }
private Animator animator;

private void Start()
{
    animator = GetComponent<Animator>();
}
    private void Attack()
    {
        animator?.SetTrigger("Attack");
        
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRadius);
        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject) continue;
            if (!hit.CompareTag("Enemy")) continue;
            if (hit.TryGetComponent<EnemyHealth>(out var health))
                health.TakeDamage(attackDamage);
            if (hit.TryGetComponent<Rigidbody2D>(out var rb))
            {
                Vector2 dir = (hit.transform.position - transform.position).normalized;
                rb.AddForce(dir * knockbackForce, ForceMode2D.Impulse);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
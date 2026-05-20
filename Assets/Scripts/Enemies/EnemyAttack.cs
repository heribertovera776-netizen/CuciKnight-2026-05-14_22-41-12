using UnityEngine;

/// <summary>
/// Pega al jugador por contacto o por overlap periodico.
/// </summary>
public class EnemyAttack : MonoBehaviour
{
    public int damage = 15;
    public float attackCooldown = 1f;
    private float attackTimer;

    private void Update()
    {
        attackTimer -= Time.deltaTime;
    }

    private void OnCollisionStay2D(Collision2D col)
    {
        TryDamage(col.gameObject);
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        TryDamage(col.gameObject);
    }

    private void TryDamage(GameObject target)
    {
        if (attackTimer > 0f) return;
        if (!target.CompareTag("player")) return;

        if (target.TryGetComponent<PlayerHealth>(out var health))
        {
            health.TakeDamage(damage);
            attackTimer = attackCooldown;
        }
    }
}
using UnityEngine;

namespace SoulKnight.Enemies
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class MeleeEnemy : BaseEnemy
    {
        [Header("Melee AI")]
        [SerializeField] private float detectionRadius = 6f;
        [SerializeField] private float attackRadius = 0.8f;
        [SerializeField] private float attackCooldown = 1.2f;

        private Rigidbody2D rb;
        private Animator animator;
        private float attackTimer;
        private EnemyState state = EnemyState.Idle;

        //private static readonly int HashIsMoving = Animator.StringToHash("IsMoving");
       //private static readonly int HashAttack  = Animator.StringToHash("Attack");

        protected override void Start()
        {
            base.Start();
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
        }

        protected override void UpdateAI()
        {
            if (playerTransform == null) return;

            float dist = Vector2.Distance(transform.position, playerTransform.position);
            attackTimer -= Time.deltaTime;

            switch (state)
            {
                case EnemyState.Idle:
                    rb.linearVelocity = Vector2.zero;
                    if (dist <= detectionRadius) state = EnemyState.Chase;
                    break;

                case EnemyState.Chase:
                    ChasePlayer(dist);
                    break;

                case EnemyState.Attack:
                    rb.linearVelocity = Vector2.zero;
                    if (attackTimer <= 0f) state = EnemyState.Chase;
                    break;
            }

           // animator?.SetBool(HashIsMoving, rb.linearVelocity.sqrMagnitude > 0.1f);
        }

        private void ChasePlayer(float dist)
        {
            if (dist > detectionRadius * 1.5f) { state = EnemyState.Idle; return; }
            if (dist <= attackRadius) { Attack(); return; }

            Vector2 dir = ((Vector2)playerTransform.position - rb.position).normalized;
            rb.linearVelocity = dir * enemyData.MoveSpeed;
        }

        private void Attack()
        {
            if (attackTimer > 0f) return;
            state = EnemyState.Attack;
            attackTimer = attackCooldown;
            //animator?.SetTrigger(HashAttack);

            Collider2D hit = Physics2D.OverlapCircle(transform.position, attackRadius + 0.2f,
                LayerMask.GetMask("player"));

            if (hit != null && hit.TryGetComponent<Player.PlayerStats>(out var player))
                player.TakeDamage(enemyData.Damage);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRadius);
        }
    }

    public enum EnemyState { Idle, Chase, Attack, Flee, Dead }
}
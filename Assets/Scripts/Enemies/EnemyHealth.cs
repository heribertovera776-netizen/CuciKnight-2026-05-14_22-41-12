using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Salud")]
    public int maxHealth = 50;
    private int currentHealth;

    [Header("Feedback")]
    public SpriteRenderer sr;
    public Color hitColor = Color.red;
    private Color originalColor;

    private void Start()
    {
        currentHealth = maxHealth;
        if (sr == null) sr = GetComponent<SpriteRenderer>();
        if (sr != null) originalColor = sr.color;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} recibio {damage} de daño. HP: {currentHealth}");

        // Flash rojo
        if (sr != null)
            StartCoroutine(HitFlash());

        if (currentHealth <= 0)
            Die();
    }

    private System.Collections.IEnumerator HitFlash()
    {
        sr.color = hitColor;
        yield return new WaitForSeconds(0.1f);
        sr.color = originalColor;
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} murio!");
        Destroy(gameObject);
    }
}
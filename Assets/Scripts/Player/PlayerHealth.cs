using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Salud")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Invencibilidad tras recibir daño")]
    public float invincibleTime = 1f;
    private float invincibleTimer;

    [Header("HUD")]
    public Slider healthBar;

    [Header("Feedback")]
    public SpriteRenderer sr;
    public Color hitColor = Color.red;
    private Color originalColor;

    private bool isDead;

    private void Start()
    {
        currentHealth = maxHealth;
        if (sr == null) sr = GetComponent<SpriteRenderer>();
        if (sr != null) originalColor = sr.color;
        UpdateHealthBar();
    }

    private void Update()
    {
        if (invincibleTimer > 0f)
            invincibleTimer -= Time.deltaTime;
    }

    public void TakeDamage(int damage)
    {
        if (isDead || invincibleTimer > 0f) return;

        currentHealth -= damage;
        invincibleTimer = invincibleTime;
        Debug.Log($"Jugador recibio {damage} daño. HP: {currentHealth}");

        if (sr != null)
            StartCoroutine(HitFlash());

        UpdateHealthBar();

        if (currentHealth <= 0)
            Die();
    }

    private System.Collections.IEnumerator HitFlash()
    {
        sr.color = hitColor;
        yield return new WaitForSeconds(0.1f);
        sr.color = originalColor;
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
            healthBar.value = (float)currentHealth / maxHealth;
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("Jugador murio! Reiniciando...");
        StartCoroutine(DeathAndRestart());
    }

    private System.Collections.IEnumerator DeathAndRestart()
    {
        // Desactiva el movimiento
        var movement = GetComponent<PlayerMovement>();
        if (movement != null) movement.enabled = false;

        // Pausa breve antes de reiniciar
        yield return new WaitForSeconds(1.5f);

        // Reinicia la escena actual
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
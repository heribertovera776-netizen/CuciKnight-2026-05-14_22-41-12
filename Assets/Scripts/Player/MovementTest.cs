using UnityEngine;

/// <summary>
/// Script de prueba minimalista - sin namespaces ni dependencias.
/// Ponlo en el Player y dale Play. Si se mueve, el problema era otro script.
/// </summary>
public class MovementTest : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Debug.Log("MovementTest: Script iniciado correctamente");
    }

    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        rb.linearVelocity = new Vector2(h, v).normalized * speed;
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controla al jugador: movimiento, salto, disparo, vida y animacion.
/// El personaje mira siempre hacia el lado en el que se mueve.
/// </summary>
public class CharacterController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] GameObject shape;
    [SerializeField] Animator animator;
    [SerializeField] LayerMask groundMask;

    [Header("Movimiento")]
    [SerializeField] float movementSpeed;
    [SerializeField] float runSpeedMultiplier = 1.6f;
    [SerializeField] KeyCode runKey = KeyCode.LeftShift;
    [SerializeField] float groundDetectionRange;

    [Header("Salto")]
    [SerializeField] float jumpForce;
    [SerializeField] int maxJumpCount;
    [SerializeField] KeyCode jumpKey;

    [Header("Disparo")]
    [SerializeField] Projectile projectilePrefabRight;
    [SerializeField] Projectile projectilePrefabLeft;
    [SerializeField] int damage;

    [Header("Vida")]
    [SerializeField] int health;
    [SerializeField] int maxHealth = 3;
    [SerializeField] float deathHeight;

    // Parpadeo del sprite mientras dura la invulnerabilidad tras un golpe.
    const int BlinkCount = 3;
    const float BlinkHalfPeriod = 0.2f;

    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;

    int jumpCount;
    bool isGrounded;
    bool isHittable;
    bool isFacingRight;

    /// <summary>True mientras el personaje mira a la derecha.</summary>
    public bool IsFacingRight => isFacingRight;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = shape.GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        isHittable = true;
        SetFacing(true);
    }

    void Update()
    {
        var input = Input.GetAxisRaw("Horizontal");

        DetectGround();
        Move(input);
        FaceMovementDirection(input);
        HandleActions();

        if (transform.position.y < deathHeight)
            Die();
    }

    // ---------------------------------------------------------------- movimiento

    void Move(float input)
    {
        var isRunning = input != 0f && Input.GetKey(runKey);
        var speed = isRunning ? movementSpeed * runSpeedMultiplier : movementSpeed;

        animator.SetBool("isMoving", input != 0f);
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isGrounded", isGrounded);

        var velocity = rb.velocity;
        velocity.x = input * speed;
        rb.velocity = velocity;
    }

    void DetectGround()
    {
        var hit = Physics2D.Raycast(transform.position, Vector2.down, groundDetectionRange, groundMask);
        if (!isGrounded && hit)
            jumpCount = 0;

        isGrounded = hit;
    }

    void HandleActions()
    {
        if (Input.GetKeyDown(jumpKey))
            Jump();

        if (Input.GetMouseButtonDown(0))
            Shoot();
    }

    void Jump()
    {
        if (jumpCount >= maxJumpCount)
            return;

        var velocity = rb.velocity;
        velocity.y = jumpForce;
        rb.velocity = velocity;
        jumpCount++;
    }

    // ---------------------------------------------------------------- mirada

    /// <summary>Gira hacia el lado en el que se mueve; si esta parado mantiene la ultima mirada.</summary>
    void FaceMovementDirection(float input)
    {
        if (Mathf.Approximately(input, 0f))
            return;

        SetFacing(input > 0f);
    }

    /// <summary>
    /// Aplica la mirada con SpriteRenderer.flipX en vez de invertir la escala del transform:
    /// no ensucia la escala y evita el doble espejado. El arte sin espejar mira a la derecha.
    /// </summary>
    void SetFacing(bool facingRight)
    {
        isFacingRight = facingRight;
        spriteRenderer.flipX = !facingRight;
    }

    // ---------------------------------------------------------------- disparo

    void Shoot()
    {
        animator.SetTrigger("attack");

        var prefab = isFacingRight ? projectilePrefabRight : projectilePrefabLeft;
        var projectile = Instantiate(prefab, transform.position, Quaternion.identity);
        projectile.Shoot(isFacingRight ? 1 : -1, damage, isFacingRight);
    }

    // ---------------------------------------------------------------- vida

    /// <summary>Aplica dano salvo que ya este muerto o en plena invulnerabilidad.</summary>
    public void GetHit(int amount)
    {
        if (!isHittable || health <= 0)
            return;

        health = Mathf.Max(health - amount, 0);
        HUDController.Refresh(health);

        if (health <= 0)
            Die();
        else
            StartCoroutine(StartInvulnerability());
    }

    /// <summary>Cura sin pasarse de la vida maxima.</summary>
    public void Heal(int amount)
    {
        if (health <= 0 || health >= maxHealth)
            return;

        health = Mathf.Min(health + amount, maxHealth);
        HUDController.Refresh(health);
    }

    IEnumerator StartInvulnerability()
    {
        isHittable = false;

        for (int i = 0; i < BlinkCount; i++)
        {
            shape.SetActive(false);
            yield return new WaitForSeconds(BlinkHalfPeriod);
            shape.SetActive(true);
            yield return new WaitForSeconds(BlinkHalfPeriod);
        }

        isHittable = true;
    }

    /// <summary>Reinicia el nivel: al morir o al llegar a la meta.</summary>
    public void Die()
    {
        SceneManager.LoadScene(0);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundDetectionRange);
    }
}

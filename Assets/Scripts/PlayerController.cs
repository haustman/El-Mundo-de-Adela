using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controla al jugador: movimiento, salto, disparo, vida y animacion.
/// El personaje mira siempre hacia el lado en el que se mueve.
/// </summary>
public class PlayerController : MonoBehaviour
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
    [SerializeField] float deathAnimationDuration = 0.5f;

    // Parpadeo del sprite mientras dura la invulnerabilidad tras un golpe.
    const int BlinkCount = 3;
    const float BlinkHalfPeriod = 0.2f;

    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;

    int jumpCount;
    bool isGrounded;
    bool isHittable;
    bool isFacingRight;
    bool isDead;
    bool isFinished;
    Vector3 spawnPosition;
    Coroutine blinkRoutine;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = shape.GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        spawnPosition = transform.position;
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

        if (transform.position.y < deathHeight && !isDead)
            FallIntoVoid();
    }

    // ---------------------------------------------------------------- movimiento

    void Move(float input)
    {
        var isRunning = input != 0f && Input.GetKey(runKey);
        var speed = isRunning ? movementSpeed * runSpeedMultiplier : movementSpeed;

        if (animator)
        {
            animator.SetBool("isMoving", input != 0f);
            animator.SetBool("isRunning", isRunning);
            animator.SetBool("isGrounded", isGrounded);
        }

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
        if (animator)
            animator.SetTrigger("attack");

        var prefab = isFacingRight ? projectilePrefabRight : projectilePrefabLeft;
        var projectile = Instantiate(prefab, transform.position, Quaternion.identity);
        projectile.Shoot(isFacingRight ? 1 : -1, damage, isFacingRight);
    }

    // ---------------------------------------------------------------- vida

    /// <summary>Aplica dano salvo que ya este muerto o en plena invulnerabilidad.</summary>
    public void GetHit(int amount)
    {
        ApplyDamage(amount, respectInvulnerability: true);
    }

    /// <summary>Resta vida y refresca el HUD. Al llegar a 0 vidas, muere.</summary>
    void ApplyDamage(int amount, bool respectInvulnerability)
    {
        if (isDead)
            return;

        if (respectInvulnerability && !isHittable)
            return;

        health = Mathf.Max(health - amount, 0);
        HUDController.Refresh(health);

        if (animator)
            animator.SetTrigger("takeDamage");

        if (health <= 0)
        {
            Die();
            return;
        }

        StartBlink();
    }

    /// <summary>Cura sin pasarse de la vida maxima. Devuelve true si ha curado de verdad.</summary>
    public bool Heal(int amount)
    {
        if (health <= 0 || health >= maxHealth)
            return false;

        health = Mathf.Min(health + amount, maxHealth);
        HUDController.Refresh(health);
        return true;
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
        blinkRoutine = null;
    }

    /// <summary>Arranca el parpadeo; si ya habia uno en marcha lo reinicia limpiamente.</summary>
    void StartBlink()
    {
        StopBlink();
        blinkRoutine = StartCoroutine(StartInvulnerability());
    }

    /// <summary>Detiene el parpadeo dejando el sprite visible.</summary>
    void StopBlink()
    {
        if (blinkRoutine == null)
            return;

        StopCoroutine(blinkRoutine);
        blinkRoutine = null;
        shape.SetActive(true);
    }

    /// <summary>Caer al vacio: resta 1 vida y reaparece en el punto de inicio. Al llegar a 0 vidas, muere.</summary>
    void FallIntoVoid()
    {
        ApplyDamage(1, respectInvulnerability: false);

        if (isDead)
            return;

        transform.position = spawnPosition;
        rb.velocity = Vector2.zero;
    }

    /// <summary>Muerte: animacion y vuelta al menu.</summary>
    public void Die()
    {
        if (isDead)
            return;

        StopBlink();
        isDead = true;
        if (animator)
            animator.SetBool("isDead", true);
        StartCoroutine(DeathSequence());
    }

    IEnumerator DeathSequence()
    {
        rb.velocity = Vector2.zero;
        rb.simulated = false;
        yield return new WaitForSeconds(deathAnimationDuration);
        SceneManager.LoadScene(0);
    }

    /// <summary>Llega a la meta: se acaba el nivel y vuelve a empezar.</summary>
    public void Finish()
    {
        if (isDead || isFinished)
            return;

        isFinished = true;
        StopBlink();
        rb.velocity = Vector2.zero;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundDetectionRange);
    }
}

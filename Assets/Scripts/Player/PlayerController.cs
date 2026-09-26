using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ElMundoDeAdela
{
    /// <summary>
    /// Controla a Adela: mover, correr, saltar (con doble salto), disparar y recibir daño.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] GameObject shape;
        [SerializeField] Animator animator;
        [SerializeField] LayerMask groundMask;
        [Tooltip("Punto desde el que sale el proyectil. Si se deja vacío se usa el centro del jugador.")]
        [SerializeField] Transform firePoint;

        [Header("Movimiento")]
        [SerializeField] float movementSpeed = 10f;
        [SerializeField] float runSpeedMultiplier = 1.6f;
        [SerializeField] KeyCode runKey = KeyCode.LeftShift;
        [SerializeField] float groundDetectionRange = 0.66f;

        [Header("Salto")]
        [SerializeField] float jumpForce = 20f;
        [SerializeField] int maxJumpCount = 2;
        [SerializeField] KeyCode jumpKey = KeyCode.Space;

        [Header("Disparo")]
        [SerializeField] Projectile projectilePrefabRight;
        [SerializeField] Projectile projectilePrefabLeft;
        [SerializeField] int damage = 1;
        [Tooltip("Retardo entre el inicio de la animación de ataque y la salida del proyectil (s).")]
        [SerializeField] float shotDelay = 0.13f;

        [Header("Vida")]
        [SerializeField] int health = 3;
        [SerializeField] int maxHealth = 3;

        [Header("Invulnerabilidad")]
        [Tooltip("Número de parpadeos tras recibir un golpe.")]
        [SerializeField] int blinkCount = 3;
        [Tooltip("Duración de cada medio parpadeo, en segundos.")]
        [SerializeField] float blinkHalfPeriod = 0.2f;

        [Header("Muerte")]
        [SerializeField] float deathHeight = -36f;
        [SerializeField] float deathAnimationDuration = 0.5f;
        [SerializeField] string menuSceneName = "Menu";

        [Header("Caída al vacío")]
        [SerializeField] int fallDamage = 1;
        [Tooltip("Punto donde reaparece Adela. Si se deja vacío se usa su posición inicial.")]
        [SerializeField] Transform respawnPoint;

        // Los parámetros del Animator por hash: evitan buscar el nombre en cada llamada.
        static readonly int IsMovingHash = Animator.StringToHash("isMoving");
        static readonly int IsRunningHash = Animator.StringToHash("isRunning");
        static readonly int IsGroundedHash = Animator.StringToHash("isGrounded");
        static readonly int AttackHash = Animator.StringToHash("attack");
        static readonly int TakeDamageHash = Animator.StringToHash("takeDamage");
        static readonly int IsDeadHash = Animator.StringToHash("isDead");

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
        bool pendingShot;
        float pendingShotTime;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();

            if (!shape)
            {
                Debug.LogError($"{nameof(PlayerController)}: falta asignar 'shape' en el Inspector.", this);
                enabled = false;
                return;
            }

            spriteRenderer = shape.GetComponent<SpriteRenderer>();

            if (!spriteRenderer)
            {
                Debug.LogError($"{nameof(PlayerController)}: 'shape' no tiene un SpriteRenderer.", this);
                enabled = false;
            }
        }

        void Start()
        {
            spawnPosition = transform.position;
            isHittable = true;
            SetFacing(true);
        }

        void Update()
        {
            if (isDead)
                return;

            var input = Input.GetAxisRaw("Horizontal");

            DetectGround();
            Move(input);
            FaceMovementDirection(input);
            HandleActions();
            ResolvePendingShot();

            if (transform.position.y < deathHeight)
                FallIntoVoid();
        }

        // ---------------------------------------------------------------- movimiento

        void Move(float input)
        {
            bool running = input != 0f && Input.GetKey(runKey);
            float speed = running ? movementSpeed * runSpeedMultiplier : movementSpeed;

            if (animator)
            {
                animator.SetBool(IsMovingHash, input != 0f);
                animator.SetBool(IsRunningHash, running);
                animator.SetBool(IsGroundedHash, isGrounded);
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

        /// <summary>Gira hacia el lado en el que se mueve; si está parado mantiene la última mirada.</summary>
        void FaceMovementDirection(float input)
        {
            if (Mathf.Approximately(input, 0f))
                return;

            SetFacing(input > 0f);
        }

        // Usamos SpriteRenderer.flipX en lugar de invertir la escala: no ensucia la
        // escala y evita el doble espejeado. El arte sin espejar mira a la derecha.
        void SetFacing(bool facingRight)
        {
            isFacingRight = facingRight;
            spriteRenderer.flipX = !facingRight;
        }

        // ---------------------------------------------------------------- disparo

        void Shoot()
        {
            // El proyectil no sale al instante: esperamos shotDelay para que coincida
            // con el fotograma del clip Attack en el que el arma apunta.
            if (animator)
            {
                animator.SetTrigger(AttackHash);
                pendingShot = true;
                pendingShotTime = Time.time;
                return;
            }

            FireProjectile();
        }

        /// <summary>Suelta el proyectil cuando pasa el retardo del ataque.</summary>
        void ResolvePendingShot()
        {
            if (!pendingShot || Time.time - pendingShotTime < shotDelay)
                return;

            pendingShot = false;
            FireProjectile();
        }

        void FireProjectile()
        {
            var prefab = isFacingRight ? projectilePrefabRight : projectilePrefabLeft;
            if (!prefab)
                return;

            var origin = firePoint ? firePoint.position : transform.position;
            var projectile = Instantiate(prefab, origin, Quaternion.identity);
            projectile.Shoot(isFacingRight ? 1 : -1, damage, isFacingRight);
        }

        // ---------------------------------------------------------------- vida

        /// <summary>Aplica daño salvo que ya esté muerto o en plena invulnerabilidad.</summary>
        public void GetHit(int amount)
        {
            ApplyDamage(amount, respectInvulnerability: true);
        }

        /// <summary>Resta vida y refresca el HUD. Al quedarse sin corazones, muere.</summary>
        void ApplyDamage(int amount, bool respectInvulnerability)
        {
            if (isDead || amount <= 0)
                return;

            if (respectInvulnerability && !isHittable)
                return;

            health = Mathf.Max(health - amount, 0);
            HUDController.Refresh(health);

            if (animator)
                animator.SetTrigger(TakeDamageHash);

            if (health <= 0)
            {
                Die();
                return;
            }

            StartBlink();
        }

        /// <summary>Cura sin pasarse de la vida máxima. Devuelve true si ha curado de verdad.</summary>
        public bool Heal(int amount)
        {
            if (isDead || health <= 0 || health >= maxHealth || amount <= 0)
                return false;

            health = Mathf.Min(health + amount, maxHealth);
            HUDController.Refresh(health);
            return true;
        }

        IEnumerator Blink()
        {
            isHittable = false;

            for (int i = 0; i < blinkCount; i++)
            {
                shape.SetActive(false);
                yield return new WaitForSeconds(blinkHalfPeriod);

                shape.SetActive(true);
                yield return new WaitForSeconds(blinkHalfPeriod);
            }

            isHittable = true;
            blinkRoutine = null;
        }

        /// <summary>Arranca el parpadeo; si ya había uno en marcha lo reinicia limpiamente.</summary>
        void StartBlink()
        {
            StopBlink();
            blinkRoutine = StartCoroutine(Blink());
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

        /// <summary>Caer al vacío resta vida y devuelve a Adela al punto de inicio.</summary>
        void FallIntoVoid()
        {
            ApplyDamage(fallDamage, respectInvulnerability: false);

            if (isDead)
                return;

            transform.position = respawnPoint ? respawnPoint.position : spawnPosition;
            rb.velocity = Vector2.zero;
        }

        /// <summary>Muerte: animación y vuelta al menú.</summary>
        public void Die()
        {
            if (isDead)
                return;

            StopBlink();
            isDead = true;

            if (animator)
                animator.SetBool(IsDeadHash, true);

            StartCoroutine(DeathSequence());
        }

        IEnumerator DeathSequence()
        {
            rb.velocity = Vector2.zero;
            rb.simulated = false;

            yield return new WaitForSeconds(deathAnimationDuration);

            SceneManager.LoadScene(menuSceneName);
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

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.black;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundDetectionRange);
        }
    }
}

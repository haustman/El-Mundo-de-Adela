# Animaciones

Clips de animación y Animator Controller del personaje.

- **Character.controller** — Animator Controller principal. Parámetros: isMoving, isRunning, isGrounded, attack (trigger), takeDamage (trigger), isDead (bool).
- **Idle.anim** — Personaje parado (4 frames).
- **Walk.anim** — Caminar (4 frames).
- **Run.anim** — Correr (4 frames).
- **Jump.anim** — Saltar (7 frames).
- **Attack.anim** — Ataque (5 frames).
- **Danger.anim** — Recibir daño (3 frames, spritesheet).
- **Death.anim** — Muerte (4 frames, spritesheet).

### Boss

- **Boss.controller** — Animator Controller del jefe. Parámetros: isMoving, isRunning (bool), attack (trigger). Estados: Idle, Walk, Run, Attack.
- **BossIdle.anim** — Jefe parado (5 frames).
- **BossWalk.anim** — Jefe caminando (5 frames).
- **BossRun.anim** — Jefe corriendo (5 frames).
- **BossAttack.anim** — Ataque del jefe (5 frames, sin loop).

Todas las animaciones están comprimidas para optimizar memoria en móvil.

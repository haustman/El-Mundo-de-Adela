# Animaciones

Clips de animación y Animator Controller del personaje.

## Character

- **Character.controller** — Animator Controller principal. Parámetros: `isMoving`, `isRunning`, `isGrounded` (bool), `attack`, `takeDamage` (trigger), `isDead` (bool). Estados: Idle, Walk, Run, Jump, Attack, Danger, Death.
- **Idle.anim** — Personaje parado (4 frames, en bucle).
- **Walk.anim** — Caminar (4 frames, en bucle).
- **Run.anim** — Correr (4 frames, en bucle).
- **Jump.anim** — Salto/caída (7 frames, sin bucle).
- **Attack.anim** — Ataque (5 frames, sin bucle).
- **Danger.anim** — Recibir daño (3 frames, sin bucle).
- **Death.anim** — Muerte (4 frames, sin bucle).

### Prioridad de las transiciones AnyState

El Animator evalúa las transiciones *AnyState* en el orden en que aparecen y aplica la primera que cumple. El orden es, a propósito:

1. `isDead` → Death
2. `takeDamage` → Danger
3. `attack` → Attack
4. `isGrounded == false` → Jump

Así **muerte y daño tienen prioridad sobre el salto**: si no, morir o recibir un golpe en el aire se pisaba con la animación de salto.

Además, `Attack` y `Danger` terminan y vuelven solos al estado de locomoción que corresponda según `isMoving`/`isRunning` (Run, Walk o Idle como último recurso), sin pasar siempre por Idle.

### Sincronización del disparo

El proyectil del jugador no sale al instante de pulsar el botón: `PlayerController` espera `shotDelay` segundos (0.13 por defecto) desde que arranca el clip `Attack`, para que coincida con el fotograma en el que el arma apunta.

> **Ojo:** el Animator del jugador está en el hijo **`Circle`** (el objeto que lleva el `SpriteRenderer`), **no** en la raíz `Character` (que es la que tiene `PlayerController`). Por eso **no** se usan Animation Events: los eventos se entregan al objeto del Animator, es decir a `Circle`, que no tiene el script, y Unity avisa *"has no receiver! Are you missing a component?"*.

### Pendiente de arte

- El estado `Jump` es también el de caída. Como el clip no hace bucle, en una caída larga se queda en el último fotograma. Un estado **Fall** propio necesita más arte.
- El jefe no tiene animación de muerte: al morir, `Damageable` destruye el objeto directamente.

### Notas

- Todas las animaciones están comprimidas para optimizar memoria en móvil.
- Los clips usan *sprite swap* (intercambio de sprite), no deforman el transform.

## Boss

- **Boss.controller** — Animator Controller del jefe. Parámetros: `isMoving`, `isRunning` (bool), `attack` (trigger). Estados: Idle, Walk, Run, Attack.
- **BossIdle.anim** — Jefe parado (5 frames, en bucle).
- **BossWalk.anim** — Jefe caminando (5 frames, en bucle).
- **BossRun.anim** — Jefe corriendo (5 frames, en bucle).
- **BossAttack.anim** — Ataque del jefe (5 frames, sin bucle).

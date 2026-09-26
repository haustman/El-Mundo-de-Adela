using UnityEngine;
using UnityEngine.Serialization;

namespace ElMundoDeAdela
{
    /// <summary>
    /// Base de todo lo que tiene vida y puede recibir daño.
    /// La vida máxima se toma de la vida inicial la primera vez que hace falta.
    /// </summary>
    public abstract class Damageable : MonoBehaviour
    {
        [Header("Vida")]
        [SerializeField, FormerlySerializedAs("Health")] int health = 3;
        [Tooltip("Destruye el objeto cuando se queda sin vida.")]
        [SerializeField] bool destroyOnDeath = true;

        bool initialized;
        bool dead;

        public int Health => health;

        public int MaxHealth { get; private set; }

        public bool IsDead => dead;

        /// <summary>
        /// Fija la vida máxima y prepara el HUD. Se llama sola la primera vez que
        /// hace falta, para no depender del orden de los Start ni de un recargue
        /// de ensamblado (editar un script en Play deja el componente a medias).
        /// </summary>
        protected void EnsureReady()
        {
            if (initialized)
                return;

            initialized = true;
            MaxHealth = health;
            SetupHUD();
        }

        public void GetHit(int amount)
        {
            EnsureReady();

            if (dead || amount <= 0)
                return;

            health = Mathf.Max(health - amount, 0);
            RefreshHUD();

            if (health <= 0)
                Die();
        }

        protected virtual void SetupHUD() { }

        protected virtual void RefreshHUD() { }

        protected virtual void Die()
        {
            dead = true;

            if (destroyOnDeath)
                Destroy(gameObject);
        }
    }
}

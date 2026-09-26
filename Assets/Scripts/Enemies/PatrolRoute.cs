using System.Collections.Generic;
using UnityEngine;

namespace ElMundoDeAdela
{
    /// <summary>Punto de patrulla: destino y tiempo que tarda en recorrerlo.</summary>
    [System.Serializable]
    public class PatrolMovement
    {
        public Transform patrolPosition;
        public float duration;
    }

    /// <summary>
    /// Recorre en bucle la lista de puntos de patrulla.
    /// La posición se calcula interpolando para que el enemigo avance a velocidad constante.
    /// </summary>
    public class PatrolRoute
    {
        const float ArriveTolerance = 0.1f;

        readonly List<Vector3> points = new List<Vector3>();
        readonly List<float> durations = new List<float>();

        Vector3 origin;

        int index;
        float elapsed;

        public PatrolRoute(List<PatrolMovement> source, Vector3 startPosition)
        {
            origin = startPosition;

            if (source == null)
                return;

            foreach (var point in source)
            {
                if (!point.patrolPosition)
                    continue;

                points.Add(point.patrolPosition.position);
                durations.Add(Mathf.Max(point.duration, 0f));
            }
        }

        /// <summary>Hay ruta que recorrer.</summary>
        public bool IsValid => points.Count > 0;

        /// <summary>Número de puntos. Con menos de 2 no hay recorrido posible.</summary>
        public int Count => points.Count;

        /// <summary>Avance dentro del tramo actual, de 0 a 1.</summary>
        public float Progress
        {
            get
            {
                // Ruta sin puntos: no hay tramo que medir (evita salirse de la lista).
                if (points.Count == 0)
                    return 1f;

                var duration = durations[index];
                return duration <= 0f ? 1f : Mathf.Clamp01(elapsed / duration);
            }
        }

        /// <summary>Posición que le corresponde al enemigo y avance del reloj del tramo.</summary>
        public Vector3 Tick(float deltaTime)
        {
            elapsed += deltaTime;

            if (points.Count == 0)
                return origin;

            return Vector3.Lerp(origin, points[index], Progress);
        }

        /// <summary>True si el enemigo ya ha alcanzado el punto destino.</summary>
        public bool HasArrived(Vector3 position)
        {
            return Vector3.Distance(points[index], position) <= ArriveTolerance;
        }

        /// <summary>Pasa al siguiente punto. Devuelve true si con ello cierra la vuelta completa.</summary>
        public bool Next()
        {
            if (points.Count > 0)
                origin = points[index];

            index++;
            elapsed = 0f;

            if (index < points.Count)
                return false;

            index = 0;
            return true;
        }
    }
}

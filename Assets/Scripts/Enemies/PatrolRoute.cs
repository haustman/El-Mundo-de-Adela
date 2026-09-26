using System.Collections.Generic;
using UnityEngine;

/// <summary>Punto de patrulla: destino y tiempo que tarda en recorrerlo.</summary>
[System.Serializable]
public class PatrolMovement
{
    public Transform patrolPosition;
    public float duration;
}

/// <summary>
/// Recorre en bucle la lista de puntos de patrulla.
/// Se calcula la posicion por interpolacion, asi el enemigo se mueve a velocidad constante.
/// </summary>
public class PatrolRoute
{
    const float ArriveTolerance = 0.1f;

    readonly List<Vector3> points = new List<Vector3>();
    readonly List<float> durations = new List<float>();

    int index;
    float elapsed;

    public PatrolRoute(List<PatrolMovement> source)
    {
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

    /// <summary>Cuantos puntos tiene la ruta. Con menos de 2 no hay recorrido posible.</summary>
    public int Count => points.Count;

    /// <summary>Avance (0 a 1) dentro del tramo actual.</summary>
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

    /// <summary>Posicion que le corresponde al enemigo y avance del reloj del tramo.</summary>
    public Vector3 Tick(float deltaTime)
    {
        elapsed += deltaTime;

        var origin = points[index > 0 ? index - 1 : points.Count - 1];
        return Vector3.Lerp(origin, points[index], Progress);
    }

    /// <summary>True si el enemigo ya ha alcanzado el punto destino.</summary>
    public bool HasArrived(Vector3 position)
    {
        return Vector3.Distance(points[index], position) <= ArriveTolerance;
    }

    /// <summary>Pasa al siguiente punto. Devuelve true si con ello ha cerrado la vuelta completa.</summary>
    public bool Next()
    {
        index++;
        elapsed = 0f;

        if (index < points.Count)
            return false;

        index = 0;
        return true;
    }
}

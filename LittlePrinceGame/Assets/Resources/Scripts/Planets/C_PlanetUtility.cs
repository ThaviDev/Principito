using UnityEngine;
// Consultar planetas activos
public static class C_PlanetUtility
{
    public static C_PlanetGrav GetNearestPlanet(Vector2 position)
    {
        C_PlanetGrav nearest = null;
        float nearestSqrDist = float.MaxValue;
        var planets = C_PlanetGrav.AllPlanets;
        for (int i = 0; i < planets.Count; i++)
        {
            var p = planets[i];
            if (p == null) continue;

            // sqrMagnitude evita la raíz cuadrada -> más rápido que Vector2.Distance
            float sqrDist = ((Vector2)p.transform.position - position).sqrMagnitude;
            if (sqrDist < nearestSqrDist)
            {
                nearestSqrDist = sqrDist;
                nearest = p;
            }
        }
        return nearest;
    }
    public static C_PlanetGrav GetNearestPlanetInGravRadius(Vector2 position)
    {
        C_PlanetGrav nearest = null;
        float nearestSqrDist = float.MaxValue;

        var planets = C_PlanetGrav.AllPlanets;
        for (int i = 0; i < planets.Count; i++)
        {
            var p = planets[i];
            if (p == null) continue;

            float sqrDist = ((Vector2)p.transform.position - position).sqrMagnitude;
            float radiusSqr = p.GravityRadius * p.GravityRadius;

            if (sqrDist > radiusSqr) continue; // fuera de rango
            if (sqrDist < nearestSqrDist)
            {
                nearestSqrDist = sqrDist;
                nearest = p;
            }
        }
        return nearest;
    }

    public static Vector2 DirectionTo(C_PlanetGrav planet, Vector2 position)
    {
        if (planet == null) return Vector2.zero;
        return ((Vector2)planet.transform.position - position).normalized;
    }
}

using UnityEngine;

public class C_GravObjectPlayer : C_GravObject
{
    private float MultiplyGravity(C_PlanetGrav planet)
    {
        float gravityMultiplier = 1.0f; // Default multiplier
        float currentPlanetSize = planet.PlanetSize;
        float maxplanetSize = 10.0f; // Maximum planet size for normalization
        float scaleFactor = Mathf.Clamp01(currentPlanetSize / maxplanetSize); // Normalize the planet size to a value between 0 and 1
        // The smaller the planet, the stronger the gravity effect on the player object, changing gradually based on the planet size
        gravityMultiplier = Mathf.Lerp(1.5f, 1, scaleFactor); // Adjust the range as needed
        return gravityMultiplier; // You can modify this value based on your game's requirements
    }
    protected override void GravityForce(Vector2 v, C_PlanetGrav planet, float dist)
    {
        // Apply gravity force to the player object
        m_rb.AddForce(v.normalized * planet.GravityStrength * (planet.GravityRadius / dist) * MultiplyGravity(planet));
    }
}

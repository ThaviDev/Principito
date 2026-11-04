using UnityEngine;
using UnityEngine.InputSystem;

public class C_LaunchDebugger : MonoBehaviour
{
    public static C_LaunchDebugger Instance { get; private set; }
    private bool m_ShowDebug = false;

    // Aquí guardaremos los datos para mostrarlos
    private float m_LastFinalForce;
    private float m_FinalForceLimitless;
    private float m_LastDistFactor;
    private float m_LastTimeFactor;
    private float m_LastRawDistance;
    private float m_LastRawDuration;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current[Key.F1].wasPressedThisFrame)
        {
            m_ShowDebug = !m_ShowDebug;
        }
    }

    public void UpdateDebugInfo(float finalForce, float finalForceLimitless, float distFactor, float timeFactor, float rawDistance, float rawDuration)
    {
        m_LastFinalForce = finalForce;
        m_FinalForceLimitless = finalForceLimitless;
        m_LastDistFactor = distFactor;
        m_LastTimeFactor = timeFactor;
        m_LastRawDistance = rawDistance;
        m_LastRawDuration = rawDuration;
    }

    private void OnGUI()
    {
        if (!m_ShowDebug) return;
        if (!m_ShowDebug) return;

        // Estilo visual rápido
        GUIStyle style = new GUIStyle(GUI.skin.box);
        style.fontSize = 16;
        style.alignment = TextAnchor.UpperLeft;
        style.normal.textColor = Color.white;

        // Fondo semitransparente para que se lea bien
        GUI.backgroundColor = new Color(0, 0, 0, 0.8f);

        // Dibujar el panel (x, y, ancho, alto)
        GUILayout.BeginArea(new Rect(20, 20, 350, 350), style);

        GUILayout.Label("--- DEBUG LANZAMIENTO (F1) ---", style);
        GUILayout.Space(10);

        GUILayout.Label($"Fuerza Final: {m_LastFinalForce:F2}", style);
        GUILayout.Label($"FFinal Sin Limite: {m_FinalForceLimitless:F2}", style);
        GUILayout.Space(5);

        GUILayout.Label("--- Impacto ---", style);
        GUILayout.Label($"Factor Distancia: {m_LastDistFactor:F2}", style);
        GUILayout.Label($"Factor Tiempo: {m_LastTimeFactor:F2}", style);
        GUILayout.Space(5);

        GUILayout.Label("--- Datos Crudos ---", style);
        GUILayout.Label($"Distancia Gestos: {m_LastRawDistance:F2}", style);
        GUILayout.Label($"Duración Gesto: {m_LastRawDuration:F2} s", style);

        GUILayout.EndArea();
    }
}

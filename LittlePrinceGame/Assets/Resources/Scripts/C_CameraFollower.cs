using UnityEngine;

public class C_CameraFollower : MonoBehaviour
{
    [Header("Configuración Básica")]
    [SerializeField] Camera m_cam;
    [SerializeField] Transform m_target;
    [SerializeField] Rigidbody2D m_rb;
    [SerializeField] float m_yOffset;

    [Header("Zonas de Seguimiento")]
    [SerializeField] float m_softZoneX;
    [SerializeField] float m_softZoneY;
    [SerializeField] float m_deadZoneX;
    [SerializeField] float m_deadZoneY;

    [Header("Velocidad y Overshoot")]
    [SerializeField] float m_followSpeed;
    [SerializeField] float m_overshootIntensity = 0.5f; // Fuerza del overshoot
    [SerializeField] float m_velocityOvershootFactor = 0.1f; // Influencia de la velocidad
    [SerializeField] float m_maxOvershoot = 3f; // Límite máximo de desplazamiento

    [Header("Rotación")]
    [SerializeField] float m_rotationSpeed = 5f; // Velocidad de rotación hacia el objetivo

    float m_xSpeed;
    float m_ySpeed;
    Vector2 m_lastCamVelocity;

    void Update()
    {
        if (m_target == null)
        {
            Debug.LogWarning("CameraFollowPro: No se ha asignado un objetivo para seguir.");
            return;
        }

        Vector2 overshoot = CalculateOvershoot();
        Vector2 targetPosition = (Vector2)m_target.position + new Vector2(0, m_yOffset) + overshoot;

        HandleCameraMovement(targetPosition);
        ApplyDeadZones(targetPosition);

        Quaternion targetRotation = m_target.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, m_rotationSpeed * Time.deltaTime);
    }

    Vector2 CalculateOvershoot()
    {
        // Calcular dirección del movimiento basado en velocidad de la cámara
        Vector2 moveDirection = m_rb.linearVelocity.normalized;

        // Calcular magnitud del overshoot (base + influencia de velocidad)
        float velocityMagnitude = Mathf.Clamp(m_rb.linearVelocity.magnitude * m_velocityOvershootFactor, 0, m_maxOvershoot);
        float overshootAmount = m_overshootIntensity * velocityMagnitude;

        return moveDirection * overshootAmount;
    }

    void HandleCameraMovement(Vector2 targetPosition)
    {
        float targetCamDistanceX = m_cam.transform.position.x - targetPosition.x;
        float targetCamDistanceY = m_cam.transform.position.y - targetPosition.y;

        // Calcular distancias a las zonas suaves
        float positiveDistanceFromSoftZoneX = targetPosition.x - (m_softZoneX + m_cam.transform.position.x);
        float negativeDistanceFromSoftZoneX = targetPosition.x - (-m_softZoneX + m_cam.transform.position.x);
        float positiveDistanceFromSoftZoneY = targetPosition.y - (m_softZoneY + m_cam.transform.position.y);
        float negativeDistanceFromSoftZoneY = targetPosition.y - (-m_softZoneY + m_cam.transform.position.y);

        // Manejar movimiento en X
        if (targetCamDistanceX > m_softZoneX)
        {
            m_xSpeed = -m_followSpeed * -negativeDistanceFromSoftZoneX;
        }
        else if (targetCamDistanceX < -m_softZoneX)
        {
            m_xSpeed = m_followSpeed * positiveDistanceFromSoftZoneX;
        }
        else
        {
            m_xSpeed = 0;
        }

        // Manejar movimiento en Y
        if (targetCamDistanceY > m_softZoneY)
        {
            m_ySpeed = -m_followSpeed * -negativeDistanceFromSoftZoneY;
        }
        else if (targetCamDistanceY < -m_softZoneY)
        {
            m_ySpeed = m_followSpeed * positiveDistanceFromSoftZoneY;
        }
        else
        {
            m_ySpeed = 0;
        }

        // Aplicar velocidad
        m_rb.linearVelocity = new Vector2(m_xSpeed, m_ySpeed);
        m_lastCamVelocity = m_rb.linearVelocity;
    }

    void ApplyDeadZones(Vector2 targetPosition)
    {
        float targetCamDistanceX = m_cam.transform.position.x - targetPosition.x;
        float targetCamDistanceY = m_cam.transform.position.y - targetPosition.y;

        // Calcular distancias a las zonas muertas
        float positiveDistanceFromDeadZoneX = targetPosition.x - (m_deadZoneX + m_cam.transform.position.x);
        float negativeDistanceFromDeadZoneX = targetPosition.x - (-m_deadZoneX + m_cam.transform.position.x);
        float positiveDistanceFromDeadZoneY = targetPosition.y - (m_deadZoneY + m_cam.transform.position.y);
        float negativeDistanceFromDeadZoneY = targetPosition.y - (-m_deadZoneY + m_cam.transform.position.y);

        // Aplicar zonas muertas
        if (targetCamDistanceX > m_deadZoneX)
        {
            transform.position += new Vector3(negativeDistanceFromDeadZoneX, 0f, 0f);
        }
        if (targetCamDistanceX < -m_deadZoneX)
        {
            transform.position += new Vector3(positiveDistanceFromDeadZoneX, 0f, 0f);
        }
        if (targetCamDistanceY > m_deadZoneY)
        {
            transform.position += new Vector3(0f, negativeDistanceFromDeadZoneY, 0f);
        }
        if (targetCamDistanceY < -m_deadZoneY)
        {
            transform.position += new Vector3(0f, positiveDistanceFromDeadZoneY, 0f);
        }
    }
}

/*
using UnityEngine;

public class CameraFollowPro : MonoBehaviour
{
    [SerializeField] Camera _cam;
    [SerializeField] Transform _target;
    [SerializeField] Rigidbody2D _rb;
    [SerializeField] float _yOffset;
    [SerializeField] float _softZoneX;
    [SerializeField] float _softZoneY;
    [SerializeField] float _deadZoneX;
    [SerializeField] float _deadZoneY;
    [SerializeField] float _followSpeed;

    float _xSpeed;
    float _ySpeed;
    void Start()
    {

    }

    void Update()
    {
        var targetPosY = _target.position.y + _yOffset;
        //var ySpeed;
        var targetCamDistanceDiferenceX = _cam.transform.position.x - _target.position.x;
        var targetCamDistanceDiferenceY = _cam.transform.position.y - targetPosY;

        //Debug.Log(_target.position.x - (_softZoneX + _cam.transform.position.x));
        var positiveDistanceFromSoftZoneX = _target.position.x - (_softZoneX + _cam.transform.position.x);
        var negativeDistanceFromSoftZoneX = _target.position.x - (-_softZoneX + _cam.transform.position.x);
        var positiveDistanceFromSoftZoneY = targetPosY - (_softZoneY + _cam.transform.position.y);
        var negativeDistanceFromSoftZoneY = targetPosY - (-_softZoneY + _cam.transform.position.y);

        if (targetCamDistanceDiferenceX > _softZoneX)
        {
            _xSpeed = -_followSpeed * -negativeDistanceFromSoftZoneX;
        }
        else if (targetCamDistanceDiferenceX < -_softZoneX)
        {
            _xSpeed = _followSpeed * positiveDistanceFromSoftZoneX;
        }
        else
        {
            _xSpeed = 0;
        }
        if (targetCamDistanceDiferenceY > _softZoneY)
        {
            _ySpeed = -_followSpeed * -negativeDistanceFromSoftZoneY;
        }
        else if (targetCamDistanceDiferenceY < -_softZoneY)
        {
            _ySpeed = _followSpeed * positiveDistanceFromSoftZoneY;
        }
        else
        {
            _ySpeed = 0;
        }
        _rb.linearVelocity = new Vector3(_xSpeed, _ySpeed);

        var positiveDistanceFromDeadZoneX = _target.position.x - (_deadZoneX + _cam.transform.position.x);
        var negativeDistanceFromDeadZoneX = _target.position.x - (-_deadZoneX + _cam.transform.position.x);
        var positiveDistanceFromDeadZoneY = targetPosY - (_deadZoneY + _cam.transform.position.y);
        var negativeDistanceFromDeadZoneY = targetPosY - (-_deadZoneY + _cam.transform.position.y);
        if (targetCamDistanceDiferenceX > _deadZoneX)
        {
            transform.position += new Vector3(negativeDistanceFromDeadZoneX, 0f, 0f);
        }
        if (targetCamDistanceDiferenceX < -_deadZoneX)
        {
            transform.position += new Vector3(positiveDistanceFromDeadZoneX, 0f, 0f);
        }
        if (targetCamDistanceDiferenceY > _deadZoneY)
        {
            transform.position += new Vector3(0f, negativeDistanceFromDeadZoneY, 0f);
        }
        if (targetCamDistanceDiferenceY < -_deadZoneY)
        {
            transform.position += new Vector3(0f, positiveDistanceFromDeadZoneY, 0f);
        }
    }
}
*/
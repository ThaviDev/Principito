using UnityEngine;

public class C_ShareTransform : MonoBehaviour
{
    [SerializeField] private Transform m_targetTransform;
    [SerializeField] private bool m_sharePosition = true;
    [SerializeField] private bool m_shareRotation = true;
    [SerializeField] private bool m_shareScale = true;
    [SerializeField] private Vector3 m_positionOffset;
    [SerializeField] private Vector3 m_scaleOffset;
    private void Update()
    {
        transform.position = m_sharePosition ? m_targetTransform.position + m_positionOffset : transform.position;
        transform.rotation = m_shareRotation ? m_targetTransform.rotation : transform.rotation;
        transform.localScale = m_shareScale ? m_targetTransform.localScale + m_scaleOffset : transform.localScale;
    }
}

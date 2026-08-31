using System.Collections;
using UnityEngine;

public class C_OptionsPanel : MonoBehaviour
{
    [SerializeField] private RectTransform m_MyRectTransform;
    [SerializeField] private Vector2 m_UnActivePos;
    [SerializeField] private Vector2 m_ActivePos;
    [SerializeField] private float m_MoveDuration;
    [SerializeField] private bool m_IsActive;

    private Coroutine m_CurrentMoveCoroutine;

    public void EventActivatePanel(bool NewActive)
    {
        if (NewActive != m_IsActive)
        {
            m_IsActive = NewActive;
            Vector2 targetPos = NewActive ? m_ActivePos : m_UnActivePos;
            StartMoveCouritine(targetPos);
        }
    }
    public void EventFlipFlipPanel()
    {
        m_IsActive = !m_IsActive;
        Vector2 targetPos = m_IsActive ? m_ActivePos : m_UnActivePos;
        StartMoveCouritine(targetPos);

        Debug.Log("Cambio estado de menu a:" + m_IsActive);
    }

    private void StartMoveCouritine(Vector2 target)
    {
        if (m_CurrentMoveCoroutine != null)
            StopCoroutine(m_CurrentMoveCoroutine);

        m_CurrentMoveCoroutine = StartCoroutine(MoveOverTime(target, m_MoveDuration));
    }
    private IEnumerator MoveOverTime(Vector2 target, float time)
    {
        Vector2 startPosition = m_MyRectTransform.anchoredPosition;
        float elapsedTime = 0f;

        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / time;
            t = Mathf.Clamp01(t);

            float easedT = 1f - Mathf.Pow(1f - t, 3f);

            m_MyRectTransform.anchoredPosition = Vector2.Lerp(startPosition, target, easedT);
            yield return null;
        }
        m_MyRectTransform.anchoredPosition = target;
        m_CurrentMoveCoroutine = null;
    }
}

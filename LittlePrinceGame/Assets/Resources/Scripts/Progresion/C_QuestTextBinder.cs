using UnityEngine;

[RequireComponent(typeof(C_SetTextVariable))]
public class C_QuestTextBinder : MonoBehaviour
{
    private C_SetTextVariable m_TextVariable;
    private void Awake()
    {
        m_TextVariable = GetComponent<C_SetTextVariable>();
    }
    private void OnEnable()
    {
        if (C_QuestManager.Instance == null) return;
        C_QuestManager.Instance.OnSequentialMissionChanged += HandleMissionChanged;
    }
    private void OnDisable()
    {
        if (C_QuestManager.Instance == null) return;
        C_QuestManager.Instance.OnSequentialMissionChanged -= HandleMissionChanged;
    }
    private void HandleMissionChanged(SCOB_QuestData mission)
    {
        if (mission == null)
        {
            m_TextVariable.Clear();
            return;
        }

        if (mission.TryGetDisplayData(
                out IntSCOB v1, out IntSCOB v2,
                out string prefix, out string middle, out string postfix))
        {
            m_TextVariable.SetVariables(v1, v2, prefix, middle, postfix);
            //m_TextVariable.SetTextActive(true);
        }
        else
        {
            // La misión no usa variables -> mostramos solo el título
            m_TextVariable.Clear();
            //m_TextVariable.SetTextActive(false);
        }
    }
}

using UnityEngine;
using TMPro;

public class C_SetTextVariable : MonoBehaviour
{
    [SerializeField] private IntSCOB m_VariableValue1;
    [SerializeField] private IntSCOB m_VariableValue2;
    [SerializeField] private TextMeshProUGUI m_Text;
    [SerializeField] private string m_Prefix = "";
    [SerializeField] private string m_Middle = "";
    [SerializeField] private string m_Postfix = "";
    void Start()
    {
        SubscribeTo(m_VariableValue1);
        SubscribeTo(m_VariableValue2);
    }
    private void OnDestroy()
    {
        UnsubscribeFrom(m_VariableValue1);
        UnsubscribeFrom(m_VariableValue2);
    }
    void UpdateText()
    {
        m_Text.text = 
            m_Prefix +
            (m_VariableValue1 != null ? m_VariableValue1.Value.ToString() : "") +
            m_Middle +
            (m_VariableValue2 != null ? m_VariableValue2.Value.ToString() : "") + 
            m_Postfix;
    }
    private void SubscribeTo(IntSCOB v)
    {
        if (v != null) v.A_OnValueChange += HandleValueChanged;
    }
    private void UnsubscribeFrom(IntSCOB v)
    {
        if (v != null) v.A_OnValueChange -= HandleValueChanged;
    }
    // Cuando el valor de una variable cambia, actualizamos el texto. El parámetro int _ es ignorado porque no necesitamos el valor en sí.
    private void HandleValueChanged(int _) => UpdateText();
    public void SetVariables(IntSCOB v1, IntSCOB v2,
                         string prefix, string middle, string postfix)
    {
        // 1. Desuscribirse de las viejas
        UnsubscribeFrom(m_VariableValue1);
        UnsubscribeFrom(m_VariableValue2);

        // 2. Guardar las nuevas
        m_VariableValue1 = v1;
        m_VariableValue2 = v2;
        m_Prefix = prefix;
        m_Middle = middle;
        m_Postfix = postfix;

        // 3. Suscribirse a las nuevas
        SubscribeTo(m_VariableValue1);
        SubscribeTo(m_VariableValue2);

        // 4. Refrescar
        UpdateText();
    }
    public void Clear()
    {
        UnsubscribeFrom(m_VariableValue1);
        UnsubscribeFrom(m_VariableValue2);
        m_VariableValue1 = null;
        m_VariableValue2 = null;
        if (m_Text != null) m_Text.text = "";
    }

}

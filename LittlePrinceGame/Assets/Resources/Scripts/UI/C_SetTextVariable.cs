using UnityEngine;

public class C_SetTextVariable : MonoBehaviour
{
    [SerializeField] private IntSCOB m_VariableValue;
    [SerializeField] private TMPro.TextMeshProUGUI m_Text;
    [SerializeField] private string m_Prefix = "";
    [SerializeField] private string m_Postfix = "";
    void Start()
    {
        m_VariableValue.A_OnValueChange += (newValue) => UpdateText();
    }
    void UpdateText()
    {
        m_Text.text = m_Prefix + m_VariableValue.Value.ToString() + m_Postfix;
    }
}

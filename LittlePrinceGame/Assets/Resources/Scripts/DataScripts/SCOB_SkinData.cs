using UnityEngine;
[CreateAssetMenu(fileName = "SkinData", menuName = "Data/SkinData")]
public class SCOB_SkinData : ScriptableObject
{
    [SerializeField] private Color m_Color1;
    [SerializeField] private Color m_Color2;
    [SerializeField] private Color m_Color3;
    [SerializeField] private Color m_ColorScarf;
    public Color Color1 => m_Color1;
    public Color Color2 => m_Color2;
    public Color Color3 => m_Color3;
    public Color ColorScarf => m_ColorScarf;
}

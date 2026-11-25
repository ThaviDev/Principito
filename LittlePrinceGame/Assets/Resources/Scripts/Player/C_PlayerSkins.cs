using UnityEngine;

public class C_PlayerSkins : MonoBehaviour
{
    [SerializeField] private SCOB_SkinData[] m_ColorSkins;

    [Header("Skin References")]
    [SerializeField] private SpriteRenderer m_ColorSkin;
    [SerializeField] private ParticleSystem m_Scarf;
    private ParticleSystem.MainModule m_ScarfMain;
    private Material m_ColorSkinMaterial;

    [Header("testing")]
    [SerializeField] private int m_CurrentSkinIndex = 0;
    private void Start()
    {
        m_ColorSkinMaterial = m_ColorSkin.material;
        m_ScarfMain = m_Scarf.main;
    }
    private void Update()
    {
        // For testing purposes, change skins with number keys, but the input system is using the old input system, i have to change it to the new input system
        ApplySkin(m_CurrentSkinIndex);
    }
    public void ApplySkin(int skinIndex) 
    {
        if (skinIndex < 0 || skinIndex >= m_ColorSkins.Length) return;
        m_ScarfMain.startColor = m_ColorSkins[skinIndex].ColorScarf;
        m_ColorSkinMaterial.SetColor("_Color1", m_ColorSkins[skinIndex].Color1);
        m_ColorSkinMaterial.SetColor("_Color2", m_ColorSkins[skinIndex].Color2);
        m_ColorSkinMaterial.SetColor("_Color3", m_ColorSkins[skinIndex].Color3);
    }
}

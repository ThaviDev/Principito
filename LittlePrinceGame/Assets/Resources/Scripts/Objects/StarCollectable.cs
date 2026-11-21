using UnityEngine;

public class StarCollectable : MonoBehaviour, I_Collectable
{
    [SerializeField] private int m_value = 1;
    [SerializeField] private GameObject m_collectParticle;
    private C_StarTrail m_trail;
    public int Value => m_value;
    private void Start()
    {
        m_trail = GetComponentInChildren<C_StarTrail>();
    }
    public void Collect(I_Collector who)
    {
        Instantiate(m_collectParticle, transform.position, Quaternion.identity);
        who.CollectThing(this);
        C_SoundManager.A_PlaySFX?.Invoke(SFXTypes.ColectablePickup, 1f);
        //var trail = GetComponentInChildren<TrailRenderer>();
        m_trail?.transform.SetParent(null);
        m_trail.StopPlay = 1f;
        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null) return;
        if (collision.CompareTag("Player"))
        {
            if (collision.TryGetComponent<I_Collector>(out I_Collector collector))
            {
                Collect(collector);
                return;
            }
            Debug.LogWarning("El objeto que colisionó con el " +
                "coleccionable no tiene el componente I_Collector. Por lo tanto " +
                "No se puede sumar.");
        }
    }

    /*
    [SerializeField] private int CollectableValue = 1;
    protected override void Collect(C_PlayerCollectables pyr)
    {
        pyr.AddStars(CollectableValue);
        // Instanciar efecto de estrella aqui
        base.Colection(pyr);
    }
    public int Value => CollectableValue;
    */

}

using UnityEngine;

public class StarCollectable : MonoBehaviour, I_Collectable
{
    [SerializeField] private int _value = 1;
    public int Value => _value;
    public void Collect(I_Collector who)
    {
        who.CollectThing(this);
        C_SoundManager.A_PlaySFX?.Invoke(SFXTypes.ColectablePickup, 1f);
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

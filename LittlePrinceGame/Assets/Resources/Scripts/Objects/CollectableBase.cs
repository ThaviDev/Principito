using UnityEngine;

public class CollectableBase : MonoBehaviour
{
    /*
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent<C_PlayerCollectables>(out C_PlayerCollectables pyr))
            {
                Collect(pyr);
                return;
            }
            Debug.LogWarning("El objeto que colisionó con el " +
                "coleccionable no tiene el componente C_PlayerCollectables. Por lo tanto " +
                "No se puede sumar.");
        }
    }
    protected virtual void Collect(C_PlayerCollectables pyr)
    {
        C_SoundManager.A_PlaySFX?.Invoke(SFXTypes.ColectablePickup, 1f);
        Destroy(gameObject);
    }*/
}

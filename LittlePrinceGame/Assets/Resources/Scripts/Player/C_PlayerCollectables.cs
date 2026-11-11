using System;
using UnityEngine;

public class C_PlayerCollectables : MonoBehaviour, I_Collector
{
    [SerializeField] IntSCOB m_Stars;

    public Action<int> A_OnStarsChange;
    private void StarsChangeEvent(int newStars)
    {
        m_Stars.Add(newStars);
        A_OnStarsChange?.Invoke(newStars);
    }
    public void CollectThing(I_Collectable collectable)
    {
        if (collectable.Value > 0)
        {
            StarsChangeEvent(collectable.Value);
        }
    }
}

using System;
using UnityEngine;

public class C_PlayerCollectables : MonoBehaviour, I_Collector
{
    [SerializeField]IntSCOB Stars;
    public Action<int> A_OnStarsChange;
    private void StarsChangeEvent(int newStars)
    {
        Stars.Add(newStars);
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

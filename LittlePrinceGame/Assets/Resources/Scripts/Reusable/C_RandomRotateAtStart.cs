using UnityEngine;

public class C_RandomRotateAtStart : MonoBehaviour
{
    private void Start()
    {
        float randomRotation = Random.Range(0f, 360f);
        transform.rotation = Quaternion.Euler(0f, 0f, randomRotation);
    }
}

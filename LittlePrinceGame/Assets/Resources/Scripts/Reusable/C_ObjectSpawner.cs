using UnityEngine;

public class C_ObjectSpawner : MonoBehaviour
{
    [SerializeField] private GameObject m_ObjectToSpawn;
    public GameObject ObjectToSpawn { get { return m_ObjectToSpawn; } set { m_ObjectToSpawn = value; } }
    public void SpawnObject(Vector3 position, Quaternion rotation)
    {
        if (m_ObjectToSpawn != null)
        {
            Instantiate(m_ObjectToSpawn, position, rotation);
        }
        else
        {
            Debug.LogWarning("No object assigned to spawn.");
        }
    }
    public void SpawnObject(Vector3 position)
    {
        SpawnObject(position, Quaternion.identity);
    }
    public void SpawnObject(Quaternion rotation)
    {
        SpawnObject(transform.position, rotation);
    }
    public void SpawnObject()
    {
        SpawnObject(transform.position, Quaternion.identity);
    }
}

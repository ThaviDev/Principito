using UnityEngine;

public class C_StarTrail : MonoBehaviour
{
    private float m_StopPlay = 0;
    public float StopPlay { set { m_StopPlay = value; }}
    void Update()
    {
        if (m_StopPlay > 0)
        {
            m_StopPlay -= Time.deltaTime;
            Debug.Log("Tiempo restante: " + m_StopPlay);
            if (m_StopPlay <= 0.1f)
            {
                print("Me destruyo porque se acabo el tiempo");
                Destroy(gameObject);
            }
        }
    }
}

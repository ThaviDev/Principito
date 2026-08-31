using UnityEngine;

public class C_TestLeanFunctionality : MonoBehaviour
{
    public void TriggerTouchDown(Vector3 touchPos)
    {
        Debug.Log("Touch Down Triggered at position: " + touchPos);
    }
    public void TriggerTouchUpdate(Vector3 touchPos)
    {
        Debug.Log("Touch Update Triggering in position: " + touchPos);
    }
    public void TriggerTouchUp(Vector3 touchPos)
    {
        Debug.Log("Touch Up Triggered at position: " + touchPos);
    }
}

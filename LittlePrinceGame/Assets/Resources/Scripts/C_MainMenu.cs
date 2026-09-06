using UnityEngine;

public class C_MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        C_GameManager.A_OnGameStart?.Invoke();
    }
}

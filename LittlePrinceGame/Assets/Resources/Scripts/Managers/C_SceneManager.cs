using System;
using UnityEngine;
public enum SceneName
{
    TestGameplay,
    TestMainMenu,
    MainMenu,
    Level1,
    Level2,
    Level3,
    GameOver,
    Victory
}
public class C_SceneManager : MonoBehaviour
{
    private static C_SceneManager _instance;
    public static C_SceneManager Instance
    {
        get {
            if (_instance == null)
            {
                // Buscar una instancia existente en la escena.
                _instance = FindAnyObjectByType<C_SceneManager>();
                if (_instance == null)
                {
                    // Crear un nuevo GameObject con el script adjunto si no se encuentra ninguna instancia.
                    GameObject singletonObject = new GameObject("Scene Manager");
                    _instance = singletonObject.AddComponent<C_SceneManager>();
                    // Opcional: Evitar que el objeto sea destruido al cambiar de escena.
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return _instance;
        }
    }

    public static Action<SceneName> A_LoadScene;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject); // Evitar que el objeto sea destruido al cambiar de escena.
        }
        else if (_instance != this)
        {
            Destroy(gameObject); // Destruir instancias adicionales si ya existe una instancia.
        }
    }
    private void Start()
    {
        A_LoadScene += LoadScene;
    }
    public void LoadScene(SceneName sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName.ToString());
    }
    private void OnDestroy()
    {
        A_LoadScene -= LoadScene;
    }
    public void LoadSceneByNameEvent(string sceneName)
    {
        if (Enum.TryParse(sceneName, out SceneName parsedScene))
        {
            LoadScene(parsedScene);
        }
        else
        {
            Debug.LogWarning($"Scene name '{sceneName}' is not valid.");
        }
    }
}

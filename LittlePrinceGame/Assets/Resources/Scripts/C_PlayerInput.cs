using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class C_PlayerInput : MonoBehaviour
{
    private static C_PlayerInput _instance;
    static PlayerInput _input;

    [SerializeField] private InputActionAsset _inputAsset; // Asset asignable desde Inspector

    public static C_PlayerInput Instance
    {
        get
        {
            if (_instance == null)
            {
                // Buscar una instancia existente en la escena.
                _instance = FindAnyObjectByType<C_PlayerInput>();

                if (_instance == null)
                {
                    // Crear un nuevo GameObject con el script adjunto si no se encuentra ninguna instancia.
                    GameObject singletonObject = new GameObject("Input Manager");
                    _instance = singletonObject.AddComponent<C_PlayerInput>();
                    DontDestroyOnLoad(singletonObject);

                    // Asignar valores indispensables
                    _instance.SetupInputComponent();
                }
            }
            return _instance;
        }
    }

    Vector2 _move;
    Vector2 _aim;
    bool _run;
    bool _dash;
    bool _flashlight;
    bool _interact;
    bool _useItem;
    bool _dropItem;
    bool _previousItem;
    bool _nextItem;
    bool _pause;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _input = GetComponent<PlayerInput>(); // Obtiene el PlayerInput del mismo objeto
            DontDestroyOnLoad(gameObject); // Evitar que el objeto sea destruido al cambiar de escena.
        }
        else if (_instance != this)
        {
            Destroy(gameObject); // Destruir instancias adicionales si ya existe una instancia.
        }
    }

    private void SetupInputComponent()
    {
        // 1. Obtener o crear PlayerInput
        if (!TryGetComponent(out _input))
        {
            _input = gameObject.AddComponent<PlayerInput>();
        }

        // 2. Configurar Input Action Asset
        if (_input.actions == null)
        {
            // Intento 1: Usar asset serializado (si se asignó en Inspector)
            if (_inputAsset != null)
            {
                _input.actions = _inputAsset;
            }
            // Intento 2: Cargar desde Resources
            else
            {
                _inputAsset = Resources.Load<InputActionAsset>("InputSystem_Actions");

                if (_inputAsset != null)
                {
                    _input.actions = _inputAsset;
                }
                else
                {
                    Debug.LogError($"Input Action Asset no encontrado en: Resources/InputSystem_Actions");
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#endif
                }
            }
        }

        // 3. Activar el sistema de input
        if (_input.actions != null && !_input.inputIsActive)
        {
            _input.ActivateInput();
        }
    }
    public Vector2 MovementVector { get { return _move; } }
    public Vector2 AimingVector { get { return _aim; } }
    public bool RuningBool { get { return _run; } }
    public bool DashBool { get { return _dash; } }
    public bool FlashLightBool { get { return _flashlight; } }
    public bool InteractAndPickUpItemBool { get { return _interact; } }
    public bool UseItemBool { get { return _useItem; } }
    public bool DropItemBool { get { return _dropItem; } }
    public bool PauseBool { get { return _pause; } }
    public bool NextItemBool { get { return _nextItem; } }
    public bool PreviousItemBool { get { return _previousItem; } }

    private void Update()
    {
        _move = OnMoveChange();
        /*
        _aim = OnAimChange();
        _run = OnRunPressed();
        _dash = OnDashPressed();
        _flashlight = OnFlashLightPressed();
        _interact = OnInteractPressed();
        _useItem = OnItemPressed();
        _dropItem = OnDropPressed();
        _pause = OnPausePressed();
        _nextItem = OnNextItemPressed();
        _previousItem = OnPreviousItemPressed();
        */
    }

    static Vector2 OnMoveChange()
    {
        return _input.actions.FindAction("Move").ReadValue<Vector2>();
        // .IsPressed(), .WasPressedThisFrame, .WasReleasedThisFrame
        // .ReadValue<Float>, .ReadValue<Vector2>
    }
    /*
    static Vector2 OnAimChange()
    {
        return _input.actions.FindAction("Look").ReadValue<Vector2>();
    }
    static bool OnRunPressed()
    {
        return _input.actions.FindAction("Speed Control").WasReleasedThisFrame();
    }
    static bool OnDashPressed()
    {
        return _input.actions.FindAction("Dash Jump").WasReleasedThisFrame();
    }
    static bool OnFlashLightPressed()
    {
        return _input.actions.FindAction("FlashLight").WasReleasedThisFrame();
    }
    static bool OnInteractPressed()
    {
        return _input.actions.FindAction("Interaction And Pick Item").WasReleasedThisFrame();
    }
    static bool OnItemPressed()
    {
        return _input.actions.FindAction("Use Item").WasReleasedThisFrame();
    }
    static bool OnDropPressed()
    {
        return _input.actions.FindAction("Drop Item").WasReleasedThisFrame();
    }
    static bool OnPausePressed()
    {
        return _input.actions.FindAction("Pause").WasReleasedThisFrame();
    }
    static bool OnNextItemPressed()
    {
        return _input.actions.FindAction("Next Item").WasReleasedThisFrame();
    }
    static bool OnPreviousItemPressed()
    {
        return _input.actions.FindAction("Previous Item").WasReleasedThisFrame();
    }

    */
}

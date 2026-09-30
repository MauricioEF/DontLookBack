using UnityEngine;

public sealed class CoreRuntime : MonoBehaviour
{
    public static CoreRuntime Instance
    {
        get;
        private set;
    }

    [Header("Core Services")]
    [SerializeField] private SceneFlowService sceneFlowService;
    [SerializeField] private InputRoot inputRoot;
    [SerializeField] private InputModeManager inputModeManager;

    public SceneFlowService SceneFlow => sceneFlowService;
    public InputRoot Input => inputRoot;
    public InputModeManager ModeManager => inputModeManager;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        inputRoot.Initialize();
        inputModeManager.Initialize(inputRoot.Actions);
        DontDestroyOnLoad(gameObject);
    }
}

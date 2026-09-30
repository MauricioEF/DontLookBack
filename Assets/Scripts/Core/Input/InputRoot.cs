using UnityEngine;

[DefaultExecutionOrder(-1000)]
public sealed class InputRoot : MonoBehaviour
{
    public InputActions Actions
    {
        get;
        private set;
    }

    public void Initialize()
    {
        Actions = new InputActions();
        Actions.Disable();
    }
    private void OnDestroy()
    {
        Actions?.Disable();
        Actions?.Dispose();
        Actions = null;
    }
}

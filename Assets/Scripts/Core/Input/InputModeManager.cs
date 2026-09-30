using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum InputMode
{
    Gameplay
}

public sealed class InputModeManager : MonoBehaviour
{
    private InputActions actions;
    private readonly List<ModeRequest> requests = new();
    private int nextRequestId;
    public InputMode CurrentMode
    {

        get;
        private set;
    } = InputMode.Gameplay;

    public void Initialize(InputActions actions)
    {
        this.actions = actions;
        ApplyCurrentMode(force: true);
    }
    public InputModeLease AcquireMode(InputMode mode)
    {
        int id = ++nextRequestId;
        requests.Add(new ModeRequest(id, mode));
        ApplyCurrentMode();
        return new InputModeLease(this, id);
    }

    public void ApplyCurrentMode(bool force = false)
    {
        InputMode mode = requests.Count == 0 ? InputMode.Gameplay : requests[^1].Mode;
        if (!force && CurrentMode == mode)
            return;
        CurrentMode = mode;
        ApplyActionMaps(mode);
    }

    private void ApplyActionMaps(InputMode mode)
    {
        bool enableGameplay = mode == InputMode.Gameplay;

        SetMapEnabled(actions.Gameplay.Get(), enableGameplay);
    }

    internal void ReleaseMode(int id)
    {
        int index = requests.FindIndex(request => request.id == id);
        if (index < 0)
            return;
        if (index >= 0)
            requests.RemoveAt(index);
        ApplyCurrentMode();
    }

    private static void SetMapEnabled(InputActionMap actionMap, bool shouldBeEnabled)
    {
        if (shouldBeEnabled)
        {
            if (!actionMap.enabled)
            {
                actionMap.Enable();
            }
            return;
        }
        if (actionMap.enabled)
        {
            actionMap.Disable();
        }
    }
    private readonly struct ModeRequest
    {
        public int id
        {
            get;
        }
        public InputMode Mode
        {
            get;
        }
        public ModeRequest(int id, InputMode mode)
        {
            this.id = id;
            this.Mode = mode;
        }
    }
}

using System;

public class InputModeLease : IDisposable
{
    private InputModeManager owner;
    private readonly int id;

    public InputModeLease(InputModeManager owner, int id)
    {
        this.owner = owner;
        this.id = id;
    }

    public void Dispose()
    {
        if (owner == null)
        {
            return;
        }
        owner.ReleaseMode(id);
        owner = null;
    }
}

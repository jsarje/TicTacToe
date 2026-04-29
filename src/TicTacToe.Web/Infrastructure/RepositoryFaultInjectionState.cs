namespace TicTacToe.Web.Infrastructure;

/// <summary>
/// Stores deterministic fault injection flags for integration tests.
/// </summary>
public sealed class RepositoryFaultInjectionState
{
    private int pendingLoadFailures;
    private int pendingSaveFailures;

    public void TriggerLoadFailure()
    {
        Interlocked.Increment(ref pendingLoadFailures);
    }

    public void TriggerSaveFailure()
    {
        Interlocked.Increment(ref pendingSaveFailures);
    }

    public bool ConsumeLoadFailure()
    {
        return Interlocked.Exchange(ref pendingLoadFailures, 0) > 0;
    }

    public bool ConsumeSaveFailure()
    {
        return Interlocked.Exchange(ref pendingSaveFailures, 0) > 0;
    }
}
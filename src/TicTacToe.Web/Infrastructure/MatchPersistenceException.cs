namespace TicTacToe.Web.Infrastructure;

/// <summary>
/// Represents a temporary failure while loading or saving authoritative match state.
/// </summary>
public sealed class MatchPersistenceException : Exception
{
    public MatchPersistenceException(string message)
        : base(message)
    {
    }

    public MatchPersistenceException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
namespace TicTacToe.Web.Client.Services;

/// <summary>
/// Represents an API failure visible to the client UI.
/// </summary>
public sealed class GameApiException(string message) : Exception(message);
using System.Text.Json.Serialization;

namespace TicTacToe.Core.Models;

 [JsonConverter(typeof(JsonStringEnumConverter))]
/// <summary>
/// Represents the current outcome of a game session.
/// </summary>
public enum GameResult
{
    InProgress,
    XWins,
    OWins,
    Draw,
}
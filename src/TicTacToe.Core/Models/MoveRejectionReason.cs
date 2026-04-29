using System.Text.Json.Serialization;

namespace TicTacToe.Core.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
/// <summary>
/// Defines the gameplay reasons a move can be rejected without infrastructure failure.
/// </summary>
public enum MoveRejectionReason
{
    OccupiedSquare,
    WrongTurn,
    MatchComplete,
    StaleRevision,
}
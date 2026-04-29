using System.Text.Json.Serialization;

namespace TicTacToe.Core.Models;

 [JsonConverter(typeof(JsonStringEnumConverter))]
/// <summary>
/// Identifies the player mark used in a session.
/// </summary>
public enum PlayerMark
{
    X,
    O,
}